using PocCache.Domain;

namespace PocCache.InfraWeather.CacheComponents;

internal interface IWeatherForecastCache
{
    public Task<IEnumerable<WeatherForecast>?> GetWeathers(
        string key,
        Func<Task<IEnumerable<WeatherForecast>?>> getFromOrigin);
}
