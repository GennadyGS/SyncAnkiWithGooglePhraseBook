namespace UpdateAnki.Models;

internal sealed record UpdateNoteFieldsParams
{
    public required NoteFieldsParams Note { get; init; }
}
