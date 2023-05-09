using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace PocCache.Cache.CacheAccessors;

internal class CacheAccessor<TObject> : ICacheAccessor<TObject>
{
    private readonly CacheConfiguration _cacheConfiguration;
    private readonly IDistributedCache _cache;

    public CacheAccessor(
        CacheConfiguration cacheConfiguration,
        IDistributedCache cache)
    {
        _cacheConfiguration = cacheConfiguration;
        _cache = cache;
    }

    public async Task SetAsync(CacheKey key, TObject? instance)
    {
        if (instance is null)
        {
            return;
        }

        var serialized = JsonSerializer.Serialize(instance);
        await _cache.SetStringAsync(
            key.Value,
            serialized,
            new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(_cacheConfiguration.CacheDuration));
    }

    public async Task<TObject?> GetAsync(CacheKey key)
    {
        var cacheValue = await _cache.GetStringAsync(key.Value);

        if (cacheValue is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<TObject>(cacheValue);
    }

    public Task RemoveAsync(CacheKey key) =>
        _cache.RemoveAsync(key.Value);
}
