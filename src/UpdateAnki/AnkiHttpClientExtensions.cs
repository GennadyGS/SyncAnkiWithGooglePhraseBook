﻿using System.Text;
using UpdateAnki.Extensions;
using UpdateAnki.Models;

namespace UpdateAnki;

internal static class AnkiHttpClientExtensions
{
    public static async Task<long[]> FindNotes(this HttpClient httpClient, string query)
    {
        var parameters = new FindNotesParams
        {
            Query = query,
        };

        return await httpClient
            .InvokeAnkiCommandAsync<FindNotesParams, long[]>("findNotes", parameters);
    }

    public static async Task<NoteInfo[]> GetNotesInfo(this HttpClient httpClient, long[] noteIds)
    {
        var parameters = new GetNotesInfoParams
        {
            Notes = noteIds,
        };

        return await httpClient
            .InvokeAnkiCommandAsync<GetNotesInfoParams, NoteInfo[]>("notesInfo", parameters);
    }
}