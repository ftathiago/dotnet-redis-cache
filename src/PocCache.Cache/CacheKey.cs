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
            .Append('.');

        if (type.IsGenericType)
        {
            return FormatGenericTypes(sb, type);
        }

        return sb.Append(type.Name);
    }

    // When is a generic type, the `type.Name` returns something like
    // "IEnumerable`1". The code bellow remove, turning a more friendly text.
    private StringBuilder FormatGenericTypes(StringBuilder sb, Type type)
    {
        var name = type.Name[..type.Name.LastIndexOf('`')];

        sb
            .Append(name)
            .Append('<');
        foreach (var arg in type.GenericTypeArguments)
        {
            GetTypeName(arg, sb)
                .Append(", ");
        }

        sb
            .Remove(sb.Length - 2, 2)
            .Append('>');

        return sb;
    }
}
