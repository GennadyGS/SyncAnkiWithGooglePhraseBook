using System.Text.RegularExpressions;
using ChangeSetCalculation.Models;
using Common.Extensions;
using Translation.Models;
using UpdateAnki.Constants;
using UpdateAnki.Extensions;
using UpdateAnki.Models;

namespace UpdateAnki.Services;

internal sealed class AnkiPhraseTranslationsRepository(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IReadOnlyDictionary<long, PhraseTranslation>> LoadPhraseTranslationsAsync(
        AnkiSettings ankiSettings)
    {
        var noteIds = await _httpClient.FindNotesAsync($"\"deck:{ankiSettings.RootDeckName}\"");
        var notesInfo = await _httpClient.GetNotesInfoAsync(noteIds);
        var modelNamePattern = Regex.Replace(
            ankiSettings.ModelNamePattern.ThrowIfNull(),
            "{(\\w+)}",
            "(?'$1'[a-z]{2})");
        var modelNameRegex = new Regex($"^{modelNamePattern}$", RegexOptions.Compiled);
        return notesInfo
            .ToDictionary(info => info.NoteId, info => info.ToPhraseTranslation(modelNameRegex));
    }

    public async Task UpdatePhraseTranslationsAsync(
        ChangeSet<PhraseTranslation, KeyValuePair<long, PhraseTranslation>> changeSet,
        AnkiSettings ankiSettings)
    {
        var translationsToUpdate = changeSet.ToUpdate
            .Select(item => KeyValuePair.Create(item.target.Key, item.source))
            .ToList();
        await UpdatePhraseTranslationsAsync(translationsToUpdate);
        await AddPhraseTranslationsAsync(changeSet.ToAdd, ankiSettings);
        var translationsToDelete = changeSet.ToDelete
            .Select(kvp => kvp.Key)
            .ToList();
        await DeletePhraseTranslationsAsync(translationsToDelete);
    }

    private async Task UpdatePhraseTranslationsAsync(
        IReadOnlyCollection<KeyValuePair<long, PhraseTranslation>> updates)
    {
        foreach (var (noteId, translation) in updates)
        {
            await UpdatePhraseTranslationAsync(noteId, translation);
        }
    }

    private async Task UpdatePhraseTranslationAsync(long noteId, PhraseTranslation translation)
    {
        var fields = new[]
        {
            KeyValuePair.Create(AnkiNoteFields.Front, (object?)translation.Source),
            KeyValuePair.Create(AnkiNoteFields.Back, (object?)translation.Target),
        };

        await _httpClient.UpdateNoteFieldsAsync(noteId, fields);
    }

    private async Task AddPhraseTranslationsAsync(
        IReadOnlyCollection<PhraseTranslation> translations, AnkiSettings ankiSettings)
    {
        var addNodeRequests = translations
            .Select(translation => translation.ToAddNoteParams(ankiSettings))
            .ToList();
        await _httpClient.AddNotesAsync(addNodeRequests);
    }

    private async Task DeletePhraseTranslationsAsync(IReadOnlyCollection<long> noteIds)
    {
        await _httpClient.DeleteNotesAsync(noteIds);
    }
}
