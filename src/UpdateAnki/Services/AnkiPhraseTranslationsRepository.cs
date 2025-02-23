using System.Text.RegularExpressions;
using Translation.Models;
using UpdateAnki.Extensions;
using UpdateAnki.Models;

namespace UpdateAnki.Services;

internal sealed class AnkiPhraseTranslationsRepository(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IDictionary<long, PhraseTranslation>> LoadPhraseTranslationsAsync(
        AnkiSettings ankiSettings)
    {
        var noteIds = await _httpClient.FindNotesAsync($"\"deck:{ankiSettings.RootDeckName}\"");
        var notesInfo = await _httpClient.GetNotesInfoAsync(noteIds);
        var modelNamePattern = ankiSettings.ModelNamePattern.ThrowIfNull();
        var modelNameRegex = new Regex($"^{modelNamePattern}$", RegexOptions.Compiled);
        return notesInfo
            .ToDictionary(info => info.NoteId, info => info.ToPhraseTranslation(modelNameRegex));
    }

    public async Task UpdatePhraseTranslationsAsync(
        UpdateActions<long, PhraseTranslation> updateActions)
    {
        await Task.CompletedTask;
    }
}
