using PocCache.Domain;
using PocCache.InfraWeather.CacheComponents;

namespace PocCache.InfraWeather;

public class CityRepository : ICities
{
    private const string Key = "e0a4ef9d-8cde-46e6-93a2-3c27ba3e88fc";
    private readonly ICitiesCache _citiesCache;

    public CityRepository(ICitiesCache citiesCache) =>
        _citiesCache = citiesCache;

    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        var cities = await _citiesCache.GetCities(Key, async () =>
            {
                var lista = new List<City>()
                {
                    new City() { Name = "Franca" },
                    new City() { Name = "Ribeirão Preto" },
                    new City() { Name = "Maringá" },
                    new City() { Name = "Londrina" },
                    new City() { Name = "São Paulo" },
                };

                await Task.Delay(TimeSpan.FromSeconds(2));

                return lista.AsEnumerable();
            }) ?? Enumerable.Empty<City>();

        return cities;
    }
}
