using System.Diagnostics;
using DistanceProviders;
using DistanceProviders.Extensions;
using MoreLinq;
using UpdateAnki.Extensions;
using UpdateAnki.Models;

namespace UpdateAnki.Utils;

public static class ModificationActionsCalculator
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Generalized algorithm requiring a lot of specifications")]
    public static ModificationActions<TSource, TTarget>
        GetModificationActions<TSource, TTarget, TKey>(
            IReadOnlyCollection<TSource> source,
            IReadOnlyCollection<TTarget> target,
            Func<TSource, TKey> sourceKeySelector,
            Func<TTarget, TKey> targetKeySelector,
            bool deleteUnmatched = false,
            bool deleteExcessMatched = false,
            IEqualityComparer<TKey>? matchComparer = null,
            IDistanceProvider<TKey>? valueDistanceProvider = null)
    {
        var groupedSource = source
            .GroupBy(sourceKeySelector, x => x, matchComparer)
            .ToList();
        var groupedTarget = target
            .GroupBy(targetKeySelector, x => x, matchComparer)
            .ToList();
        return groupedSource
            .FullJoin(
                groupedTarget,
                s => s.Key,
                t => t.Key,
                s => [new ModificationAction<TSource, TTarget>.Add(s),],
                t => GetDeleteActions<TSource, TTarget>(t, deleteUnmatched),
                (s, t) => GetMatchingActions(
                    s.ToArray(),
                    t.ToArray(),
                    sourceKeySelector,
                    targetKeySelector,
                    deleteExcessMatched,
                    valueDistanceProvider),
                matchComparer)
            .Aggregate(
                new EnumerableModificationActions<TSource, TTarget>(),
                (actions, action) => actions.AddUpdateActions(action))
            .ToArrays();
    }

    private static ModificationAction<TSource, TTarget>[] GetDeleteActions<TSource, TTarget>(
        IEnumerable<TTarget> target, bool deleteUnmatched) =>
        deleteUnmatched
            ? [new ModificationAction<TSource, TTarget>.Delete(target)]
            : [];

    private static IEnumerable<ModificationAction<TSource, TTarget>>
        GetMatchingActions<TSource, TTarget, TKey>(
            TSource[] source,
            TTarget[] target,
            Func<TSource, TKey> sourceKeySelector,
            Func<TTarget, TKey> targetKeySelector,
            bool deleteExcessMatched,
            IDistanceProvider<TKey>? valueDistanceProvider)
    {
        var sourceKeys = source.Select(sourceKeySelector).ToArray();
        var targetKeys = target.Select(targetKeySelector).ToArray();
        return OptimalMatchCalculator
            .GetOptimalMatch(sourceKeys, targetKeys, valueDistanceProvider)
            .SelectMany((ti, si) => (ti, si) switch
            {
                _ when ti < target.Length && si < source.Length => GetUpdateActions(
                    source[si], sourceKeys[si], target[ti], targetKeys[ti], valueDistanceProvider),
                _ when ti >= target.Length =>
                    [new ModificationAction<TSource, TTarget>.Add([source[si]])],
                _ when si >= source.Length =>
                    GetMatchingDeleteActions<TSource, TTarget>(target[ti], deleteExcessMatched),
                _ => throw new UnreachableException("Unexpected output from optimizer"),
            });
    }

    private static IEnumerable<ModificationAction<TSource, TTarget>>
        GetUpdateActions<TSource, TTarget, TKey>(
            TSource sourceItem,
            TKey sourceKey,
            TTarget targetItem,
            TKey targetKey,
            IDistanceProvider<TKey>? valueDistanceProvider)
    {
        var establishedValueDistanceProvider =
            valueDistanceProvider ?? EqualityComparer<TKey>.Default.ToDistanceProvider();
        var distance = establishedValueDistanceProvider.GetDistance(sourceKey, targetKey);

        if (MathUtils.EqualWithTolerance(distance, 0))
        {
            return [];
        }

        return
        [
            new ModificationAction<TSource, TTarget>
                .Update([(source: sourceItem, target: targetItem)]),
        ];
    }

    private static IEnumerable<ModificationAction<TSource, TTarget>>
        GetMatchingDeleteActions<TSource, TTarget>(TTarget targetItem, bool deleteExcessMatched)
    {
        if (!deleteExcessMatched)
        {
            return [];
        }

        return [new ModificationAction<TSource, TTarget>.Delete([targetItem])];
    }
}
