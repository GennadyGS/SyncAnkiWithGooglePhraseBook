using Accord.Math.Optimization;
using DistanceProviders;
using DistanceProviders.Extensions;

namespace ChangeSetCalculation.Utils;

internal static class OptimalAssignmentSolver
{
    public static IReadOnlyList<int> CalculateOptimalAssignment<TValue>(
       IReadOnlyList<TValue> source,
       IReadOnlyList<TValue> target,
       IDistanceProvider<TValue>? distanceProvider)
    {
        var distanceMatrix = GetDistanceMatrix(source, target, distanceProvider);
        var optimizer = new Munkres(distanceMatrix);
        if (!optimizer.Minimize())
        {
            throw new InvalidOperationException("Cannot solve for optimal matching assignment.");
        }

        return optimizer.Solution
            .Select(index => (int)index)
            .ToList();
    }

    private static double[][] GetDistanceMatrix<TValue>(
        IReadOnlyList<TValue> source,
        IReadOnlyList<TValue> target,
        IDistanceProvider<TValue>? distanceProvider)
    {
        var establishedDistanceProvider =
            distanceProvider ?? EqualityComparer<TValue>.Default.ToDistanceProvider();
        var distanceRectMatrix = Enumerable.Range(0, source.Count)
            .Select(s =>
                Enumerable.Range(0, target.Count)
                    .Select(t => establishedDistanceProvider.GetDistance(source[s], target[t]))
                    .ToArray())
            .ToArray();
        var maxValue = distanceRectMatrix.SelectMany(x => x).Max();
        var maxSize = Math.Max(source.Count, target.Count);
        return SetDimensions(distanceRectMatrix, maxSize, maxSize, maxValue + 1);
    }

    private static T[][] SetDimensions<T>(
        T[][] source, int dimension1, int dimension2, T defaultValue) =>
        Enumerable.Range(0, dimension1)
            .Select(i1 =>
                Enumerable.Range(0, dimension2)
                    .Select(i2 =>
                        i1 < source.Length && i2 < source[i1].Length
                            ? source[i1][i2]
                            : defaultValue)
                    .ToArray())
            .ToArray();
}
