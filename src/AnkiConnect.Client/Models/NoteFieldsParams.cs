namespace AnkiConnect.Client.Models;

internal sealed record NoteFieldsParams
{
    public required long Id { get; init; }

    public required IReadOnlyCollection<KeyValuePair<string, object?>> Fields { get; init; }
}
