using Translation.Models;
using UpdateAnki.Constants;
using UpdateAnki.Models;
using UpdateAnki.Utils;

namespace UpdateAnki.Extensions;

internal static class NoteInfoExtensions
{
    public static PhraseTranslation ToPhraseTranslation(
        this NoteInfo noteInfo, string modelNamePattern)
    {
        var translationDirection =
            ModelNamePatternEngine.ParseModelName(modelNamePattern, noteInfo.ModelName);
        return new PhraseTranslation
        {
            Source = new Phrase
            {
                LanguageCode = translationDirection.SourceLanguageCode,
                Text = noteInfo.Fields[NoteInfoFields.Front]!.value,
            },
            Target = new Phrase
            {
                LanguageCode = translationDirection.TargetLanguageCode,
                Text = noteInfo.Fields[NoteInfoFields.Back]!.value,
            },
        };
    }
}
