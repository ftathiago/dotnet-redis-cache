using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PocCache.Cache.Extensions;
using PocCache.Domain;
using PocCache.InfraWeather.CacheComponents;

namespace PocCache.InfraWeather.Extensions;

public static class InfraWeatherServicesExtension
{
    public static IServiceCollection AddInfraWeather(this IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();
        return services
            .AddScoped<IWeatherForecasts, WeatherForecastRepository>()
            .AddScoped<ICities, CityRepository>()
            .AddTransient<IWeatherForecastCache, WeatherForecastCache>()
            .AddTransient<ICitiesCache, CitiesCache>()
            .AddSingleton(provider => provider
                .GetRequiredService<IConfiguration>()
                .GetSection(nameof(WeatherCacheConfig))
                .Get<WeatherCacheConfig>())
            .AddSingleton(provider => provider
                .GetRequiredService<IConfiguration>()
                .GetSection(nameof(CitiesCacheConfig))
                .Get<CitiesCacheConfig>())
            .AddObjectCache()
            .AddDistributedCache(config =>
                provider.GetRequiredService<IConfiguration>()
                    .GetSection("CacheConfig")
                    .Bind(config));
    }
}
