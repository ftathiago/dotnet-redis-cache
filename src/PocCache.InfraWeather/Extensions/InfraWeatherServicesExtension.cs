using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PocCache.Cache.Extensions;
using PocCache.Domain;
using PocCache.InfraWeather.CacheComponents;

namespace PocCache.InfraWeather.Extensions;

public static class InfraWeatherServicesExtension
{
    public static IServiceCollection AddInfraWeather(this IServiceCollection services) =>
        services
            .AddObjectCache()
            .AddScoped<IWeatherForecasts, WeatherForecastRepository>()
            .AddScoped<ICities, CityRepository>()
            .AddTransient<IWeatherForecastCache, WeatherForecastCache>()
            .AddTransient<ICitiesCache, CitiesCache>()
            .AddSingleton(provider =>
            {
                return provider.GetRequiredService<IConfiguration>()
                    .GetSection(nameof(WeatherCacheConfig)).Get<WeatherCacheConfig>();
            })
            .AddSingleton(provider =>
            {
                return provider.GetRequiredService<IConfiguration>()
                    .GetSection(nameof(CitiesCacheConfig)).Get<CitiesCacheConfig>();
            });
}
