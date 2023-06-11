namespace PocCache.Cache;

public interface IObjectCache<TObject>
{
    Task<TObject?> GetAsync(string key, Func<Task<TObject?>> getFromOrigin);

    Task RemoveAsync(string key);

    void SetCacheOptions(CacheEntryConfiguration cacheConfiguration);
}
