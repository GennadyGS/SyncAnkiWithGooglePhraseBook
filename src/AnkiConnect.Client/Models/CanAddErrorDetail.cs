namespace AnkiConnect.Client.Models;

public sealed record CanAddErrorDetail
{
    public required bool CanAdd { get; init; }

    public required string Error { get; init; }
}
