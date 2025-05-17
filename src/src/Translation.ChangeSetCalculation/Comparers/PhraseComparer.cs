using Translation.Models;
using Translation.Models.Comparers;

namespace Translation.ChangeSetCalculation.Comparers;

internal sealed class PhraseComparer : IEqualityComparer<Phrase>
{
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

        return EqualityComparers.TextComparer.Equals(x.Text, y.Text) &&
            EqualityComparers.LanguageCodeComparer.Equals(x.LanguageCode, y.LanguageCode);
    }

    public int GetHashCode(Phrase obj) =>
        HashCode.Combine(obj.Text, obj.LanguageCode);
}
