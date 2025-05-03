namespace UpdateAnki.Models;

public sealed record ChangeSet<TSource, TTarget>
{
    public IReadOnlyCollection<TSource> ToAdd { get; init; } = [];

    public IReadOnlyCollection<(TSource source, TTarget target)> ToUpdate { get; init; } = [];

    public IReadOnlyCollection<TTarget> ToDelete { get; init; } = [];
}
