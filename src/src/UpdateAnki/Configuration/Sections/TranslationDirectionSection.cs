namespace UpdateAnki.Configuration.Sections;

internal sealed record TranslationDirectionSection
{
    public string? SourceLanguageCode { get; init; }

    public string? TargetLanguageCode { get; init; }
}
