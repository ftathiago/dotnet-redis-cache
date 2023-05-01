using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace PocCache.Cache;

internal class ObjectCache : IObjectCache
{
    private readonly IDistributedCache _distributedCache;
    private CacheConfiguration _cacheConfiguration = new();

    public ObjectCache(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<TObject> GetAsync<TObject>(string key, Func<Task<TObject>> getFromOrigin)
    {
        var cacheKey = BuildKey(key);

        var cache = await _distributedCache.GetStringAsync(cacheKey);
        if (cache is not null)
        {
            return JsonSerializer.Deserialize<TObject>(cache)!;
        }

        var cachedObject = await getFromOrigin();

        return await SetValueAsync(cacheKey, cachedObject);
    }

    public void SetCacheOptions(CacheConfiguration cacheConfiguration)
    {
        _cacheConfiguration = cacheConfiguration;
    }

    private string BuildKey(string baseKey)
    {
        if (_cacheConfiguration.KeyPrefix is null)
        {
            return baseKey;
        }

        return $"{_cacheConfiguration.KeyPrefix}-{baseKey}";
    }

    private async Task<TObject> SetValueAsync<TObject>(string cacheKey, TObject cachedObject)
    {
        var serializedResult = JsonSerializer.Serialize(cachedObject);
        await _distributedCache.SetStringAsync(
            cacheKey,
            serializedResult,
            new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(_cacheConfiguration.CacheDuration));

        return cachedObject;
    }
}
