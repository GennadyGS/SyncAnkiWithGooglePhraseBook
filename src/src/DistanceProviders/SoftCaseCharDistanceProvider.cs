using DistanceProviders.EqualityComparers;

namespace DistanceProviders;

public sealed class SoftCaseCharDistanceProvider(
    double caseDistance, IEqualityComparer<char>? ignoreCaseComparer = null)
    : IDistanceProvider<char>
{
    private readonly double _caseDistance = caseDistance;
    private readonly IEqualityComparer<char> _ignoreCaseComparer =
        ignoreCaseComparer ?? CharEqualityComparers.OrdinalIgnoreCase;

    public double GetDistance(char source, char target) =>
        (source, target) switch
        {
            _ when source == target => 0,
            _ when _ignoreCaseComparer.Equals(source, target) => _caseDistance,
            _ => 1,
        };
}
