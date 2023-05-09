namespace PocCache.Cache;

public record CacheConfiguration
{
    public bool Active { get; set; } = true;
    public int CacheDurationInMinutes { get; set; } = 5;

    public string? KeyPrefix { get; set; }

    internal TimeSpan CacheDuration =>
        TimeSpan.FromMinutes(CacheDurationInMinutes);

}
