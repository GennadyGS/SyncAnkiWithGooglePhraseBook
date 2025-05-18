namespace ChangeSetCalculation.Models;

public sealed record UpdatePair<TSource, TTarget>(TSource Source, TTarget Target);
