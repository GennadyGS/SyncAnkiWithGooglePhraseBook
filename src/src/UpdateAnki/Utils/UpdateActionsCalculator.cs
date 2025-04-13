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
        var matches = GetMatches(source, target, matchComparer);
        return new UpdateActions<TKey, TValue>
        {
            ToAdd = GetToAdd(matches),
            ToDelete = deleteUnmatched ? GetToDelete(matches) : [],
            ToUpdate = GetToUpdate(matches, deleteExcessMatched),
        };
    }

    private static (TValue[]? source, KeyValuePair<TKey, TValue>[]? target)[]
        GetMatches<TKey, TValue>(
            IReadOnlyCollection<TValue> source,
            IDictionary<TKey, TValue> target,
            IEqualityComparer<TValue>? matchComparer)
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
                s => (source: (TValue[]?)s.ToArray(), target: null),
                t => (source: (TValue[]?)null, target: (KeyValuePair<TKey, TValue>[]?)t.ToArray()),
                (s, t) => (source: s.ToArray(), target: t.ToArray()),
                matchComparer)
            .ToArray();
    }

    private static TValue[] GetToAdd<TKey, TValue>(
        (TValue[]? source, KeyValuePair<TKey, TValue>[]? target)[] matches) =>
        matches
            .SelectMany(match => match.source is not null && match.target is null
                ? match.source
                : [])
            .ToArray();

    private static TKey[] GetToDelete<TKey, TValue>(
        (TValue[]? source, KeyValuePair<TKey, TValue>[]? target)[] matches) =>
        matches
            .SelectMany(x => x.source is null && x.target is not null
                ? x.target.Select(kvp => kvp.Key)
                : [])
            .ToArray();

    private static IReadOnlyCollection<KeyValuePair<TKey, TValue>> GetToUpdate<TKey, TValue>(
        (TValue[]? source, KeyValuePair<TKey, TValue>[]? target)[] matches,
        bool deleteExcessMatched) =>
        matches
            .SelectMany(match => match.source is not null && match.target is not null
                ? GetUpdates(match.source, match.target, deleteExcessMatched)
                : [])
            .ToArray();

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S1172:Unused method parameters should be removed",
        Justification = "Pending")]
    private static IEnumerable<KeyValuePair<TKey, TValue>> GetUpdates<TValue, TKey>(
        TValue[] source, KeyValuePair<TKey, TValue>[] target, bool deleteExcessMatched)
    {
        return [];
    }
}
