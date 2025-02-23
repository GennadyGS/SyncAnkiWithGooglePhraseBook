namespace UpdateAnki.Models;

internal sealed record AddNotesParams
{
    public required AddNoteParams[] Notes { get; init; }
}
