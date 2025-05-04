using System.Diagnostics;
using ChangeSetCalculation.Extensions;
using ChangeSetCalculation.Models;
using ChangeSetCalculation.Utils;
using DistanceProviders;
using DistanceProviders.Extensions;
using MoreLinq.Extensions;

namespace ChangeSetCalculation;

public static class ChangeSetCalculator
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Generalized algorithm requiring a lot of specifications")]
    public static ChangeSet<TSource, TTarget> CalculateChangeSet<TSource, TTarget, TKey>(
        IReadOnlyCollection<TSource> source,
        IReadOnlyCollection<TTarget> target,
        Func<TSource, TKey> sourceKeySelector,
        Func<TTarget, TKey> targetKeySelector,
        bool deleteUnmatched = false,
        bool deleteExcessMatched = false,
        IEqualityComparer<TKey>? matchComparer = null,
        IDistanceProvider<TKey>? keyDistanceProvider = null)
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
                s => [new ChangeAction<TSource, TTarget>.Add(s)],
                t => CalculateDeletions<TSource, TTarget>(t, deleteUnmatched),
                (s, t) => CalculateMatches(
                    s.ToArray(),
                    t.ToArray(),
                    sourceKeySelector,
                    targetKeySelector,
                    deleteExcessMatched,
                    keyDistanceProvider),
                matchComparer)
            .Aggregate(
                new EnumerableChangeSet<TSource, TTarget>(),
                (actions, action) => actions.AddUpdateActions(action))
            .ToArrays();
    }

    private static ChangeAction<TSource, TTarget>[] CalculateDeletions<TSource, TTarget>(
        IEnumerable<TTarget> target, bool deleteUnmatched) =>
        deleteUnmatched
            ? [new ChangeAction<TSource, TTarget>.Delete(target)]
            : [];

    private static IEnumerable<ChangeAction<TSource, TTarget>>
        CalculateMatches<TSource, TTarget, TKey>(
            TSource[] source,
            TTarget[] target,
            Func<TSource, TKey> sourceKeySelector,
            Func<TTarget, TKey> targetKeySelector,
            bool deleteExcessMatched,
            IDistanceProvider<TKey>? keyDistanceProvider)
    {
        var establishedKeyDistanceProvider =
            keyDistanceProvider ?? EqualityComparer<TKey>.Default.ToDistanceProvider();
        return OptimalAssignmentSolver
            .CalculateOptimalAssignment(source.Length, target.Length, GetKeyGetDistance)
            .SelectMany(assignment => (assignment.si, assignment.ti) switch
            {
                ({ } si, { } ti) =>
                    CalculateUpdates(source[si], target[ti], GetKeyGetDistance(si, ti)),
                ({ } si, null) =>
                    [new ChangeAction<TSource, TTarget>.Add([source[si]])],
                (null, { } ti) =>
                    CalculateMatchingDeletions<TSource, TTarget>(target[ti], deleteExcessMatched),
                _ => throw new UnreachableException("Unexpected output from optimizer"),
            });

        double GetKeyGetDistance(int si, int ti) =>
            establishedKeyDistanceProvider.GetDistance(
                sourceKeySelector(source[si]),
                targetKeySelector(target[ti]));
    }

    private static IEnumerable<ChangeAction<TSource, TTarget>> CalculateUpdates<TSource, TTarget>(
        TSource sourceItem, TTarget targetItem, double distance)
    {
        if (MathUtils.EqualWithTolerance(distance, 0))
        {
            return [];
        }

        return
        [
            new ChangeAction<TSource, TTarget>
                .Update([(source: sourceItem, target: targetItem)]),
        ];
    }

    private static IEnumerable<ChangeAction<TSource, TTarget>>
        CalculateMatchingDeletions<TSource, TTarget>(TTarget targetItem, bool deleteExcessMatched)
    {
        if (!deleteExcessMatched)
        {
            return [];
        }

        return [new ChangeAction<TSource, TTarget>.Delete([targetItem])];
    }
}
