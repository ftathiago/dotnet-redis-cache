using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using PocCache.Cache.CacheAccessors;

namespace PocCache.Cache;

internal class ObjectCacheFacade<TObject> : IObjectCache<TObject>
{
    private readonly ILogger<TObject> _logger;
    private readonly IDistributedCache _distributedCache;

    private ICacheAccessor<TObject> _cacheAccessor;
    private CacheConfiguration _cacheConfiguration = new();

    public ObjectCacheFacade(
        ILogger<TObject> logger,
        IDistributedCache distributedCache)
    {
        _logger = logger;
        _cacheAccessor = new NoCacheAccessor<TObject>(logger);
        _distributedCache = distributedCache;
    }

    public async Task<TObject?> GetAsync(string key, Func<Task<TObject?>> getFromOrigin)
    {
        var cacheKey = BuildCacheKey(key);

        var cachedObject = await _cacheAccessor.GetAsync(cacheKey);
        if (cachedObject is null)
        {
            cachedObject = await getFromOrigin();
            await _cacheAccessor.SetAsync(cacheKey, cachedObject);
        }

        return cachedObject;
    }

    public async Task RemoveAsync(string key)
    {
        var cacheKey = BuildCacheKey(key);
        await _cacheAccessor.RemoveAsync(cacheKey);
    }

    public void SetCacheOptions(CacheConfiguration cacheConfiguration)
    {
        _cacheConfiguration = cacheConfiguration;
        if (cacheConfiguration.Active)
        {
            _cacheAccessor = new CacheAccessor<TObject>(
                cacheConfiguration,
                _distributedCache);
            return;
        }

        _cacheAccessor = new NoCacheAccessor<TObject>(_logger);
    }

    private CacheKey BuildCacheKey(string baseKey) =>
        new(_cacheConfiguration, baseKey);
}
