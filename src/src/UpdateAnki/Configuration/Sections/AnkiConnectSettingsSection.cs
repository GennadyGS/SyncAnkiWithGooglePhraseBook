namespace UpdateAnki.Configuration.Sections;

internal sealed record AnkiConnectSettingsSection
{
    public Uri? Uri { get; init; }
}
