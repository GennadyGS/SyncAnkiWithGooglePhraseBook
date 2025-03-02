namespace UpdateAnki.Models;

internal sealed record AnkiSettings
{
    public string? RootDeckName { get; init; }

    public string? ModelNamePattern { get; init; }
}
