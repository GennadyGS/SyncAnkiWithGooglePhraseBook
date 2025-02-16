using System.Text.RegularExpressions;
using Translation.Models;
using UpdateAnki.Extensions;
using UpdateAnki.Models;

namespace UpdateAnki.Services;

internal sealed class AnkiPhraseTranslationsRepository(AnkiSettings ankiSettings)
{
    private readonly AnkiSettings _ankiSettings = ankiSettings;

    public async Task<IDictionary<long, PhraseTranslation>> LoadPhraseTranslationsAsync()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = _ankiSettings.AnkiConnectUri,
        };

        var noteIds = await httpClient.FindNotesAsync($"\"deck:{_ankiSettings.RootDeckName}\"");
        var notesInfo = await httpClient.GetNotesInfoAsync(noteIds);
        var modelNamePattern = _ankiSettings.ModelNamePattern.ThrowIfNull();
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
