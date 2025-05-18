namespace ChangeSetCalculation.Models;

internal abstract record ChangeAction<TSource, TTarget>
{
    public sealed record Add(IEnumerable<TSource> Values) : ChangeAction<TSource, TTarget>;

    public sealed record Delete(IEnumerable<TTarget> Keys) : ChangeAction<TSource, TTarget>;

    public sealed record Update(IEnumerable<UpdatePair<TSource, TTarget>> Updates)
        : ChangeAction<TSource, TTarget>;
}
