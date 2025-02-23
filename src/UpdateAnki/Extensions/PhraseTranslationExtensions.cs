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
            ModelName = translation.GetAnkiModelName(),
            Fields = new Dictionary<string, object?>
            {
                [AnkiNoteFields.Front] = translation.Source,
                [AnkiNoteFields.Back] = translation.Target,
            },
        };

    private static string GetAnkiModelName(this PhraseTranslation translation) =>
        $"{translation.Source.LanguageCode}-{translation.Target.LanguageCode}";
}
