namespace AssignmentOptimization.Models;

public readonly record struct OptimalMatch
{
    public int? SourceIndex { get; init; }

    public int? TargetIndex { get; init; }

    public double? Cost { get; init; }

    public void Deconstruct(out int? sourceIndex, out int? targetIndex, out double? cost)
    {
        sourceIndex = SourceIndex;
        targetIndex = TargetIndex;
        cost = Cost;
    }
}
