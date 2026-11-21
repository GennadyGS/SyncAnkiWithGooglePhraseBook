using Translation.Models;
using Translation.Models.Comparers;

namespace Translation.ChangeSetCalculation.Comparers;

internal sealed class PhraseTranslationMatchComparer : IEqualityComparer<PhraseTranslation>
{
    private static readonly StringComparer LanguageCodeComparer = EqualityComparers.LanguageCodeComparer;
    private static readonly PhraseComparer SourcePhraseComparer = new();

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

        return SourcePhraseComparer.Equals(x.Source, y.Source) &&
            LanguageCodeComparer.Equals(x.Target.LanguageCode, y.Target.LanguageCode);
    }

    public int GetHashCode(PhraseTranslation obj) =>
        HashCode.Combine(
            SourcePhraseComparer.GetHashCode(obj.Source),
            LanguageCodeComparer.GetHashCode(obj.Target.LanguageCode));
}
