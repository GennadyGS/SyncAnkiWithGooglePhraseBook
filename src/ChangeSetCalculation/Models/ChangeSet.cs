namespace ChangeSetCalculation.Models;

public sealed record ChangeSet<TSource, TTarget>
{
    public IReadOnlyCollection<TSource> ToAdd { get; init; } = [];

    public IReadOnlyCollection<UpdatePair<TSource, TTarget>> ToUpdate { get; init; } = [];

    public IReadOnlyCollection<TTarget> ToDelete { get; init; } = [];
}
