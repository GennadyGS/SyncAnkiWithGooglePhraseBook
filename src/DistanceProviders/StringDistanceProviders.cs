using DistanceProviders.Extensions;

namespace DistanceProviders;

public static class StringDistanceProviders
{
    public static IDistanceProvider<string> Default { get; } =
         CharDistanceProviders.Default.ToStringDistanceProvider();

    public static IDistanceProvider<string> OrdinalIgnoreCase { get; } =
         CharDistanceProviders.OrdinalIgnoreCase.ToStringDistanceProvider();

    public static IDistanceProvider<string> CreateSoftCase(
        double caseDistance, IEqualityComparer<char>? ignoreCaseComparer = null) =>
        CharDistanceProviders
            .CreateSoftCase(caseDistance, ignoreCaseComparer)
            .ToStringDistanceProvider();
}
