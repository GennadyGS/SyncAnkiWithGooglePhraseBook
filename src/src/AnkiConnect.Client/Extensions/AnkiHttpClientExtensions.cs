using AnkiConnect.Client.Models;
using Common.Extensions;
using Newtonsoft.Json;

namespace AnkiConnect.Client.Extensions;

public static class AnkiHttpClientExtensions
{
    private const string DummyDeckName = "Dummy deck name";
    private const string DummyModelName = "Dummy model name";

    public static async Task<long[]> FindNotesAsync(this HttpClient httpClient, string query)
    {
        var parameters = new FindNotesParams
        {
            Query = query,
        };

        return await httpClient.InvokeAnkiCommandAsync<FindNotesParams, long[]>(
                AnkiCommands.FindNotes, parameters) ??
            throw new InvalidOperationException("Result cannot be null");
    }

    public static async Task<NoteInfo[]> GetNotesInfoAsync(
        this HttpClient httpClient, long[] noteIds)
    {
        var parameters = new GetNotesInfoParams
        {
            Notes = noteIds,
        };

        return await httpClient.InvokeAnkiCommandAsync<GetNotesInfoParams, NoteInfo[]>(
                AnkiCommands.NotesInfo, parameters) ??
            throw new InvalidOperationException("Result cannot be null");
    }

    public static async Task UpdateNoteFieldsAsync(
        this HttpClient httpClient,
        long noteId,
        IReadOnlyDictionary<string, object?> fields)
    {
        var parameters = new UpdateNoteFieldsParams
        {
            Note = new NoteFieldsParams
            {
                Id = noteId,
                Fields = fields,
            },
        };

        await httpClient.InvokeAnkiCommandAsync<UpdateNoteFieldsParams, object>(
            AnkiCommands.UpdateNoteFields, parameters);
    }

    public static async Task AddNotesAsync(
        this HttpClient httpClient, IReadOnlyCollection<AddNoteParams> addNoteParams)
    {
        var dummyNoteParam = new AddNoteParams
        {
            DeckName = DummyDeckName,
            ModelName = DummyModelName,
            Fields = [],
        };
        var augmentedParams = addNoteParams
            .SelectMany(noteParam => new[] { noteParam, dummyNoteParam })
            .ToArray();
        var parameters = new AddNotesParams
        {
            Notes = augmentedParams,
        };
        var ankiResponse = await httpClient.PostAnkiCommandAsync<AddNotesParams, object>(
            AnkiCommands.AddNotes, parameters);
        if (ankiResponse.Error is null)
        {
            return;
        }

        var errors = JsonConvert.DeserializeObject<string[]>(
            ankiResponse.Error.Replace("'", "\"")).ThrowIfNull();
        var matchedErrors = GetMatchedErrors(addNoteParams, errors);
        throw new AnkiException(JsonConvert.SerializeObject(matchedErrors));
    }

    public static async Task DeleteNotesAsync(
        this HttpClient httpClient, IReadOnlyCollection<long> noteIds)
    {
        var parameters = new DeleteNotesParams
        {
            Notes = noteIds.ToArray(),
        };

        await httpClient.InvokeAnkiCommandAsync<DeleteNotesParams, object>(
            AnkiCommands.DeleteNotes, parameters);
    }

    private static IEnumerable<MatchedError<T>> GetMatchedErrors<T>(
        IEnumerable<T> parameters, IEnumerable<string> errors)
    {
        using var parametersEnumerator = parameters.GetEnumerator();
        parametersEnumerator.MoveNext();
        foreach (var error in errors)
        {
            if (error.Contains(DummyModelName))
            {
                parametersEnumerator.MoveNext();
            }
            else
            {
                yield return new MatchedError<T>(parametersEnumerator.Current, error);
            }
        }
    }

    private sealed record MatchedError<T>(T Parameter, string Error);
}
