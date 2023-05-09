using PocCache.Cache;
using PocCache.Domain;

namespace PocCache.InfraWeather.CacheComponents;

public class WeatherForecastCache : IWeatherForecastCache
{
    private readonly IObjectCache<IEnumerable<WeatherForecast>> _objectCache;

    public WeatherForecastCache(
        IObjectCache<IEnumerable<WeatherForecast>> objectCache,
        WeatherCacheConfig weatherCacheConfig)
    {
        _objectCache = objectCache;
        _objectCache.SetCacheOptions(weatherCacheConfig);
    }

    public Task<IEnumerable<WeatherForecast>?> GetWeathers(
        string key,
        Func<Task<IEnumerable<WeatherForecast>?>> getFromOrigin)
    {
        return _objectCache.GetAsync(key, getFromOrigin);
    }
}
