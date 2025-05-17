namespace UpdateAnki.Models;

internal sealed record TranslationDirection
{
    public required string SourceLanguage { get; init; }

    public required string TargetLanguage { get; init; }
}
