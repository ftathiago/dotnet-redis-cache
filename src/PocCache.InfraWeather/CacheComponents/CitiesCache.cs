using PocCache.Cache;
using PocCache.Domain;

namespace PocCache.InfraWeather.CacheComponents;

internal class CitiesCache : ICitiesCache
{
    private readonly IObjectCache _objectCache;

    public CitiesCache(
        IObjectCache objectCache,
        CitiesCacheConfig citiesCacheConfig)
    {
        _objectCache = objectCache;
        _objectCache.SetCacheOptions(citiesCacheConfig);
    }
    public Task<IEnumerable<City>> GetCities(
        string key,
        Func<Task<IEnumerable<City>>> getFromOrigin)
    {
        return _objectCache.GetAsync(key, getFromOrigin);
    }
}
