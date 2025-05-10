using Translation.Models;

namespace UpdateAnki.Comparers;

internal sealed class PhraseTranslationMatchComparer : IEqualityComparer<PhraseTranslation>
{
    private static readonly StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;

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

        var sourcePhraseComparer = new PhraseComparer(StringComparer);
        return sourcePhraseComparer.Equals(x.Source, y.Source) &&
            StringComparer.Equals(x.Target.LanguageCode, y.Target.LanguageCode);
    }

    public int GetHashCode(PhraseTranslation obj) =>
        HashCode.Combine(obj.Source, obj.Target);
}
