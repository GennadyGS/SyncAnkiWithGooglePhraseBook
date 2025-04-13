namespace UpdateAnki.Models;

internal abstract record UpdateAction<TKey, TValue>
{
    public sealed record Add(IEnumerable<TValue> Values) : UpdateAction<TKey, TValue>;

    public sealed record Delete(IEnumerable<TKey> Keys) : UpdateAction<TKey, TValue>;

    public sealed record Update(IEnumerable<KeyValuePair<TKey, TValue>> Updates)
        : UpdateAction<TKey, TValue>;
}
