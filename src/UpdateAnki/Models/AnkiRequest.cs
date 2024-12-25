namespace UpdateAnki.Models;

internal sealed record AnkiRequest<TParams>
{
    public required string Action { get; init; }

    public required int Version { get; init; }
    
    public required TParams Params { get; init; }
}