using Common.Extensions;
using Translation.Models;
using UpdateAnki.Constants;
using UpdateAnki.Models;

namespace UpdateAnki.Extensions;

internal static class PhraseTranslationExtensions
{
    public static AddNoteParams ToAddNoteParams(
        this PhraseTranslation translation, AnkiSettings ankiSettings) =>
        new()
        {
            DeckName = ankiSettings.RootDeckName.ThrowIfNull(),
            ModelName = translation.GetAnkiModelName(ankiSettings.ModelNamePattern.ThrowIfNull()),
            Fields = new Dictionary<string, object?>
            {
                [AnkiNoteFields.Front] = translation.Source.Text,
                [AnkiNoteFields.Back] = translation.Target.Text,
            },
        };

    private static string GetAnkiModelName(
        this PhraseTranslation translation, string modelNamePattern) =>
        modelNamePattern
            .Replace("{sourceLanguage}", translation.Source.LanguageCode)
            .Replace("{targetLanguage}", translation.Target.LanguageCode);
}
