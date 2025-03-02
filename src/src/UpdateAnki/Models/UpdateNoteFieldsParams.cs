namespace UpdateAnki.Models;

internal sealed record UpdateNoteFieldsParams
{
    public required long NoteId { get; init; }

    public required IReadOnlyCollection<KeyValuePair<string, object?>> Fields { get; init; }
}
