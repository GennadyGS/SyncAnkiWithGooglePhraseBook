namespace DistanceProviders;

public sealed class StringDistanceProvider(IDistanceProvider<char> charDistanceProvider)
    : IDistanceProvider<string>
{
    private readonly IDistanceProvider<IReadOnlyList<char>> _listDistanceProvider =
        new ListDistanceProvider<char>(charDistanceProvider);

    public double GetDistance(string source, string target) =>
        _listDistanceProvider.GetDistance(source.ToCharArray(), target.ToCharArray());
}
