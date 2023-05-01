using Microsoft.Extensions.DependencyInjection;

namespace PocCache.Cache.Extensions;

public static class CacheServiceExtension
{
    public static IServiceCollection AddObjectCache(this IServiceCollection services) =>
        services
            .AddTransient<IObjectCache, ObjectCache>();
}
