using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

namespace PocCache.Cache.Tests.CacheAccessorsTest;

public class GetAsyncTest : CacheAccessorBaseTest<Guid>
{
    public GetAsyncTest(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task Should_AnObject_When_EntryIsLocatedAsync()
    {
        // Given
        var cacheKey = BuildCacheKey();
        var expectedObj = Guid.NewGuid();
        var serializedObj = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(expectedObj));
        DistributedCache
            .Setup(dc => dc.GetAsync(
                cacheKey.Value,
                It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(serializedObj));
        var accessor = BuildCacheAccessor();

        // When
        var result = await accessor.GetAsync(cacheKey);

        // Then
        result.Should().Be(expectedObj);
    }

    [Fact]
    public async Task Should_ReturnNull_When_EntryIsNotLocatedAsync()
    {
        // Given
        var cacheKey = BuildCacheKey();
        var serializedObj = Array.Empty<byte>();
        DistributedCache
            .Setup(dc => dc.GetAsync(
                cacheKey.Value,
                It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(serializedObj));
        var accessor = BuildCacheAccessor();

        // When
        var result = await accessor.GetAsync(cacheKey);

        // Then
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_LogError_When_CacheThrowsExceptionAsync()
    {
        // Given
        var cacheKey = BuildCacheKey();
        var exception = BuildRedisConnectionException();
        DistributedCache
            .Setup(dc => dc.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
        var accessor = BuildCacheAccessor();

        // When
        Func<Task> act = () => accessor.GetAsync(cacheKey);

        // Then
        await act.Should().NotThrowAsync();
        Logger.Verify(logger =>
            logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Error while retrieving data from cache. This could make requests slowly." && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
