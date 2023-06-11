using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace PocCache.Cache.Extensions;

internal static class RedisConfigExtension
{
    public static IServiceCollection ConfigureRedis(
        this IServiceCollection services,
        CacheConfig config)
    {
        var redisConfig = ConfigurationOptions.Parse(
            config.ConnectionString!,
            ignoreUnknown: false);
        redisConfig.AbortOnConnectFail = false;

        var cacheMonitor = new CacheMonitor();
        var multiplexer = BuildMultiplexer(cacheMonitor, redisConfig);

        return services
            .AddSingleton(cacheMonitor)
            .AddSingleton(multiplexer)
            .AddStackExchangeRedisCache(options =>
                options.ConfigurationOptions = redisConfig);
    }

    private static IConnectionMultiplexer BuildMultiplexer(
        CacheMonitor redisCacheMonitor,
        ConfigurationOptions redisConfig)
    {
        var multiplexer = ConnectionMultiplexer.Connect(redisConfig) as IConnectionMultiplexer;
        multiplexer.ConnectionRestored += (sender, args) =>
            redisCacheMonitor.UpdateCache(true);
        multiplexer.ConnectionFailed += (sender, args) =>
            redisCacheMonitor.UpdateCache(false);

        return multiplexer;
    }
}
