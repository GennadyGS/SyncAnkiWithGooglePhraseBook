using MoreLinq;
using UpdateAnki.Extensions;
using UpdateAnki.Models;

namespace UpdateAnki.Utils;

public static class UpdateActionsCalculator
{
    public static UpdateActions<TKey, TValue> GetUpdateActions<TKey, TValue>(
        IReadOnlyCollection<TValue> source,
        IDictionary<TKey, TValue> target,
        bool deleteUnmatched = false,
        bool deleteExcessMatched = false,
        IEqualityComparer<TValue>? matchComparer = null,
        Func<TValue, TValue, double>? valueEditDistanceProvider = null)
    {
        var groupedSource = source
            .GroupBy(x => x, x => x, matchComparer)
            .ToList();
        var groupedTarget = target
            .GroupBy(x => x.Value, x => x, matchComparer)
            .ToList();
        return groupedSource
            .FullJoin(
                groupedTarget,
                s => s.Key,
                t => t.Key,
                s => (UpdateAction<TKey, TValue>)new UpdateAction<TKey, TValue>.Add(s),
                t => new UpdateAction<TKey, TValue>.Delete(GetDeletes(t, deleteUnmatched)),
                (s, t) => new UpdateAction<TKey, TValue>.Update(
                    GetUpdates(s.ToArray(), t.ToArray(), deleteExcessMatched)),
                matchComparer)
            .Aggregate(
                new EnumerableUpdateActions<TKey, TValue>(),
                (actions, action) => actions.AddUpdateAction(action))
            .ToArrays();
    }

    private static IEnumerable<TKey> GetDeletes<TKey, TValue>(
        IEnumerable<KeyValuePair<TKey, TValue>> keyValues, bool deleteUnmatched) =>
        deleteUnmatched
            ? keyValues.Select(kvp => kvp.Key)
            : [];

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S1172:Unused method parameters should be removed",
        Justification = "Pending")]
    private static IEnumerable<KeyValuePair<TKey, TValue>> GetUpdates<TKey, TValue>(
        TValue[] source, KeyValuePair<TKey, TValue>[] target, bool deleteExcessMatched)
    {
        return [];
    }
}
