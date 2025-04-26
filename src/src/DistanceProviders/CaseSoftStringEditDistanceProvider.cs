namespace DistanceProviders;

public sealed class CaseSoftStringEditDistanceProvider(double caseDistance)
    : IDistanceProvider<string>
{
    private readonly IDistanceProvider<IReadOnlyList<char>> _listDistanceProvider =
        new ListDistanceProvider<char>(
            new CaseSoftCharEditDistanceProvider(caseDistance));

    public double GetDistance(string source, string target) =>
        _listDistanceProvider.GetDistance(source.ToCharArray(), target.ToCharArray());
}
