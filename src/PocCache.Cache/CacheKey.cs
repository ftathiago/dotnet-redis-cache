using System.Text;

namespace PocCache.Cache;

internal readonly struct CacheKey<TObject>
{
    private readonly CacheEntryConfiguration _cacheConfiguration;
    private readonly string _key;

    public CacheKey(
        CacheEntryConfiguration cacheConfiguration,
        string key)
    {
        _cacheConfiguration = cacheConfiguration;
        _key = key;
    }

    public string Value => string.IsNullOrEmpty(_cacheConfiguration.KeyPrefix)
        ? $"{GetTypeName(typeof(TObject))}:{_key}"
        : $"{_cacheConfiguration.KeyPrefix}:{GetTypeName(typeof(TObject))}:{_key}";

    private StringBuilder GetTypeName(Type type, StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();

        sb
            .Append(type.Namespace)
            .Append('.')
            .Append(type.Name);

        if (type.IsGenericType)
        {
            sb
                .Remove(sb.Length - 2, 2)
                .Append('<');
            foreach (var arg in type.GenericTypeArguments)
            {
                GetTypeName(arg, sb)
                    .Append(", ");
            }
            sb
                .Remove(sb.Length - 2, 2)
                .Append('>');
        }

        return sb;
    }
}
