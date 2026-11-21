namespace AnkiConnect.Client.Models;

internal sealed record GetNotesInfoParams
{
    public required long[] Notes { get; init; }
}
