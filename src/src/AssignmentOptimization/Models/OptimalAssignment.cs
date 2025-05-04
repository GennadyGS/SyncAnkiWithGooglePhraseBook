namespace AssignmentOptimization.Models;

public sealed record OptimalAssignment
{
    public required IReadOnlyList<OptimalMatch> Matches { get; init; }

    public required double TotalCost { get; init; }
}
