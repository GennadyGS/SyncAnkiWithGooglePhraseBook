using Accord.Math.Optimization;

namespace UpdateAnki.Utils;

internal static class OptimalMatchCalculator
{
    public static IReadOnlyList<int> GetOptimalMatch<TValue>(
        IReadOnlyList<TValue> source,
        IReadOnlyList<TValue> target,
        Func<TValue, TValue, double>? distanceProvider)
    {
        var establishedDistanceProvider =
            distanceProvider ?? DistanceUtils.CreateDefaultDistanceProvider<TValue>();
        var maxLength = Math.Max(source.Count, target.Count);
        var costMatrix = Enumerable.Range(0, maxLength)
            .Select(s =>
                Enumerable.Range(0, maxLength)
                    .Select(t => establishedDistanceProvider(source[s], target[t]))
                    .ToArray())
            .ToArray();
        var optimizer = new Munkres(costMatrix);
        if (!optimizer.Minimize())
        {
            throw new InvalidOperationException("Cannot solve for optimal matching assignment.");
        }

        return optimizer.Solution
            .Select(index => (int)index)
            .ToList();
    }
}
