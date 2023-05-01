namespace PocCache.Cache;

public interface IObjectCache
{
    Task<TObject> GetAsync<TObject>(string key, Func<Task<TObject>> getData);
}
