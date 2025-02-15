using System.Text.RegularExpressions;
using Translation.Models;
using UpdateAnki.Constants;
using UpdateAnki.Models;

namespace UpdateAnki.Extensions;

internal static class NoteInfoExtensions
{
    public static PhraseTranslation ToPhraseTranslation(
        this NoteInfo noteInfo, Regex modelNameRegex)
    {
        var regexResult = modelNameRegex.Match(noteInfo.ModelName);
        if (!regexResult.Success)
        {
            throw new InvalidOperationException(
                $"Model name '{noteInfo.ModelName}' does not match the pattern.");
        }

        return new PhraseTranslation
        {
            Source = new Phrase
            {
                LanguageCode = regexResult.Groups[ModelNameTemplateVariables.SourceLanguage].Value,
                Text = noteInfo.Fields[NoteInfoFields.Front]!.value,
            },
            Target = new Phrase
            {
                LanguageCode = regexResult.Groups[ModelNameTemplateVariables.TargetLanguage].Value,
                Text = noteInfo.Fields[NoteInfoFields.Back]!.value,
            },
        };
    }
}