namespace PocCache.Cache;

public class CacheConfig
{
    public CacheType CacheType { get; set; } = CacheType.InMemory;

    public string? ConnectionString { get; set; }
}
