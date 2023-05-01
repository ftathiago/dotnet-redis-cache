namespace PocCache.Domain;

public interface ICities
{
    Task<IEnumerable<City>> GetCitiesAsync();
}
