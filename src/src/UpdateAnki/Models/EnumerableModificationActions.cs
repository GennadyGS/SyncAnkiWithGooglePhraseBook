namespace UpdateAnki.Models;

public sealed record EnumerableModificationActions<TSource, TTarget>
{
    public IEnumerable<TSource> ToAdd { get; init; } = [];

    public IEnumerable<(TSource source, TTarget target)> ToUpdate { get; init; } = [];

    public IEnumerable<TTarget> ToDelete { get; init; } = [];
}
