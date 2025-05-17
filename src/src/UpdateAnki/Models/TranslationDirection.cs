namespace UpdateAnki.Models;

internal sealed record TranslationDirection
{
    public required string SourceLanguageCode { get; init; }

    public required string TargetLanguageCode { get; init; }
}
