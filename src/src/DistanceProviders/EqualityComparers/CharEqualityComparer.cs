namespace DistanceProviders.EqualityComparers;

public sealed class CharEqualityComparer(IEqualityComparer<string> stringEqualityComparer)
    : IEqualityComparer<char>
{
    private readonly IEqualityComparer<string> _stringEqualityComparer = stringEqualityComparer;

    public bool Equals(char x, char y) =>
        _stringEqualityComparer.Equals(x.ToString(), y.ToString());

    public int GetHashCode(char obj) => _stringEqualityComparer.GetHashCode(obj.ToString());
}
