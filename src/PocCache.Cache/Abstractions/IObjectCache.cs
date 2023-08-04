namespace PocCache.Cache.Abstractions;

/// <summary>
/// Manipulate typed objects from cache
/// </summary>
/// <typeparam name="TObject">The object/type that must be stored on/recovered from cache.</typeparam>
public interface IObjectCache<TObject>
{
    /// <summary>
    /// Recover an object instance based on his key (must be translated to string).
    /// When the specific key was not found on cache, the getFromOrigin will be executed
    /// and his return - if not null - will be stored.
    /// </summary>
    /// <param name="key">A unique key that identifies this instance.</param>
    /// <param name="getFromOrigin">A method that returns the desired instance, if it is not already cached.</param>
    /// <returns>The requested instance.</returns>
    Task<TObject?> GetAsync(string key, Func<Task<TObject?>> getFromOrigin);

    /// <summary>
    /// Removes the specified instance from cache. If the specified instance does not exist,
    /// nothing will happen.
    /// </summary>
    /// <param name="key">A unique key that identifies this instance.</param>
    /// <returns>Task</returns>
    Task RemoveAsync(string key);

    /// <summary>
    /// Define the Cache configuration for this specific TObject.
    /// This data will be used for configuring parameters like Key prefix, Cache and sliding duration
    /// and etc.
    /// </summary>
    /// <param name="cacheConfiguration">A configurational instance.</param>
    void SetCacheOptions(CacheEntryConfiguration cacheConfiguration);
}
