using Microsoft.Extensions.Logging;

namespace PocCache.Cache.Tests.CacheAccessorsTest;

public class RemoveAsyncTest : CacheAccessorBaseTest<Guid?>
{
    [Fact]
    public async Task Should_CallRemoveMethodFromCacheAsync()
    {
        // Given
        var cacheKey = BuildCacheKey();
        DistributedCache
            .Setup(dc => dc.RemoveAsync(cacheKey.Value, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var accessor = BuildCacheAccessor();

        // When
        await accessor.RemoveAsync(cacheKey);

        // Then
        DistributedCache
            .Verify(dc =>
                dc.RemoveAsync(
                    cacheKey.Value,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task Should_LogCallRemoveMethodFromCacheAsync()
    {
        // Given
        var cacheKey = BuildCacheKey();
        DistributedCache
            .Setup(dc => dc.RemoveAsync(cacheKey.Value, It.IsAny<CancellationToken>()))
            .Throws(BuildRedisConnectionException());
        var accessor = BuildCacheAccessor();

        // When
        Func<Task> act = () => accessor.RemoveAsync(cacheKey);

        // Then
        await act.Should().NotThrowAsync();
        Logger.Verify(logger =>
            logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Error while removing data from cache. This could make requests slowly." && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
