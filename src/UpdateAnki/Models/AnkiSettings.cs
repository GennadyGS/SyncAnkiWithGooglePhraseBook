namespace UpdateAnki.Models;

internal sealed record AnkiSettings
{
    public Uri? AnkiConnectUri { get; init; }

    public string? RootDeckName { get; init; }

    public string? ModelNamePattern { get; init; }
}
