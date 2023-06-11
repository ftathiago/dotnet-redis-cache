namespace PocCache.Cache;

internal class CacheMonitor
{
    private readonly object _lock = new();

    private volatile bool _active = true;

    public bool Active => _active;

    public void UpdateCache(bool active)
    {
        lock (_lock)
        {
            _active = active;
        }
    }
}
