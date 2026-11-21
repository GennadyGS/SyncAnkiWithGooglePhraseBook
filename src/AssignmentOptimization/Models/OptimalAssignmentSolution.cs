namespace AssignmentOptimization.Models;

public sealed record OptimalAssignmentSolution
{
    public required IReadOnlyList<OptimalMatch> Matches { get; init; }

    public required double TotalCost { get; init; }
}
