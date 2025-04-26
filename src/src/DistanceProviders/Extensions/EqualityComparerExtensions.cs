namespace DistanceProviders.Extensions;

public static class EqualityComparerExtensions
{
    public static IDistanceProvider<T> ToDistanceProvider<T>(
        this IEqualityComparer<T> equalityComparer) =>
        new EqualityDistanceProvider<T>(equalityComparer);
}
