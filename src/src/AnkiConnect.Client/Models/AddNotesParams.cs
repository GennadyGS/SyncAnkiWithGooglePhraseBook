namespace AnkiConnect.Client.Models;

internal sealed record AddNotesParams
{
    public required AddNoteParams[] Notes { get; init; }
}
