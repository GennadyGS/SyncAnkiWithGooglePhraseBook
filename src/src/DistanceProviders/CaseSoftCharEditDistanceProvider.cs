namespace DistanceProviders;

public sealed class CaseSoftCharEditDistanceProvider(double caseDistance)
    : IDistanceProvider<char>
{
    private readonly double _caseDistance = caseDistance;

    public double GetDistance(char source, char target) =>
        (source, target) switch
        {
            _ when source == target => 0,
            _ when char.ToLower(source) == char.ToLower(target) => _caseDistance,
            _ => 1,
        };
}
