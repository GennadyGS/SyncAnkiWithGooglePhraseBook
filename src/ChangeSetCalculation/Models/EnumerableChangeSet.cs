namespace ChangeSetCalculation.Models;

public sealed record EnumerableChangeSet<TSource, TTarget>
{
    public IEnumerable<TSource> ToAdd { get; init; } = [];

    public IEnumerable<UpdatePair<TSource, TTarget>> ToUpdate { get; init; } = [];

    public IEnumerable<TTarget> ToDelete { get; init; } = [];
}
