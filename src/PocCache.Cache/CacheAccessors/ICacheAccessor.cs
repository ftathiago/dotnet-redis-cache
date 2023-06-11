namespace PocCache.Cache.CacheAccessors;

internal interface ICacheAccessor<TObject>
{
    Task<TObject?> GetAsync(CacheKey<TObject> key);

    Task SetAsync(CacheKey<TObject> key, TObject? instance);

    Task RemoveAsync(CacheKey<TObject> key);
}
