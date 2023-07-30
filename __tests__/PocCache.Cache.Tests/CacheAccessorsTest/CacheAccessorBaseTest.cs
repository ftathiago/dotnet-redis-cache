using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using PocCache.Cache.CacheAccessors;
using StackExchange.Redis;
using Xunit.Abstractions;

namespace PocCache.Cache.Tests.CacheAccessorsTest;

public abstract class CacheAccessorBaseTest<TObject> : IDisposable
{
    protected readonly ITestOutputHelper _output;

    public CacheAccessorBaseTest(ITestOutputHelper output)
    {
        _output = output;
    }

    protected Mock<ILogger<TObject>> Logger { get; } = new(MockBehavior.Loose);

    protected CacheEntryConfiguration CacheEntryConfiguration { get; } =
        new CacheEntryConfiguration()
        {
            CacheDurationInMinutes = 5,
        };

    protected Mock<IDistributedCache> DistributedCache { get; } =
        new(MockBehavior.Strict);

    public void Dispose() =>
        Mock.VerifyAll(Logger, DistributedCache);

    internal ICacheAccessor<TObject> BuildCacheAccessor() =>
        new CacheAccessor<TObject>(
            Logger.Object,
            CacheEntryConfiguration,
            DistributedCache.Object);

    internal CacheKey<TObject> BuildCacheKey() => new(
        CacheEntryConfiguration,
        Guid.NewGuid().ToString());

    internal RedisConnectionException BuildRedisConnectionException() => new(
        Fixture.GetFaker().Random.Enum<ConnectionFailureType>(),
        Fixture.GetFaker().Lorem.Word());
}
