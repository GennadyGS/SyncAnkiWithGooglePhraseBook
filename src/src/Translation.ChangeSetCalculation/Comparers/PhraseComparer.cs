using Translation.Models;
using Translation.Models.Comparers;

namespace Translation.ChangeSetCalculation.Comparers;

internal sealed class PhraseComparer : IEqualityComparer<Phrase>
{
    private static readonly StringComparer TextComparer = EqualityComparers.TextComparer;
    private static readonly StringComparer LanguageCodeComparer = EqualityComparers.LanguageCodeComparer;

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

        return TextComparer.Equals(x.Text, y.Text) &&
            LanguageCodeComparer.Equals(x.LanguageCode, y.LanguageCode);
    }

    public int GetHashCode(Phrase obj) =>
        HashCode.Combine(
            TextComparer.GetHashCode(obj.Text), LanguageCodeComparer.GetHashCode(obj.LanguageCode));
}
