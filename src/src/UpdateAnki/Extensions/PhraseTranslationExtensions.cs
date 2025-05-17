using Translation.Models;
using UpdateAnki.Constants;
using UpdateAnki.Models;
using UpdateAnki.Utils;

namespace UpdateAnki.Extensions;

internal static class PhraseTranslationExtensions
{
    public static AddNoteParams ToAddNoteParams(
        this PhraseTranslation translation, AnkiSettings ankiSettings) =>
        new()
        {
            DeckName = ankiSettings.RootDeckName,
            ModelName = ModelNamePatternEngine.GenerateModelName(
                ankiSettings.ModelNamePattern, translation.GetDirection()),
            Fields = new Dictionary<string, object?>
            {
                [AnkiNoteFields.Front] = translation.Source.Text,
                [AnkiNoteFields.Back] = translation.Target.Text,
            },
        };

    private static TranslationDirection GetDirection(this PhraseTranslation translation) =>
        new()
        {
            SourceLanguageCode = translation.Source.LanguageCode,
            TargetLanguageCode = translation.Target.LanguageCode,
        };
}
