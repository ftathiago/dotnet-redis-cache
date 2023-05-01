namespace PocCache.Cache;

public class CacheConfiguration
{
    public int CacheDurationInMinutes { get; set; } = 5;

    public string? KeyPrefix { get; set; }

    public TimeSpan CacheDuration =>
        TimeSpan.FromMinutes(CacheDurationInMinutes);

}
