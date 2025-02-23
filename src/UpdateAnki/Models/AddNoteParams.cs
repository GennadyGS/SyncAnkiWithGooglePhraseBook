namespace UpdateAnki.Models;

internal sealed record AddNoteParams
{
    public required string DeckName { get; init; }

    public required string ModelName { get; init; }

    public IReadOnlyCollection<KeyValuePair<string, object?>> Fields { get; init; } = [];
}
