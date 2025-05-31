namespace AnkiConnect.Client.Models;

public sealed record NoteInfo
{
    public required long NoteId { get; init; }

    public required string ModelName { get; init; }

    public required IReadOnlyDictionary<string, dynamic?> Fields { get; init; }
}
