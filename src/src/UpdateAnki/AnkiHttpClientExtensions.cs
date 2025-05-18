using UpdateAnki.Constants;
using UpdateAnki.Extensions;
using UpdateAnki.Models;

namespace UpdateAnki;

internal static class AnkiHttpClientExtensions
{
    public static async Task<long[]> FindNotesAsync(this HttpClient httpClient, string query)
    {
        var parameters = new FindNotesParams
        {
            Query = query,
        };

        return await httpClient
            .InvokeAnkiCommandAsync<FindNotesParams, long[]>(AnkiCommands.FindNotes, parameters) ??
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
                AnkiCommands.NotesInfo, parameters)
            ?? throw new InvalidOperationException("Result cannot be null");
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
        var parameters = new AddNotesParams
        {
            Notes = addNoteParams.ToArray(),
        };

        await httpClient.InvokeAnkiCommandAsync<AddNotesParams, object>(
            AnkiCommands.AddNotes, parameters);
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
}
