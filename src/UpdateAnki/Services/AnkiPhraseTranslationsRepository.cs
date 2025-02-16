using System.Text.RegularExpressions;
using Translation.Models;
using UpdateAnki.Extensions;
using UpdateAnki.Models;

namespace UpdateAnki.Services;

internal static class AnkiPhraseTranslationsRepository
{
    public static async Task<IDictionary<long, PhraseTranslation>>
        LoadPhraseTranslationsAsync(AnkiSettings ankiSettings)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = ankiSettings.AnkiConnectUri,
        };

        var noteIds = await httpClient.FindNotesAsync($"\"deck:{ankiSettings.RootDeckName}\"");
        var notesInfo = await httpClient.GetNotesInfoAsync(noteIds);
        var modelNamePattern = ankiSettings.ModelNamePattern.ThrowIfNull();
        var modelNameRegex = new Regex($"^{modelNamePattern}$", RegexOptions.Compiled);
        return notesInfo
            .ToDictionary(info => info.NoteId, info => info.ToPhraseTranslation(modelNameRegex));
    }

    public static async Task UpdatePhraseTranslationsAsync(
        AnkiSettings ankiSettings, UpdateActions<long, PhraseTranslation> updateActions)
    {
        await Task.CompletedTask;
    }
}
