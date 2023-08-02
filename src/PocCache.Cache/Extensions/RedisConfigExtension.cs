using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace PocCache.Cache.Extensions;

internal static class RedisConfigExtension
{
    public static IServiceCollection ConfigureRedis(
        this IServiceCollection services,
        Action<RedisCacheOptions> setupAction)
    {
        var cacheMonitor = new CacheMonitor();

        return services
            .AddSingleton(cacheMonitor)
            .Configure(setupAction)
            .AddSingleton(p => Connection(p.GetRequiredService<IOptions<RedisCacheOptions>>().Value, cacheMonitor))
            .AddSingleton(provider => new ServiceCollection()
                .AddStackExchangeRedisCache(ReuseConnection(provider, setupAction))
                .BuildServiceProvider()
                .GetRequiredService<IDistributedCache>());
    }

    /// <summary>
    /// Get connection from given options
    /// </summary>
    /// <param name="options">Cache options</param>
    /// <returns>Connection multiplexer</returns>
    /// <exception cref="ArgumentNullException">Throws if no connection options available</exception>
    private static IConnectionMultiplexer Connection(RedisCacheOptions options, CacheMonitor cacheMonitor)
    {
        IConnectionMultiplexer? connection = null;
        if (options.ConnectionMultiplexerFactory != null)
        {
            connection = options.ConnectionMultiplexerFactory().GetAwaiter().GetResult();
        }
        else if (options.ConfigurationOptions != null)
        {
            connection = ConnectionMultiplexer.Connect(options.ConfigurationOptions);
        }
        else if (options.Configuration != null)
        {
            connection = ConnectionMultiplexer.Connect(options.Configuration);
        }

        if (connection is null)
        {
            throw new ArgumentNullException(nameof(options), "All connection options are null");
        }

        connection.ConnectionFailed += (sender, args) => cacheMonitor.UpdateCache(false);
        connection.ConnectionRestored += (sender, args) => cacheMonitor.UpdateCache(true);

        return connection;
    }

    /// <summary>
    /// The trick to reuse existing connection in our and RedisCache implementations
    /// </summary>
    /// <param name="provider">Provider</param>
    /// <param name="setupAction">Setup Action</param>
    private static Action<RedisCacheOptions> ReuseConnection(IServiceProvider provider, Action<RedisCacheOptions> setupAction) => options =>
    {
        setupAction(options);
        options.Configuration = null;
        options.ConfigurationOptions = null;
        options.ConnectionMultiplexerFactory = () => Task.FromResult(provider.GetRequiredService<IConnectionMultiplexer>());
    };
}
