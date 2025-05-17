using System.Text.RegularExpressions;
using UpdateAnki.Models;

namespace UpdateAnki.Utils;

internal static class ModelNamePatternEngine
{
    public static Func<string, TranslationDirection> CreateModelNameParser(
        string moderNamePattern)
    {
        var modelNameRegex = GetModelNameRegex(moderNamePattern);
        return modelName => ModelNameToTranslationDirection(modelName, modelNameRegex);
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

    private static TranslationDirection ModelNameToTranslationDirection(
        string modelName, Regex modelNameRegex)
    {
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

    private static string ReplaceVariable(this string input, string name, string value) =>
        input.Replace($"{{{name}}}", value);

    private static class Variables
    {
        public const string SourceLanguage = "sourceLanguage";

        public const string TargetLanguage = "targetLanguage";
    }
}
