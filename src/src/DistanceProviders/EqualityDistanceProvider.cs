namespace DistanceProviders;

public sealed class EqualityDistanceProvider<T>(IEqualityComparer<T> equalityComparer)
    : IDistanceProvider<T>
{
    private readonly IEqualityComparer<T> _equalityComparer = equalityComparer;

    public double GetDistance(T source, T target) =>
        _equalityComparer.Equals(source, target) ? 0 : 1;
}
