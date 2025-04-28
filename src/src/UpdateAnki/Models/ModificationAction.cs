namespace UpdateAnki.Models;

internal abstract record ModificationAction<TSource, TTarget>
{
    public sealed record Add(IEnumerable<TSource> Values) : ModificationAction<TSource, TTarget>;

    public sealed record Delete(IEnumerable<TTarget> Keys) : ModificationAction<TSource, TTarget>;

    public sealed record Update(IEnumerable<(TSource source, TTarget target)> Updates)
        : ModificationAction<TSource, TTarget>;
}
