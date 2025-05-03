using System.Diagnostics;
using UpdateAnki.Models;

namespace ChangeSetCalculation.Extensions;

internal static class EnumerableUpdateActionsExtensions
{
    public static EnumerableChangeSet<TSource, TTarget>
        AddUpdateActions<TSource, TTarget>(
            this EnumerableChangeSet<TSource, TTarget> source,
            IEnumerable<ChangeAction<TSource, TTarget>> actions) =>
            actions.Aggregate(source, AddUpdateAction);

    public static EnumerableChangeSet<TSource, TTarget> AddUpdateAction<TSource, TTarget>(
        this EnumerableChangeSet<TSource, TTarget> source,
        ChangeAction<TSource, TTarget> action) =>
        action switch
        {
            ChangeAction<TSource, TTarget>.Add addAction =>
                source with
                {
                    ToAdd = source.ToAdd.Concat(addAction.Values),
                },
            ChangeAction<TSource, TTarget>.Update updateAction =>
                source with
                {
                    ToUpdate = source.ToUpdate.Concat(updateAction.Updates),
                },
            ChangeAction<TSource, TTarget>.Delete deleteAction =>
                source with
                {
                    ToDelete = source.ToDelete.Concat(deleteAction.Keys),
                },
            _ => throw new UnreachableException(),
        };

    public static ChangeSet<TSource, TTarget> ToArrays<TSource, TTarget>(
        this EnumerableChangeSet<TSource, TTarget> source) =>
        new()
        {
            ToAdd = source.ToAdd.ToArray(),
            ToUpdate = source.ToUpdate.ToArray(),
            ToDelete = source.ToDelete.ToArray(),
        };
}
