namespace UpdateAnki.Models;

internal sealed record GetNotesInfoParams
{
    public required long[] Notes { get; init; }
}
