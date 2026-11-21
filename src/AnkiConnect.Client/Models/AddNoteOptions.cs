namespace AnkiConnect.Client.Models;

public sealed record AddNoteOptions
{
    public required bool AllowDuplicate { get; init; }
}
