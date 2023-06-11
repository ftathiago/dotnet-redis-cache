namespace PocCache.Cache;

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
