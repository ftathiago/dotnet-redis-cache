using Microsoft.Extensions.DependencyInjection;

namespace PocCache.Cache.Extensions;

public static class CacheServiceExtension
{
    public static IServiceCollection AddDistributedCache(
        this IServiceCollection services,
        Action<CacheConfig> setupCache)
    {
        var config = new CacheConfig();

        setupCache(config);

        services
            .AddObjectCache();

        if (config.CacheType == CacheType.InMemory)
        {
            return services
                .ConfigureInMemory();
        }

        return services
            .ConfigureRedis(opt => opt.Configuration = config.ConnectionString);
    }

    private static IServiceCollection AddObjectCache(this IServiceCollection services) =>
        services
            .AddTransient(typeof(IObjectCache<>), typeof(ObjectCacheFacade<>));
}
