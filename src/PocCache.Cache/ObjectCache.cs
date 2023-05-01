using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace PocCache.Cache;

internal class ObjectCache : IObjectCache
{
    private readonly IDistributedCache _distributedCache;

    public ObjectCache(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<TObject> GetAsync<TObject>(string key, Func<Task<TObject>> getFromOrigin)
    {
        TObject cachedObject;
        var cache = await _distributedCache.GetStringAsync(key);
        await _distributedCache.RefreshAsync(key);
        if (cache is not null)
        {
            cachedObject = JsonSerializer.Deserialize<TObject>(cache)!;
        }
        else
        {
            cachedObject = await getFromOrigin();

            var serializedResult = JsonSerializer.Serialize(cachedObject);

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(60));

            await _distributedCache.SetStringAsync(key, serializedResult, options);
        }

        return cachedObject;
    }
}
