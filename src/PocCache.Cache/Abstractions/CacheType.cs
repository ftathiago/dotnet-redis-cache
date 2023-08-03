namespace PocCache.Cache.Abstractions;

public enum CacheType
{
    /// <summary>
    /// The cache data will be stored at container memory.
    /// </summary>
    InMemory,

    /// <summary>
    /// The cache data will be stored into redis instance.
    /// </summary>
    Redis,
}
