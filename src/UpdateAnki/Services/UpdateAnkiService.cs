using UpdateAnki.Models;
using UpdateAnki.Utils;

namespace UpdateAnki.Services;

internal sealed class UpdateAnkiService(string sourceFileName, AnkiSettings ankiSettings)
{
    private readonly string _sourceFileName = sourceFileName;

    private readonly AnkiSettings _ankiSettings = ankiSettings;

    private readonly JsonPhraseTranslationsRepositoryFactory _jsonPhraseTranslationsRepositoryFactory =
        new();

    private readonly AnkiPhraseTranslationsRepositoryFactory _ankiPhraseTranslationsRepositoryFactory =
        new();

    public async Task UpdateAnkiFromJsonFileAsync()
    {
        var ankiPhraseTranslationsRepository =
            _ankiPhraseTranslationsRepositoryFactory.CreateRepository(_ankiSettings);
        var jsonPhraseTranslationsRepository =
            _jsonPhraseTranslationsRepositoryFactory.CreateRepository(_sourceFileName);
        var sourcePhraseTranslations = await jsonPhraseTranslationsRepository
            .LoadPhraseTranslationsAsync();
        var targetPhraseTranslations =
            await ankiPhraseTranslationsRepository.LoadPhraseTranslationsAsync();
        var updateActions = UpdateActionsCalculator
            .GetUpdateActions(sourcePhraseTranslations, targetPhraseTranslations);
        await ankiPhraseTranslationsRepository.UpdatePhraseTranslationsAsync(updateActions);
    }
}
