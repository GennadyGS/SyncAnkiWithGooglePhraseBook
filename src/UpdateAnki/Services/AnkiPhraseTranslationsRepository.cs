using AnkiConnect.Client.Extensions;
using AnkiConnect.Client.Models;
using ChangeSetCalculation.Models;
using Common.Extensions;
using Newtonsoft.Json;
using Translation.Models;
using UpdateAnki.Extensions;
using UpdateAnki.Models;
using UpdateAnki.Utils;

namespace UpdateAnki.Services;

internal sealed class AnkiPhraseTranslationsRepository(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IReadOnlyDictionary<long, PhraseTranslation>> LoadPhraseTranslationsAsync(
        AnkiDeckSettings ankiSettings)
    {
        var query = GetSearchQuery(ankiSettings);
        var noteIds = await _httpClient.FindNotesAsync(query);
        var notesInfo = await _httpClient.GetNotesInfoAsync(noteIds);
        return notesInfo.ToDictionary(
            info => info.NoteId, info => info.ToPhraseTranslation(ankiSettings.ModelNamePattern));
    }

    public async Task UpdatePhraseTranslationsAsync(
        ChangeSet<PhraseTranslation, KeyValuePair<long, PhraseTranslation>> changeSet,
        AnkiDeckSettings ankiSettings)
    {
        var translationsToUpdate = changeSet.ToUpdate
            .Select(item => KeyValuePair.Create(item.Target.Key, item.Source))
            .ToList();
        await UpdatePhraseTranslationsAsync(translationsToUpdate);
        await AddPhraseTranslationsAsync(changeSet.ToAdd, ankiSettings);
        var translationsToDelete = changeSet.ToDelete
            .Select(kvp => kvp.Key)
            .ToList();
        await DeletePhraseTranslationsAsync(translationsToDelete);
    }

    private static string GetSearchQuery(AnkiDeckSettings ankiSettings)
    {
        var deckQuery = $"\"deck:{ankiSettings.DeckName}\"";
        var languagesQuery = ankiSettings.TranslationDirections
            .Select(td => GetModelNameQuery(td, ankiSettings.ModelNamePattern))
            .JoinStrings(" OR ");
        return $"{deckQuery} AND ({languagesQuery})";
    }

    private static string GetModelNameQuery(
        TranslationDirection translationDirection, string modelNamePattern)
    {
        var modelName =
            ModelNamePatternEngine.GenerateModelName(modelNamePattern, translationDirection);
        return $"\"note:{modelName}\"";
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
        var fields = new Dictionary<string, object?>
        {
            [NoteInfoFields.Front] = translation.Source.Text,
            [NoteInfoFields.Back] = translation.Target.Text,
        };

        await _httpClient.UpdateNoteFieldsAsync(noteId, fields);
    }

    private async Task AddPhraseTranslationsAsync(
        IReadOnlyCollection<PhraseTranslation> translations, AnkiDeckSettings ankiSettings)
    {
        var addNodeRequests = translations
            .Select(translation => translation.ToAddNoteParams(ankiSettings))
            .ToList();
        var results = await _httpClient.CanAddNotesWithErrorDetailAsync(addNodeRequests);
        if (results.Any(x => !x.CanAdd))
        {
            var errorDetails = results
                .Zip(addNodeRequests, (result, request) => (result, request))
                .Where(x => !x.result.CanAdd)
                .Select(x => new MatchedError<AddNoteParams>(x.request, x.result.Error))
                .ToList();
            var errorDetailsJson = JsonConvert.SerializeObject(errorDetails);
            throw new InvalidOperationException($"Cannot add some notes: '{errorDetailsJson}'");
        }

        await _httpClient.AddNotesAsync(addNodeRequests);
    }

    private async Task DeletePhraseTranslationsAsync(IReadOnlyCollection<long> noteIds)
    {
        await _httpClient.DeleteNotesAsync(noteIds);
    }

    private sealed record MatchedError<T>(T Parameter, string Error);
}
