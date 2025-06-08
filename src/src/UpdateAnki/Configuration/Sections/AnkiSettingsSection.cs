namespace UpdateAnki.Configuration.Sections;

internal sealed record AnkiSettingsSection
{
    public string? DeckName { get; init; }

    public string? ModelNamePattern { get; init; }

    public IReadOnlyCollection<TranslationDirectionSection?>? TranslationDirections { get; init; }
}
