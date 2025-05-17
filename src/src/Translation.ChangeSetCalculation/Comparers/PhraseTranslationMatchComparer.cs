using Translation.Models;
using Translation.Models.Comparers;

namespace Translation.ChangeSetCalculation.Comparers;

internal sealed class PhraseTranslationMatchComparer : IEqualityComparer<PhraseTranslation>
{
    private readonly PhraseComparer _sourcePhraseComparer = new();

    public bool Equals(PhraseTranslation? x, PhraseTranslation? y)
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

        var languageCodeComparer = EqualityComparers.LanguageCodeComparer;
        return _sourcePhraseComparer.Equals(x.Source, y.Source) &&
            languageCodeComparer.Equals(x.Target.LanguageCode, y.Target.LanguageCode);
    }

    public int GetHashCode(PhraseTranslation obj) =>
        HashCode.Combine(obj.Source, obj.Target);
}
