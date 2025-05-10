using Translation.Models;

namespace UpdateAnki.Comparers;

internal sealed class PhraseComparer(IEqualityComparer<string>? stringComparer = null)
    : IEqualityComparer<Phrase>
{
    private readonly IEqualityComparer<string> _stringComparer =
        stringComparer ?? StringComparer.Ordinal;

    public bool Equals(Phrase? x, Phrase? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        if (x.GetType() != y.GetType())
        {
            return false;
        }

        return _stringComparer.Equals(x.Text, y.Text) &&
            _stringComparer.Equals(x.LanguageCode, y.LanguageCode);
    }

    public int GetHashCode(Phrase obj) =>
        HashCode.Combine(obj.Text, obj.LanguageCode);
}
