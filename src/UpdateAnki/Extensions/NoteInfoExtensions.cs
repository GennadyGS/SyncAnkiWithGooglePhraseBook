using AnkiConnect.Client.Models;
using Translation.Models;
using UpdateAnki.Utils;

namespace UpdateAnki.Extensions;

internal static class NoteInfoExtensions
{
    public static PhraseTranslation ToPhraseTranslation(
        this NoteInfo noteInfo, string modelNamePattern)
    {
        var translationDirection =
            ModelNamePatternEngine.ParseModelName(modelNamePattern, noteInfo.ModelName);
        return new()
        {
            Source = new()
            {
                LanguageCode = translationDirection.SourceLanguageCode,
                Text = noteInfo.Fields[NoteInfoFields.Front]!.value,
            },
            Target = new()
            {
                LanguageCode = translationDirection.TargetLanguageCode,
                Text = noteInfo.Fields[NoteInfoFields.Back]!.value,
            },
        };
    }
}
