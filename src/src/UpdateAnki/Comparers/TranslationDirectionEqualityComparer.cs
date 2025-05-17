using UpdateAnki.Models;

namespace UpdateAnki.Comparers;

internal sealed class TranslationDirectionEqualityComparer
    : IEqualityComparer<TranslationDirection>
{
    public bool Equals(TranslationDirection? x, TranslationDirection? y)
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
        return languageCodeComparer.Equals(x.SourceLanguageCode, y.SourceLanguageCode) &&
            languageCodeComparer.Equals(x.TargetLanguageCode, y.TargetLanguageCode);
    }

    public int GetHashCode(TranslationDirection obj) =>
        HashCode.Combine(obj.SourceLanguageCode, obj.TargetLanguageCode);
}
