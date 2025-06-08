using ChangeSetCalculation.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Translation.ChangeSetCalculation;
using Translation.Models;
using UpdateAnki.Comparers;
using UpdateAnki.Configuration;
using UpdateAnki.Extensions;
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
        AnkiDeckSettings ankiSettings, CommandLineOptions commandLineOptions)
    {
        if (commandLineOptions.WhatIf)
        {
            _logger.LogInformation("Running in what-if mode. No changes will be applied.");
        }

        var sourcePhraseTranslations = await _jsonPhraseTranslationsReader
            .LoadPhraseTranslationsAsync(commandLineOptions.InputFilePath);
        var relevantSourcePhraseTranslations = sourcePhraseTranslations
            .Where(tr => WithinDirections(tr, ankiSettings.TranslationDirections))
            .ToList();
        var targetPhraseTranslations =
            await LoadAndDumpAnkiPhraseTranslationsAsync(ankiSettings, commandLineOptions);
        var changeSet = PhraseTranslationChangeSetCalculator
            .CalculateChangeSet(relevantSourcePhraseTranslations, targetPhraseTranslations);
        await UpdateAndDumpPhraseTranslationsAsync(changeSet, ankiSettings, commandLineOptions);
    }

    private static bool WithinDirections(
        PhraseTranslation translation,
        IReadOnlyCollection<TranslationDirection> translationDirections) =>
        translationDirections.Contains(
            translation.GetDirection(), new TranslationDirectionEqualityComparer());

    private async Task<IReadOnlyDictionary<long, PhraseTranslation>>
        LoadAndDumpAnkiPhraseTranslationsAsync(
            AnkiDeckSettings ankiSettings, CommandLineOptions commandLineOptions)
    {
        var result =
            await _ankiPhraseTranslationsRepository.LoadPhraseTranslationsAsync(ankiSettings);
        DumpUtils.DumpObject(
            result, FileNames.AnkiPhraseTranslations, ankiSettings.DeckName, commandLineOptions);
        return result;
    }

    private async Task UpdateAndDumpPhraseTranslationsAsync(
        ChangeSet<PhraseTranslation, KeyValuePair<long, PhraseTranslation>> changeSet,
        AnkiDeckSettings ankiSettings,
        CommandLineOptions commandLineOptions)
    {
        _logger.LogDebug("ChangeSet: {ChangeSet}", JsonConvert.SerializeObject(changeSet));
        DumpUtils.DumpObject(changeSet, FileNames.ChangeSet, ankiSettings.DeckName, commandLineOptions);
        if (!commandLineOptions.WhatIf)
        {
            await _ankiPhraseTranslationsRepository
                .UpdatePhraseTranslationsAsync(changeSet, ankiSettings);
        }
    }

    private static class FileNames
    {
        public const string AnkiPhraseTranslations = nameof(AnkiPhraseTranslations);

        public const string ChangeSet = nameof(ChangeSet);
    }
}
