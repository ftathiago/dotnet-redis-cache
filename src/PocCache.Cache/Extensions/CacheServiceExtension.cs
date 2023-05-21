using Microsoft.Extensions.DependencyInjection;

namespace PocCache.Cache.Extensions;

public static class CacheServiceExtension
{
    public static IServiceCollection AddObjectCache(this IServiceCollection services) =>
        services
            .AddTransient(typeof(IObjectCache<>), typeof(ObjectCacheFacade<>));

    public static IServiceCollection AddDistributedCache(
        this IServiceCollection services,
        Action<CacheConfig> setupCache)
    {
        var config = new CacheConfig();

        setupCache(config);

        if (config.CacheType == CacheType.InMemory)
        {
            return services.AddDistributedMemoryCache();
        }

        return services.ConfigureRedis(config);
    }
}
