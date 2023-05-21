using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using PocCache.Cache.CacheAccessors;

namespace PocCache.Cache;

internal class ObjectCacheFacade<TObject> : IObjectCache<TObject>
{
    private readonly ILogger<TObject> _logger;
    private readonly IDistributedCache _distributedCache;
    private readonly RedisCacheMonitor _redisCacheMonitor;
    private ICacheAccessor<TObject> _cacheAccessor;
    private CacheEntryConfiguration _cacheConfiguration = new();

    public ObjectCacheFacade(
        ILogger<TObject> logger,
        IDistributedCache distributedCache,
        RedisCacheMonitor redisCacheMonitor)
    {
        _logger = logger;
        _cacheAccessor = new NoCacheAccessor<TObject>(logger);
        _distributedCache = distributedCache;
        _redisCacheMonitor = redisCacheMonitor;
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

    public void SetCacheOptions(CacheEntryConfiguration cacheConfiguration)
    {
        _cacheConfiguration = cacheConfiguration;

        if (cacheConfiguration.Active && _redisCacheMonitor.Active)
        {
            _cacheAccessor = new CacheAccessor<TObject>(
                _logger,
                cacheConfiguration,
                _distributedCache);
            return;
        }

        _cacheAccessor = new NoCacheAccessor<TObject>(_logger);
    }

    private CacheKey<TObject> BuildCacheKey(string baseKey) =>
        new(_cacheConfiguration, baseKey);
}
