namespace PocCache.Domain;

public interface IWeatherForecasts
{
    Task<IEnumerable<WeatherForecast>> GetWeathers();
}
