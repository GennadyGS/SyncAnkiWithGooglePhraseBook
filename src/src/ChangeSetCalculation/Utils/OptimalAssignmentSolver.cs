using Accord.Math.Optimization;
using ChangeSetCalculation.Models;

namespace ChangeSetCalculation.Utils;

internal static class OptimalAssignmentSolver
{
    public static OptimalAssignment CalculateOptimalAssignment(
        int sourceCount, int targetCount, Func<int, int, double> costProvider)
    {
        var costMatrix = GetCostMatrix(sourceCount, targetCount, costProvider);
        var optimizer = new Munkres(costMatrix);
        if (!optimizer.Minimize())
        {
            throw new InvalidOperationException("Cannot solve for optimal matching assignment.");
        }

        var matches = optimizer.Solution
            .Select((ti, si) => CreateMatch(si, ti))
            .ToList();
        return new OptimalAssignment
        {
            Matches = matches,
            TotalCost = optimizer.Value,
        };

        OptimalMatch CreateMatch(int sourceIndex, double targetIndex) =>
            new()
            {
                SourceIndex = sourceIndex < sourceCount ? sourceIndex : null,
                TargetIndex = targetIndex < targetCount ? (int)targetIndex : null,
                Cost = sourceIndex < sourceCount && targetIndex < targetCount
                    ? costMatrix[sourceIndex][(int)targetIndex]
                    : null,
            };
    }

    private static double[][] GetCostMatrix(
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
