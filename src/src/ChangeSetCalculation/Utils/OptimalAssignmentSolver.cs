using Accord.Math.Optimization;

namespace ChangeSetCalculation.Utils;

internal static class OptimalAssignmentSolver
{
    public static IReadOnlyList<(int? si, int? ti)> CalculateOptimalAssignment(
        int sourceCount, int targetCount, Func<int, int, double> costProvider)
    {
        var distanceMatrix = GetDistanceMatrix(sourceCount, targetCount, costProvider);
        var optimizer = new Munkres(distanceMatrix);
        if (!optimizer.Minimize())
        {
            throw new InvalidOperationException("Cannot solve for optimal matching assignment.");
        }

        return optimizer.Solution
            .Select((ti, si) =>
                (si < sourceCount ? (int?)si : null, ti < targetCount ? (int?)ti : null))
            .ToList();
    }

    private static double[][] GetDistanceMatrix(
        int sourceCount, int targetCount, Func<int, int, double> costProvider)
    {
        var maxCount = Math.Max(sourceCount, targetCount);
        return Enumerable.Range(0, maxCount)
            .Select(si =>
                Enumerable.Range(0, maxCount)
                    .Select(ti => si < sourceCount && ti < targetCount ? costProvider(si, ti) : 0)
                    .ToArray())
            .ToArray();
    }
}
