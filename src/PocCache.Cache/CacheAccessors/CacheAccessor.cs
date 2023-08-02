using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace PocCache.Cache.CacheAccessors;

internal class CacheAccessor<TObject> : ICacheAccessor<TObject>
{
    private const string ErrorUpdating =
        "Error while updating cache. This could make requests slowly.";

    private const string ErrorGetting =
        "Error while retrieving data from cache. This could make requests slowly.";

    private const string ErrorRemoving =
        "Error while removing data from cache. This could make requests slowly.";

    private readonly ILogger<TObject> _logger;
    private readonly CacheEntryConfiguration _cacheConfiguration;
    private readonly IDistributedCache _cache;
    private readonly CacheMonitor _cacheMonitor;

    public CacheAccessor(
        ILogger<TObject> logger,
        CacheEntryConfiguration cacheConfiguration,
        IDistributedCache cache,
        CacheMonitor cacheMonitor)
    {
        _logger = logger;
        _cacheConfiguration = cacheConfiguration;
        _cache = cache;
        _cacheMonitor = cacheMonitor;
    }

    public async Task SetAsync(CacheKey<TObject> key, TObject? instance)
    {
        if (instance is null)
        {
            return;
        }

        try
        {
            var serialized = JsonSerializer.Serialize(instance);
            await _cache.SetStringAsync(
                key.Value,
                serialized,
                new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow.Add(_cacheConfiguration.CacheDuration),
                    SlidingExpiration = _cacheConfiguration.CacheSlidingDuration,
                });
        }
        catch (RedisConnectionException ex)
        {
            _cacheMonitor.UpdateCache(false);
            _logger.LogError(eventId: default, ex, ErrorUpdating);
        }
    }

    public async Task<TObject?> GetAsync(CacheKey<TObject> key)
    {
        try
        {
            var cacheValue = await _cache.GetStringAsync(key.Value);
            if (string.IsNullOrEmpty(cacheValue))
            {
                return default;
            }

            return JsonSerializer.Deserialize<TObject>(cacheValue);
        }
        catch (RedisConnectionException ex)
        {
            _cacheMonitor.UpdateCache(false);
            _logger.LogError(
                eventId: default,
                ex,
                ErrorGetting);

            return default;
        }
    }

    public async Task RemoveAsync(CacheKey<TObject> key)
    {
        try
        {
            await _cache.RemoveAsync(key.Value);
        }
        catch (RedisConnectionException ex)
        {
            _cacheMonitor.UpdateCache(false);
            _logger.LogError(
                eventId: default,
                ex,
                ErrorRemoving);
        }
    }
}
