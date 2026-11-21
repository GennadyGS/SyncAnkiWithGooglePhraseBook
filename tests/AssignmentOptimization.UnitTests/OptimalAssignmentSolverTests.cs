using AssignmentOptimization.Models;
using FluentAssertions;
using Xunit;

namespace AssignmentOptimization.UnitTests;

public sealed class OptimalAssignmentSolverTests
{
    [Fact]
    public void CalculateOptimalAssignment_ShouldReturnOptimalAssignment_WhenValidInputProvided()
    {
        var costMatrix = new[,]
        {
            { 9, 2, 7 },
            { 6, 4, 3 },
            { 5, 8, 1 },
        };

        var result = CalculateOptimalAssignment(costMatrix);

        var expectedResult = new OptimalAssignmentSolution
        {
            Matches =
            [
                new()
                {
                    SourceIndex = 0,
                    TargetIndex = 1,
                    Cost = 2,
                },
                new()
                {
                    SourceIndex = 1,
                    TargetIndex = 0,
                    Cost = 6,
                },
                new()
                {
                    SourceIndex = 2,
                    TargetIndex = 2,
                    Cost = 1,
                },
            ],
            TotalCost = 9,
        };
        result.Should().BeEquivalentTo(expectedResult);
    }

    private static OptimalAssignmentSolution CalculateOptimalAssignment(int[,] costMatrix) =>
        OptimalAssignmentSolver.CalculateOptimalAssignment(
            costMatrix.GetLength(0), costMatrix.GetLength(1), (i, j) => costMatrix[i, j]);
}
