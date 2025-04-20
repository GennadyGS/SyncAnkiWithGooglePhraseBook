namespace UpdateAnki.UnitTests.Extensions;

internal static class EqualityComparerExtensions
{
    public static Func<T, T, double> ToDistanceProvider<T>(
        this IEqualityComparer<T> equalityComparer)
    {
        return (x, y) => equalityComparer.Equals(x, y) ? 0 : 1;
    }
}
