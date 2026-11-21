namespace AnkiConnect.Client.Models;

internal sealed record DeleteNotesParams
{
    public required long[] Notes { get; init; }
}
