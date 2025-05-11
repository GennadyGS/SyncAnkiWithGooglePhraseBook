using ChangeSetCalculation;
using ChangeSetCalculation.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Translation.Models;
using UpdateAnki.Comparers;
using UpdateAnki.Configuration;
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

    public async Task UpdateAnkiFromJsonFileAsync(
        AnkiSettings ankiSettings, CommandLineOptions commandLineOptions)
    {
        var sourcePhraseTranslations = await _jsonPhraseTranslationsReader
            .LoadPhraseTranslationsAsync(commandLineOptions.InputFilePath);
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
        if (!commandLineOptions.WhatIf)
        {
            await _ankiPhraseTranslationsRepository
                .UpdatePhraseTranslationsAsync(changeSet, ankiSettings);
        }
        else
        {
            _logger.LogInformation("WhatIf mode: no changes will be applied.");
        }
    }

    private void LogChangeSet(
        ChangeSet<PhraseTranslation, KeyValuePair<long, PhraseTranslation>> changeSet)
    {
        var changeSetJson = JsonConvert.SerializeObject(changeSet);
        _logger.LogDebug("ChangeSet: {ChangeSet}", changeSetJson);
    }
}
