using Microsoft.AspNetCore.Mvc;
using PocCache.Domain;

namespace PocCache.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecasts _weatherForecasts;

    public WeatherForecastController(IWeatherForecasts weatherForecasts)
    {
        _weatherForecasts = weatherForecasts;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> GetAsync()
    {
        return await _weatherForecasts.GetWeathers();
    }
}
