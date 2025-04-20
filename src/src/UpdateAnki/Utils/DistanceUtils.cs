namespace UpdateAnki.Utils;

internal static class DistanceUtils
{
    public static Func<T, T, double> CreateDefaultDistanceProvider<T>()
    {
        var equalityComparer = EqualityComparer<T>.Default;
        return (x, y) => equalityComparer.Equals(x, y) ? 0 : 1;
    }
}
