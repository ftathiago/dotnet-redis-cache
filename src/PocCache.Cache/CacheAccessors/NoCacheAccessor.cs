using Microsoft.Extensions.Logging;

namespace PocCache.Cache.CacheAccessors;

internal class NoCacheAccessor<TObject> : ICacheAccessor<TObject>
{
    private const string WillNotSet =
        "The cache key {keyValue} will not be store at cache, because caching is disabled.";

    private const string WillNotGet =
        "The cache key {keyValue} will not retrieved from cache, because caching is disabled.";

    private const string WillNotRemove =
        "The cache key {keyValue} will not be remove from cache, because caching is disabled.";

    private readonly ILogger<TObject> _logger;

    public NoCacheAccessor(
        ILogger<TObject> logger)
    {
        _logger = logger;
    }

    public Task SetAsync(CacheKey key, TObject? instance)
    {
        _logger.LogDebug(WillNotSet, key.Value);
        return Task.CompletedTask;
    }

    public Task<TObject?> GetAsync(CacheKey key)
    {
        _logger.LogDebug(WillNotGet, key.Value);
        return Task.FromResult<TObject?>(default);
    }

    public Task RemoveAsync(CacheKey key)
    {
        _logger.LogDebug(WillNotRemove, key.Value);
        return Task.CompletedTask;
    }
}
