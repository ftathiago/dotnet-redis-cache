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

        var redisCacheMonitor = new RedisCacheMonitor();
        var multiplexer = BuildMultiplexer(redisCacheMonitor, redisConfig);

        return services
            .AddSingleton(redisCacheMonitor)
            .AddSingleton(multiplexer)
            .AddStackExchangeRedisCache(options =>
                options.ConfigurationOptions = redisConfig);
    }

    private static IConnectionMultiplexer BuildMultiplexer(
        RedisCacheMonitor redisCacheMonitor,
        ConfigurationOptions redisConfig)
    {
        var multiplexer = ConnectionMultiplexer.Connect(redisConfig) as IConnectionMultiplexer;
        multiplexer.ConnectionRestored += (sender, args) =>
        {
            redisCacheMonitor.UpdateCache(true);
        };
        multiplexer.ConnectionFailed += (sender, args) =>
        {
            redisCacheMonitor.UpdateCache(false);
        };

        return multiplexer;
    }
}
