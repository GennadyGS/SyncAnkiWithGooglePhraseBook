using AnkiConnect.Client.Models;

namespace AnkiConnect.Client.Extensions;

public static class AnkiHttpClientExtensions
{
    extension(HttpClient httpClient)
    {
        public async Task<long[]> FindNotesAsync(string query)
        {
            var parameters = new FindNotesParams
            {
                Query = query,
            };

            return await httpClient.InvokeAnkiCommandAsync<FindNotesParams, long[]>(
                    AnkiCommands.FindNotes, parameters) ??
                throw new InvalidOperationException("Result cannot be null");
        }

        public async Task<NoteInfo[]> GetNotesInfoAsync(long[] noteIds)
        {
            var parameters = new GetNotesInfoParams
            {
                Notes = noteIds,
            };

            return await httpClient.InvokeAnkiCommandAsync<GetNotesInfoParams, NoteInfo[]>(
                    AnkiCommands.NotesInfo, parameters) ??
                throw new InvalidOperationException("Result cannot be null");
        }

        public async Task UpdateNoteFieldsAsync(
            long noteId,
            IReadOnlyDictionary<string, object?> fields)
        {
            var parameters = new UpdateNoteFieldsParams
            {
                Note = new()
                {
                    Id = noteId,
                    Fields = fields,
                },
            };

            await httpClient.InvokeAnkiCommandAsync<UpdateNoteFieldsParams, object>(
                AnkiCommands.UpdateNoteFields, parameters);
        }

        public async Task<CanAddErrorDetail[]> CanAddNotesWithErrorDetailAsync(
            IReadOnlyCollection<AddNoteParams> addNoteParams)
        {
            var parameters = new AddNotesParams
            {
                Notes = addNoteParams,
            };
            return await httpClient.InvokeAnkiCommandAsync<AddNotesParams, CanAddErrorDetail[]>(
                    AnkiCommands.CanAddNotesWithErrorDetail, parameters) ??
                throw new InvalidOperationException("Result cannot be null");
        }

        public async Task AddNotesAsync(IReadOnlyCollection<AddNoteParams> addNoteParams)
        {
            var parameters = new AddNotesParams
            {
                Notes = addNoteParams,
            };
            await httpClient.InvokeAnkiCommandAsync<AddNotesParams, object>(
                AnkiCommands.AddNotes, parameters);
        }

        public async Task DeleteNotesAsync(IReadOnlyCollection<long> noteIds)
        {
            var parameters = new DeleteNotesParams
            {
                Notes = noteIds.ToArray(),
            };

            await httpClient.InvokeAnkiCommandAsync<DeleteNotesParams, object>(
                AnkiCommands.DeleteNotes, parameters);
        }
    }
}
