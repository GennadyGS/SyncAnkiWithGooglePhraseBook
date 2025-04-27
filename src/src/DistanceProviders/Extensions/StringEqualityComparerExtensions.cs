using DistanceProviders.EqualityComparers;

namespace DistanceProviders.Extensions;

public static class StringEqualityComparerExtensions
{
    public static IEqualityComparer<char> ToCharEqualityComparer(
        this IEqualityComparer<string> source) => new CharEqualityComparer(source);
}
