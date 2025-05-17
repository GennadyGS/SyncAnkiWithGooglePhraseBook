using System.Text.RegularExpressions;
using Translation.Models;

namespace UpdateAnki.Utils;

internal static class ModelNamePatternEngine
{
    public static TranslationDirection ParseModelName(string moderNamePattern, string modelName)
    {
        var modelNameRegex = GetModelNameRegex(moderNamePattern);
        var regexResult = modelNameRegex.Match(modelName);
        if (!regexResult.Success)
        {
            throw new InvalidOperationException(
                $"Model name '{modelName}' does not match the pattern.");
        }

        return new TranslationDirection
        {
            SourceLanguageCode = regexResult.Groups[Variables.SourceLanguage].Value,
            TargetLanguageCode = regexResult.Groups[Variables.TargetLanguage].Value,
        };
    }

    public static string GenerateModelName(
        string modelNamePattern, TranslationDirection translationDirection) =>
        modelNamePattern
            .ReplaceVariable(Variables.SourceLanguage, translationDirection.SourceLanguageCode)
            .ReplaceVariable(Variables.TargetLanguage, translationDirection.TargetLanguageCode);

    private static Regex GetModelNameRegex(string modelNamePattern)
    {
        var modelNameRegex = Regex.Replace(modelNamePattern, "{(\\w+)}", "(?'$1'[a-z]{2})");
        return new Regex($"^{modelNameRegex}$", RegexOptions.Compiled);
    }

    private static string ReplaceVariable(this string input, string name, string value) =>
        input.Replace($"{{{name}}}", value);

    private static class Variables
    {
        public const string SourceLanguage = "sourceLanguage";

        public const string TargetLanguage = "targetLanguage";
    }
}
