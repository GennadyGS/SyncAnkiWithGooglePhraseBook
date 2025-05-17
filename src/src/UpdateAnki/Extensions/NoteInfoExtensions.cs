using Translation.Models;
using UpdateAnki.Constants;
using UpdateAnki.Models;

namespace UpdateAnki.Extensions;

internal static class NoteInfoExtensions
{
    public static PhraseTranslation ToPhraseTranslation(
        this NoteInfo noteInfo, Func<string, TranslationDirection> modelNameParser)
    {
        var translationDirection = modelNameParser(noteInfo.ModelName);
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
