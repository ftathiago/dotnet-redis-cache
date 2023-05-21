namespace PocCache.Cache;

public interface IObjectCache<TObject>
{
    Task<TObject?> GetAsync(string key, Func<Task<TObject?>> getData);

    Task RemoveAsync(string key);

    void SetCacheOptions(CacheEntryConfiguration cacheConfiguration);
}
