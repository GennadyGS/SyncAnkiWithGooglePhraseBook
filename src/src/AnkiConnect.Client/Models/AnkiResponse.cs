namespace AnkiConnect.Client.Models;

internal sealed record AnkiResponse<TResult>
{
    public TResult? Result { get; init; }

    public string? Error { get; init; }
}
