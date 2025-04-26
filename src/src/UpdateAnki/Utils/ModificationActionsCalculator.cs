using System.Diagnostics;
using DistanceProviders;
using DistanceProviders.Extensions;
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
        IDistanceProvider<TValue>? valueDistanceProvider = null)
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
                (s, t) => GetMatchingActions(
                    s.ToArray(), t.ToArray(), deleteExcessMatched, valueDistanceProvider),
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

    private static IEnumerable<ModificationAction<TKey, TValue>> GetMatchingActions<TKey, TValue>(
        TValue[] source,
        KeyValuePair<TKey, TValue>[] target,
        bool deleteExcessMatched,
        IDistanceProvider<TValue>? valueDistanceProvider)
    {
        var targetValues = target.Select(kvp => kvp.Value).ToList();
        return OptimalMatchCalculator
            .GetOptimalMatch(source, targetValues, valueDistanceProvider)
            .SelectMany((ti, si) => (ti, si) switch
            {
                _ when ti < target.Length && si < source.Length =>
                    GetUpdateActions(source, si, target, ti, valueDistanceProvider),
                _ when ti >= target.Length =>
                    [new ModificationAction<TKey, TValue>.Add([source[si]])],
                _ when si >= source.Length =>
                    GetMatchingDeleteActions(target, ti, deleteExcessMatched),
                _ => throw new UnreachableException("Unexpected output from optimizer"),
            });
    }

    private static IEnumerable<ModificationAction<TKey, TValue>> GetUpdateActions<TKey, TValue>(
        TValue[] source,
        int sourceIndex,
        KeyValuePair<TKey, TValue>[] target,
        int targetIndex,
        IDistanceProvider<TValue>? valueDistanceProvider)
    {
        var establishedValueDistanceProvider =
            valueDistanceProvider ?? EqualityComparer<TValue>.Default.ToDistanceProvider();
        var distance = establishedValueDistanceProvider
            .GetDistance(target[targetIndex].Value, source[sourceIndex]);

        if (MathUtils.EqualWithTolerance(distance, 0))
        {
            return [];
        }

        return
        [
            new ModificationAction<TKey, TValue>.Update(
                [new KeyValuePair<TKey, TValue>(target[targetIndex].Key, source[sourceIndex])]),
        ];
    }

    private static IEnumerable<ModificationAction<TKey, TValue>>
        GetMatchingDeleteActions<TKey, TValue>(
            KeyValuePair<TKey, TValue>[] target, int ti, bool deleteExcessMatched)
    {
        if (!deleteExcessMatched)
        {
            return [];
        }

        return [new ModificationAction<TKey, TValue>.Delete([target[ti].Key])];
    }
}
