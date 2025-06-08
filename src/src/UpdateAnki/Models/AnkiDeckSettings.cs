using Translation.Models;

namespace UpdateAnki.Models;

internal sealed record AnkiDeckSettings
{
    public required string DeckName { get; init; }

    public required string ModelNamePattern { get; init; }

    public required IReadOnlyCollection<TranslationDirection> TranslationDirections { get; init; }
}
