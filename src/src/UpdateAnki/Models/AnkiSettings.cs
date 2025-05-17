using Translation.Models;

namespace UpdateAnki.Models;

internal sealed record AnkiSettings
{
    public required string RootDeckName { get; init; }

    public required string ModelNamePattern { get; init; }

    public required IReadOnlyCollection<TranslationDirection> TranslationDirections { get; init; }
}
