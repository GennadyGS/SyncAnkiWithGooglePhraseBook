using AnkiConnect.Client.Models;
using Translation.Models;
using UpdateAnki.Models;
using UpdateAnki.Utils;

namespace UpdateAnki.Extensions;

internal static class PhraseTranslationExtensions
{
    extension(PhraseTranslation translation)
    {
        public AddNoteParams ToAddNoteParams(AnkiDeckSettings ankiSettings) =>
            new()
            {
                DeckName = ankiSettings.DeckName,
                ModelName = ModelNamePatternEngine.GenerateModelName(
                    ankiSettings.ModelNamePattern, translation.GetDirection()),
                Fields = new Dictionary<string, object?>
                {
                    [NoteInfoFields.Front] = translation.Source.Text,
                    [NoteInfoFields.Back] = translation.Target.Text,
                },
                Options = new()
                {
                    AllowDuplicate = true,
                },
            };

        public TranslationDirection GetDirection() =>
            new()
            {
                SourceLanguageCode = translation.Source.LanguageCode,
                TargetLanguageCode = translation.Target.LanguageCode,
            };
    }
}
