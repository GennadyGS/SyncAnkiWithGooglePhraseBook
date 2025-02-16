namespace UpdateAnki.Models;

internal sealed record UpdateActions<TKey, TValue>
{
    public IReadOnlyCollection<TValue> ToAdd { get; init; } = [];

    public IReadOnlyCollection<KeyValuePair<TKey, TValue>> ToUpdate { get; init; } = [];

    public IReadOnlyCollection<TKey> ToDelete { get; init; } = [];
}
