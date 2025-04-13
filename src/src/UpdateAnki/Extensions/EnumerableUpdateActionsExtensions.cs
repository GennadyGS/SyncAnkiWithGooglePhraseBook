using System.Diagnostics;
using UpdateAnki.Models;

namespace UpdateAnki.Extensions;

internal static class EnumerableUpdateActionsExtensions
{
    public static EnumerableModificationActions<TKey, TValue> AddUpdateActions<TKey, TValue>(
        this EnumerableModificationActions<TKey, TValue> source,
        IEnumerable<ModificationAction<TKey, TValue>> actions) =>
        actions.Aggregate(source, AddUpdateAction);

    public static EnumerableModificationActions<TKey, TValue> AddUpdateAction<TKey, TValue>(
        this EnumerableModificationActions<TKey, TValue> source, ModificationAction<TKey, TValue> action) =>
        action switch
        {
            ModificationAction<TKey, TValue>.Add addAction =>
                source with
                {
                    ToAdd = source.ToAdd.Concat(addAction.Values),
                },
            ModificationAction<TKey, TValue>.Update updateAction =>
                source with
                {
                    ToUpdate = source.ToUpdate.Concat(updateAction.Updates),
                },
            ModificationAction<TKey, TValue>.Delete deleteAction =>
                source with
                {
                    ToDelete = source.ToDelete.Concat(deleteAction.Keys),
                },
            _ => throw new UnreachableException(),
        };

    public static ModificationActions<TKey, TValue> ToArrays<TKey, TValue>(
        this EnumerableModificationActions<TKey, TValue> source) =>
        new()
        {
            ToAdd = source.ToAdd.ToArray(),
            ToUpdate = source.ToUpdate.ToArray(),
            ToDelete = source.ToDelete.ToArray(),
        };
}
