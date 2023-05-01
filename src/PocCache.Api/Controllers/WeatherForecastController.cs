using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace PocCache.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private const string Key = "09578ca0-949c-4e1b-98a2-2eb31a2c0592";

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IDistributedCache _distributedCache;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> GetAsync()
    {
        IEnumerable<WeatherForecast> result;
        var cache = await _distributedCache.GetStringAsync(Key);
        await _distributedCache.RefreshAsync(Key);
        if (cache is not null)
        {
            _logger.LogInformation("Reading from cache.");
            result = JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(cache)!;
        }
        else
        {
            _logger.LogInformation("Reading from infra.");
            result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            var serializedResult = JsonSerializer.Serialize(result);

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(60));

            await _distributedCache.SetStringAsync(Key, serializedResult, options);
        }

        return result;
    }
}
