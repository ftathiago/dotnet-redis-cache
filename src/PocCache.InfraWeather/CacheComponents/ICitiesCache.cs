using PocCache.Domain;

namespace PocCache.InfraWeather.CacheComponents;

public interface ICitiesCache
{
    public Task<IEnumerable<City>?> GetCities(
        string key,
        Func<Task<IEnumerable<City>?>> getFromOrigin);
}
