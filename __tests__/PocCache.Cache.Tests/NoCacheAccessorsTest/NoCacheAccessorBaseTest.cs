using Microsoft.Extensions.Logging;
using PocCache.Cache.CacheAccessors;

namespace PocCache.Cache.Tests.NoCacheAccessorsTest;

public class NoCacheAccessorBaseTest : IDisposable
{
    private const string WillNotSet =
        "The cache key {0} will not be store on cache, because caching is disabled.";

    private const string WillNotGet =
        "The cache key {0} will not retrieved from cache, because caching is disabled.";

    private const string WillNotRemove =
        "The cache key {0} will not be remove from cache, because caching is disabled.";

    protected Mock<ILogger<Guid>> Logger { get; } = new(MockBehavior.Loose);
    protected CacheEntryConfiguration CacheEntryConfiguration { get; } =
        new CacheEntryConfiguration()
        {
            CacheDurationInMinutes = 5,
        };

    public void Dispose()
    {
        Mock.VerifyAll(Logger);
    }

    [Fact]
    public async Task Should_LogDebug_WhenCallSetAsync()
    {
        // Given
        var key = BuildCacheKey();
        var accessor = BuildNoCacheAccessor();

        // When
        await accessor.SetAsync(key, Guid.NewGuid());

        // Then
        Logger.Verify(logger =>
            logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Debug),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == string.Format(WillNotSet, key.Value)
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_LogDebug_WhenCallGetAsync()
    {
        // Given
        var key = BuildCacheKey();
        var accessor = BuildNoCacheAccessor();

        // When
        await accessor.GetAsync(key);

        // Then
        Logger.Verify(logger =>
            logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Debug),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == string.Format(WillNotGet, key.Value)
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_LogDebug_WhenCallRemoveAsync()
    {
        // Given
        var key = BuildCacheKey();
        var accessor = BuildNoCacheAccessor();

        // When
        await accessor.RemoveAsync(key);

        // Then
        Logger.Verify(logger =>
            logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Debug),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == string.Format(WillNotRemove, key.Value)
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    internal ICacheAccessor<Guid> BuildNoCacheAccessor() =>
        new NoCacheAccessor<Guid>(Logger.Object);

    internal CacheKey<Guid> BuildCacheKey() => new(
        CacheEntryConfiguration,
        Guid.NewGuid().ToString());
}
