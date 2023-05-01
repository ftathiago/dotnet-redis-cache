using Microsoft.Extensions.DependencyInjection;
using PocCache.Cache.Extensions;
using PocCache.Domain;

namespace PocCache.InfraWeather.Extensions;

public static class InfraWeatherServicesExtension
{
    public static IServiceCollection AddInfraWeather(this IServiceCollection services) =>
        services
            .AddObjectCache()
            .AddScoped<IWeatherForecasts, WeatherForecastRepository>();
}
