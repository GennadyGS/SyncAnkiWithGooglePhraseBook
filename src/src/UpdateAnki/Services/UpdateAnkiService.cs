using ChangeSetCalculation;
using ChangeSetCalculation.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Translation.Models;
using UpdateAnki.Comparers;
using UpdateAnki.Models;
using UpdateAnki.Utils;

namespace UpdateAnki.Services;

internal sealed class UpdateAnkiService(
    AnkiPhraseTranslationsRepository ankiPhraseTranslationsRepository,
    JsonPhraseTranslationsReader jsonPhraseTranslationsReader,
    ILogger<UpdateAnkiService> logger)
{
    private readonly AnkiPhraseTranslationsRepository _ankiPhraseTranslationsRepository =
        ankiPhraseTranslationsRepository;

    private readonly JsonPhraseTranslationsReader _jsonPhraseTranslationsReader =
        jsonPhraseTranslationsReader;

    private readonly ILogger<UpdateAnkiService> _logger = logger;

    public async Task UpdateAnkiFromJsonFileAsync(AnkiSettings ankiSettings, string fileName)
    {
        var sourcePhraseTranslations = await _jsonPhraseTranslationsReader
            .LoadPhraseTranslationsAsync(fileName);
        var targetPhraseTranslations =
            await _ankiPhraseTranslationsRepository.LoadPhraseTranslationsAsync(ankiSettings);
        var changeSet = ChangeSetCalculator.CalculateChangeSet(
            sourcePhraseTranslations,
            targetPhraseTranslations,
            s => s,
            t => t.Value,
            deleteExcessMatched: true,
            deleteUnmatched: false,
            matchComparer: new PhraseTranslationMatchComparer(),
            keyDistanceProvider: new PhraseTranslationDistanceProvider());
        LogChangeSet(changeSet);
        await _ankiPhraseTranslationsRepository
            .UpdatePhraseTranslationsAsync(changeSet, ankiSettings);
    }

    private void LogChangeSet(
        ChangeSet<PhraseTranslation, KeyValuePair<long, PhraseTranslation>> changeSet)
    {
        var changeSetJson = JsonConvert.SerializeObject(changeSet, Formatting.Indented);
        _logger.LogDebug("ChangeSet: {ChangeSet}", changeSetJson);
    }
}
