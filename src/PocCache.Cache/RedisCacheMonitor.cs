namespace PocCache.Cache;

internal class RedisCacheMonitor
{
    private volatile bool _active = true;

    private readonly object _lock = new();

    public void UpdateCache(bool active)
    {
        lock (_lock)
        {
            _active = active;
        }
    }

    public bool Active => _active;
}
