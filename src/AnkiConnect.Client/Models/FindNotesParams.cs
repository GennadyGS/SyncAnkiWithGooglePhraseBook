namespace AnkiConnect.Client.Models;

internal sealed record FindNotesParams
{
    public required string Query { get; init; }
}
