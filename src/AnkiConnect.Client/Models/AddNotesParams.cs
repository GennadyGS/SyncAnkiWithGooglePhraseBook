namespace AnkiConnect.Client.Models;

internal sealed record AddNotesParams
{
    public required IReadOnlyCollection<AddNoteParams> Notes { get; init; }
}
