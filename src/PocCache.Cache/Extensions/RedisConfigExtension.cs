using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace PocCache.Cache.Extensions;

internal static class RedisConfigExtension
{
    /// <summary>
    /// Extension method to configure Redis as Cache database provider.
    /// Also, this setup a monitor for Redis status, turning on/off
    /// the cache functionality based on Redis Status.
    /// </summary>
    /// <param name="services">Instance of DependencyInjection container</param>
    /// <param name="setupAction">Delegate method for RedisCacheOptions configuration.</param>
    /// <returns>The same services instance.</returns>
    public static IServiceCollection ConfigureRedis(
        this IServiceCollection services,
        Action<RedisCacheOptions> setupAction)
    {
        var cacheMonitor = new CacheMonitor();

        return services
            .AddSingleton(cacheMonitor)
            .Configure(setupAction)

            // Turns Connection destructive by IServiceCollection, avoiding memory leak and
            // connection "keeping open" after application shutdown.
            .AddSingleton(provider => BuildRedisConnection(provider.GetRequiredService<IOptions<RedisCacheOptions>>().Value, cacheMonitor))

            // This is need in this way, because otherwise, we will have a circular reference
            // about connection configuration. Can be removed, maybe, if reading original source,
            // we create this objects tree manually. But, we want to maintain any lib change?
            .AddSingleton(provider => new ServiceCollection()
                .AddStackExchangeRedisCache(ReuseConnection(provider, setupAction))
                .BuildServiceProvider()
                .GetRequiredService<IDistributedCache>());
    }

    /// <summary>
    /// Get connection from given options.
    /// </summary>
    /// <param name="options">Cache options and configurations.</param>
    /// <param name="cacheMonitor">Monitoring Redis connection, toggling cache on/off.</param>
    /// <returns>Connection multiplexer</returns>
    /// <exception cref="ArgumentNullException">Throws if no connection options available</exception>
    private static IConnectionMultiplexer BuildRedisConnection(RedisCacheOptions options, CacheMonitor cacheMonitor)
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
    /// This method changes RedisCacheOptions to take (and return) connections from DependencyInjection
    /// container.
    /// </summary>
    /// <param name="provider">Service provider to provide a singleton instance of IConnectionMultiplexer</param>
    /// <param name="setupAction">Action thats setup the RedisCacheOptions.</param>
    private static Action<RedisCacheOptions> ReuseConnection(IServiceProvider provider, Action<RedisCacheOptions> setupAction) => options =>
    {
        setupAction(options);
        options.Configuration = null;
        options.ConfigurationOptions = null;
        options.ConnectionMultiplexerFactory = () => Task.FromResult(provider.GetRequiredService<IConnectionMultiplexer>());
    };
}
