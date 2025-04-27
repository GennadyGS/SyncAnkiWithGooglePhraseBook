namespace DistanceProviders;

public sealed class StringEditDistanceProvider(
    IDistanceProvider<char> charEditDistanceProvider)
    : IDistanceProvider<string>
{
    private readonly IDistanceProvider<IReadOnlyList<char>> _listDistanceProvider =
        new ListDistanceProvider<char>(charEditDistanceProvider);

    public double GetDistance(string source, string target) =>
        _listDistanceProvider.GetDistance(source.ToCharArray(), target.ToCharArray());
}
