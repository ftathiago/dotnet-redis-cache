namespace PocCache.Cache;

public class CacheEntryConfiguration
{
    public bool Active { get; set; } = true;

    public int CacheDurationInMinutes { get; set; } = 5;

    public int CacheSlidingDurationInMinutes { get; set; } = int.MaxValue;

    public string? KeyPrefix { get; set; }

    internal TimeSpan CacheDuration =>
        TimeSpan.FromMinutes(CacheDurationInMinutes);

    internal TimeSpan CacheSlidingDuration =>
        TimeSpan.FromMinutes(CacheSlidingDurationInMinutes);
}
