namespace UpdateAnki.Configuration.Sections;

internal sealed record AnkiDeckSettingsSection
{
    public string? DeckName { get; init; }

    public string? ModelNamePattern { get; init; }

    public IReadOnlyCollection<TranslationDirectionSection?>? TranslationDirections { get; init; }
}
