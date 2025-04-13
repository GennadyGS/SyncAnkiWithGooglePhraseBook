namespace UpdateAnki.Models;

public sealed record EnumerableModificationActions<TKey, TValue>
{
    public IEnumerable<TValue> ToAdd { get; init; } = [];

    public IEnumerable<KeyValuePair<TKey, TValue>> ToUpdate { get; init; } = [];

    public IEnumerable<TKey> ToDelete { get; init; } = [];
}
