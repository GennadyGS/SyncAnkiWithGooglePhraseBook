using System.Diagnostics;
using UpdateAnki.Models;

namespace UpdateAnki.Extensions;

internal static class EnumerableUpdateActionsExtensions
{
    public static EnumerableUpdateActions<TKey, TValue> AddUpdateActions<TKey, TValue>(
        this EnumerableUpdateActions<TKey, TValue> source,
        IEnumerable<UpdateAction<TKey, TValue>> actions) =>
        actions.Aggregate(source, AddUpdateAction);

    public static EnumerableUpdateActions<TKey, TValue> AddUpdateAction<TKey, TValue>(
        this EnumerableUpdateActions<TKey, TValue> source, UpdateAction<TKey, TValue> action) =>
        action switch
        {
            UpdateAction<TKey, TValue>.Add addAction =>
                source with
                {
                    ToAdd = source.ToAdd.Concat(addAction.Values),
                },
            UpdateAction<TKey, TValue>.Update updateAction =>
                source with
                {
                    ToUpdate = source.ToUpdate.Concat(updateAction.Updates),
                },
            UpdateAction<TKey, TValue>.Delete deleteAction =>
                source with
                {
                    ToDelete = source.ToDelete.Concat(deleteAction.Keys),
                },
            _ => throw new UnreachableException(),
        };

    public static UpdateActions<TKey, TValue> ToArrays<TKey, TValue>(
        this EnumerableUpdateActions<TKey, TValue> source) =>
        new()
        {
            ToAdd = source.ToAdd.ToArray(),
            ToUpdate = source.ToUpdate.ToArray(),
            ToDelete = source.ToDelete.ToArray(),
        };
}
