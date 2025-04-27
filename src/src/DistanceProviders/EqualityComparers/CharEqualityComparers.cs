using DistanceProviders.Extensions;

namespace DistanceProviders.EqualityComparers;

public static class CharEqualityComparers
{
    public static readonly IEqualityComparer<char> OrdinalIgnoreCase =
        StringComparer.OrdinalIgnoreCase.ToCharEqualityComparer();
}
