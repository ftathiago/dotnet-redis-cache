namespace PocCache.Cache;

/// <summary>
/// You must to implement this class to define what cache configuration will be
/// used for each stored object.
/// </summary>
/// <typeparam name="TCachedObject">The object to be cached.</typeparam>
/// <typeparam name="TCacheConfig">The object with cache configurations.</typeparam>
public abstract class BaseCacheComponent<TCachedObject, TCacheConfig>
    where TCacheConfig : CacheEntryConfiguration
{
    protected BaseCacheComponent(
        IObjectCache<TCachedObject> objectCache,
        TCacheConfig cacheConfig)
    {
        ObjectCache = objectCache;
        CacheConfig = cacheConfig;
        ObjectCache.SetCacheOptions(cacheConfig);
    }

    protected IObjectCache<TCachedObject> ObjectCache { get; }

    protected TCacheConfig CacheConfig { get; }
}
