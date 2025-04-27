using DistanceProviders.EqualityComparers;
using DistanceProviders.Extensions;

namespace DistanceProviders;

public static class CharDistanceProviders
{
    public static IDistanceProvider<char> Default { get; } =
        EqualityComparer<char>.Default.ToDistanceProvider();

    public static IDistanceProvider<char> OrdinalIgnoreCase { get; } =
        CharEqualityComparers.OrdinalIgnoreCase.ToDistanceProvider();

    public static IDistanceProvider<char> CreateSoftCase(
        double caseDistance, IEqualityComparer<char>? ignoreCaseComparer = null) =>
        new SoftCaseCharDistanceProvider(caseDistance, ignoreCaseComparer);
}
