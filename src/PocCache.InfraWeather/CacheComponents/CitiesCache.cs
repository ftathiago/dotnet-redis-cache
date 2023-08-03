using PocCache.Cache;
using PocCache.Cache.Abstractions;
using PocCache.Domain;

namespace PocCache.InfraWeather.CacheComponents;

internal class CitiesCache :
    BaseCacheComponent<IEnumerable<City>, CitiesCacheConfig>,
    ICitiesCache
{
    public CitiesCache(
        IObjectCache<IEnumerable<City>> objectCache,
        CitiesCacheConfig citiesCacheConfig)
        : base(objectCache, citiesCacheConfig)
    {
    }

    public Task<IEnumerable<City>?> GetCities(
        string key,
        Func<Task<IEnumerable<City>?>> getFromOrigin)
    {
        return ObjectCache.GetAsync(key, getFromOrigin);
    }
}
