using MoreLinq;
using UpdateAnki.Extensions;
using UpdateAnki.Models;

namespace UpdateAnki.Utils;

public static class ModificationActionsCalculator
{
    public static ModificationActions<TKey, TValue> GetModificationActions<TKey, TValue>(
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
                s => [new ModificationAction<TKey, TValue>.Add(s)],
                t => GetDeleteActions(t, deleteUnmatched),
                (s, t) => GetMatchingActions(s.ToArray(), t.ToArray(), deleteExcessMatched),
                matchComparer)
            .Aggregate(
                new EnumerableModificationActions<TKey, TValue>(),
                (actions, action) => actions.AddUpdateActions(action))
            .ToArrays();
    }

    private static ModificationAction<TKey, TValue>[] GetDeleteActions<TKey, TValue>(
        IEnumerable<KeyValuePair<TKey, TValue>> keyValues, bool deleteUnmatched) =>
        deleteUnmatched
            ? [new ModificationAction<TKey, TValue>.Delete(keyValues.Select(kvp => kvp.Key))]
            : [];

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S1172:Unused method parameters should be removed",
        Justification = "Pending")]
    private static IEnumerable<ModificationAction<TKey, TValue>>
        GetMatchingActions<TKey, TValue>(
            TValue[] source, KeyValuePair<TKey, TValue>[] target, bool deleteExcessMatched)
    {
        return [];
    }
}
