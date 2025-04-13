namespace UpdateAnki.Models;

public sealed record ModificationActions<TKey, TValue>
{
    public IReadOnlyCollection<TValue> ToAdd { get; init; } = [];

    public IReadOnlyCollection<KeyValuePair<TKey, TValue>> ToUpdate { get; init; } = [];

    public IReadOnlyCollection<TKey> ToDelete { get; init; } = [];
}
