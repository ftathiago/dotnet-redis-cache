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

    public CacheAccessor(
        ILogger<TObject> logger,
        CacheEntryConfiguration cacheConfiguration,
        IDistributedCache cache)
    {
        _logger = logger;
        _cacheConfiguration = cacheConfiguration;
        _cache = cache;
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
                    .SetAbsoluteExpiration(_cacheConfiguration.CacheDuration)
                    .SetSlidingExpiration(_cacheConfiguration.CacheSlidingDuration));
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogError(eventId: default, ex, ErrorUpdating);
        }
    }

    public async Task<TObject?> GetAsync(CacheKey<TObject> key)
    {
        try
        {
            var cacheValue = await _cache.GetStringAsync(key.Value);
            if (cacheValue is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<TObject>(cacheValue);
        }
        catch (RedisConnectionException ex)
        {
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
            _logger.LogError(
                eventId: default,
                ex,
                ErrorRemoving);
        }
    }
}
