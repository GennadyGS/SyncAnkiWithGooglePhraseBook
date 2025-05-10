using ChangeSetCalculation;
using Microsoft.Extensions.Logging;
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
        _logger.LogDebug("ChangeSet: {ChangeSet}", changeSet);
        await _ankiPhraseTranslationsRepository
            .UpdatePhraseTranslationsAsync(changeSet, ankiSettings);
    }
}
