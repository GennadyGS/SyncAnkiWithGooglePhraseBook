namespace UpdateAnki.Models;

internal sealed record AnkiConnectSettings
{
    public required Uri Uri { get; init; }
}
