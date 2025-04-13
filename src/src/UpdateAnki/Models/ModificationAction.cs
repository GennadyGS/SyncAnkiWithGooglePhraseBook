namespace UpdateAnki.Models;

internal abstract record ModificationAction<TKey, TValue>
{
    public sealed record Add(IEnumerable<TValue> Values) : ModificationAction<TKey, TValue>;

    public sealed record Delete(IEnumerable<TKey> Keys) : ModificationAction<TKey, TValue>;

    public sealed record Update(IEnumerable<KeyValuePair<TKey, TValue>> Updates)
        : ModificationAction<TKey, TValue>;
}
