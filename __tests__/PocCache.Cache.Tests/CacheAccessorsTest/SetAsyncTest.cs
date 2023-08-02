using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace PocCache.Cache.Tests.CacheAccessorsTest;

public class SetAsyncTest : CacheAccessorBaseTest<Guid?>
{
    [Fact]
    public void Should_ReturnTaskCompleted_When_InstanceIsNull()
    {
        // Given
        var cacheKey = BuildCacheKey();
        var accessor = BuildCacheAccessor();

        // When
        var result = accessor.SetAsync(cacheKey, null);

        // Then
        result.Should().Be(Task.CompletedTask);
    }

    [Fact]
    public async Task Should_StoreValueOnCache_When_InstanceIsNotNullAsync()
    {
        // Given
        var cacheKey = BuildCacheKey();
        var instance = Guid.NewGuid();
        var serialized = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(instance));
        var accessor = BuildCacheAccessor();
        DistributedCache
            .Setup(dc => dc.SetAsync(
                cacheKey.Value,
                serialized,
                It.Is<DistributedCacheEntryOptions>(opt =>
                    opt.AbsoluteExpiration < DateTimeOffset.UtcNow.Add(CacheEntryConfiguration.CacheDuration)
                    && opt.SlidingExpiration == CacheEntryConfiguration.CacheSlidingDuration),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // When 
        await accessor.SetAsync(cacheKey, instance);

        // Then
    }

    [Fact]
    public async Task Should_NotThrowExceptionAndLog_When_CacheSetThrowsExceptionAsync()
    {
        var cacheKey = BuildCacheKey();
        var instance = Guid.NewGuid();
        var accessor = BuildCacheAccessor();
        var exception = BuildRedisConnectionException();
        DistributedCache
            .Setup(dc => dc.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Throws(exception);

        // When 
        Func<Task> act = () => accessor.SetAsync(cacheKey, instance);

        // Then
        await act.Should().NotThrowAsync<RedisConnectionException>();
    }
}
