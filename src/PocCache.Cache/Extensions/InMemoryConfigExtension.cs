using Microsoft.Extensions.DependencyInjection;

namespace PocCache.Cache.Extensions;

internal static class InMemoryConfigExtension
{
    public static IServiceCollection ConfigureInMemory(
        this IServiceCollection services)
    {
        var cacheMonitor = new CacheMonitor();
        return services
            .AddSingleton(cacheMonitor)
            .AddDistributedMemoryCache();
    }
}
