namespace ChangeSetCalculation.Models;

public static class UpdatePair
{
    public static UpdatePair<TSource, TTarget> Create<TSource, TTarget>(
        TSource source, TTarget target) => new(source, target);
}
