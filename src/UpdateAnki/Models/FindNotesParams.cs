namespace UpdateAnki.Models;

internal sealed record FindNotesParams
{
    public required string Query { get; init; }
}
