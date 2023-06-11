using PocCache.Domain;
using PocCache.InfraWeather.CacheComponents;

namespace PocCache.InfraWeather;

internal class WeatherForecastRepository : IWeatherForecasts
{
    private const string Key = "09578ca0-949c-4e1b-98a2-2eb31a2c0592";

    private static readonly string[] _summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
    };

    private readonly IWeatherForecastCache _cache;

    public WeatherForecastRepository(IWeatherForecastCache cache) =>
        _cache = cache;

    public async Task<IEnumerable<WeatherForecast>> GetWeathers()
    {
        var cache = await _cache.GetWeathers(Key, async () =>
            {
                var retorno = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = _summaries[Random.Shared.Next(_summaries.Length)],
                })
                .ToList();

                await Task.Delay(TimeSpan.FromSeconds(2));

                return retorno.AsEnumerable();
            }) ?? Array.Empty<WeatherForecast>();

        return cache;
    }
}
