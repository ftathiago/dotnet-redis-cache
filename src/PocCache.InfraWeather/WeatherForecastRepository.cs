using Microsoft.Extensions.Logging;
using PocCache.Cache;
using PocCache.Domain;

namespace PocCache.InfraWeather;

internal class WeatherForecastRepository : IWeatherForecasts
{
    private const string Key = "09578ca0-949c-4e1b-98a2-2eb31a2c0592";

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IObjectCache _objectCache;

    public WeatherForecastRepository(
        IObjectCache objectCache,
        ILogger<WeatherForecastRepository> logger)
    {
        _objectCache = objectCache;
    }

    public async Task<IEnumerable<WeatherForecast>> GetWeathers()
    {
        var cache = await _objectCache.GetAsync<IEnumerable<WeatherForecast>>(
            Key,
            () =>
            {
                var retorno = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                });

                return Task.FromResult(retorno);
            });

        return cache;
    }
}
