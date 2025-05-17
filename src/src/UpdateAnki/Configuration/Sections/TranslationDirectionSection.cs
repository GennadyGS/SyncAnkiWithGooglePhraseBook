namespace UpdateAnki.Configuration.Sections;

internal sealed record TranslationDirectionSection
{
    public string? SourceLanguage { get; init; }

    public string? TargetLanguage { get; init; }
}
