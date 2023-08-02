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
        var provider = services.BuildServiceProvider();
        return services
            .AddSingleton(cacheMonitor)
            .Configure(setupAction)
            .AddSingleton(p => Connection(p.GetRequiredService<IOptions<RedisCacheOptions>>().Value))
            .AddStackExchangeRedisCache(ReuseConnection(provider, setupAction));
    }

    /// <summary>
    /// Get connection from given options
    /// </summary>
    /// <param name="options">Cache options</param>
    /// <returns>Connection multiplexer</returns>
    /// <exception cref="ArgumentNullException">Throws if no connection options available</exception>
    private static IConnectionMultiplexer Connection(RedisCacheOptions options)
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

        connection.ConnectionFailed += (sender, args) => Console.WriteLine("Conexão Falhou");
        connection.ConnectionRestored += (sender, args) => Console.WriteLine("Conexão Voltou");

        return connection;
    }

    /// <summary>
    /// The trick to reuse existing connection in our and RedisCache implementations
    /// </summary>
    /// <param name="serviceProvider">Provider</param>
    /// <param name="setupAction">Setup Action</param>
    private static Action<RedisCacheOptions> ReuseConnection(IServiceProvider serviceProvider, Action<RedisCacheOptions> setupAction) => options =>
    {
        setupAction(options);
        options.Configuration = null;
        options.ConfigurationOptions = null;
        options.ConnectionMultiplexerFactory = () => Task.FromResult(serviceProvider.GetRequiredService<IConnectionMultiplexer>());
    };
}
