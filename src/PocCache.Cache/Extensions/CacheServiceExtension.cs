using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace PocCache.Cache.Extensions;

public static class CacheServiceExtension
{
    public static IServiceCollection AddDistributedCache(this IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();
        var configuration = provider.GetRequiredService<IConfiguration>();

        var connectionString = configuration.GetConnectionString("RedisCache");
        var config = ConfigurationOptions.Parse(connectionString, ignoreUnknown: false);

        return services.AddStackExchangeRedisCache(options => options.ConfigurationOptions = config);
    }

    public static IServiceCollection AddObjectCache(this IServiceCollection services) =>
        services
            .AddTransient(typeof(IObjectCache<>), typeof(ObjectCacheFacade<>));
}
