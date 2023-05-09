namespace PocCache.Cache;

internal readonly struct CacheKey
{
    private readonly CacheConfiguration _cacheConfiguration;
    private readonly string _key;

    public CacheKey(
        CacheConfiguration cacheConfiguration,
        string key)
    {
        _cacheConfiguration = cacheConfiguration;
        _key = key;
    }

    public string Value => string.IsNullOrEmpty(_cacheConfiguration.KeyPrefix)
        ? _key
        : $"{_cacheConfiguration.KeyPrefix}-{_key}";
}
