using MoreLinq;
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
        var fullOuterJoin = groupedSource.FullJoin(
            groupedTarget,
            s => s.Key,
            t => t.Key,
            s => (source: (TValue[]?)s.ToArray(), target: null),
            t => (source: (TValue[]?)null, target: (KeyValuePair<TKey, TValue>[]?)t.ToArray()),
            (s, t) => (source: s.ToArray(), target: t.ToArray()),
            matchComparer);
        var toAdd = fullOuterJoin
            .Where(x => x.source is not null && x.target is null)
            .SelectMany(x => x.source!)
            .ToArray();
        return new UpdateActions<TKey, TValue>
        {
            ToAdd = toAdd,
        };
    }
}
