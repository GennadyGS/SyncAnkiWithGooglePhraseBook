using System.Diagnostics;
using UpdateAnki.Models;

namespace UpdateAnki.Extensions;

internal static class EnumerableUpdateActionsExtensions
{
    public static EnumerableModificationActions<TSource, TTarget>
        AddUpdateActions<TSource, TTarget>(
            this EnumerableModificationActions<TSource, TTarget> source,
            IEnumerable<ModificationAction<TSource, TTarget>> actions) =>
            actions.Aggregate(source, AddUpdateAction);

    public static EnumerableModificationActions<TSource, TTarget> AddUpdateAction<TSource, TTarget>(
        this EnumerableModificationActions<TSource, TTarget> source,
        ModificationAction<TSource, TTarget> action) =>
        action switch
        {
            ModificationAction<TSource, TTarget>.Add addAction =>
                source with
                {
                    ToAdd = source.ToAdd.Concat(addAction.Values),
                },
            ModificationAction<TSource, TTarget>.Update updateAction =>
                source with
                {
                    ToUpdate = source.ToUpdate.Concat(updateAction.Updates),
                },
            ModificationAction<TSource, TTarget>.Delete deleteAction =>
                source with
                {
                    ToDelete = source.ToDelete.Concat(deleteAction.Keys),
                },
            _ => throw new UnreachableException(),
        };

    public static ModificationActions<TSource, TTarget> ToArrays<TSource, TTarget>(
        this EnumerableModificationActions<TSource, TTarget> source) =>
        new()
        {
            ToAdd = source.ToAdd.ToArray(),
            ToUpdate = source.ToUpdate.ToArray(),
            ToDelete = source.ToDelete.ToArray(),
        };
}
