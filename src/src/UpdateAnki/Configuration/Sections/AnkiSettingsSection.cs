namespace UpdateAnki.Configuration.Sections;

internal sealed record AnkiSettingsSection
{
    public string? RootDeckName { get; init; }

    public string? ModelNamePattern { get; init; }

    public IReadOnlyCollection<TranslationDirectionSection?>? TranslationDirections { get; init; }
}
