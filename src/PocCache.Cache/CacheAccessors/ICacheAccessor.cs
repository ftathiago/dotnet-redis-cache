
namespace PocCache.Cache.CacheAccessors;

internal interface ICacheAccessor<TObject>
{
    Task<TObject?> GetAsync(CacheKey key);

    Task SetAsync(CacheKey key, TObject? instance);

    Task RemoveAsync(CacheKey key);
}
