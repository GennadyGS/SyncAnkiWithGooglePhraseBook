using UpdateAnki.Models;
using UpdateAnki.Utils;

namespace UpdateAnki.Services;

internal sealed class UpdateAnkiService
{
    public async Task UpdateAnkiFromJsonFileAsync(
        string sourceFileName, AnkiSettings ankiSettings)
    {
        var ankiPhraseTranslationsRepository = new AnkiPhraseTranslationsRepository(ankiSettings);
        var jsonPhraseTranslationsRepository = new JsonPhraseTranslationsRepository(sourceFileName);
        var sourcePhraseTranslations = await jsonPhraseTranslationsRepository
            .LoadPhraseTranslationsAsync();
        var targetPhraseTranslations =
            await ankiPhraseTranslationsRepository.LoadPhraseTranslationsAsync();
        var updateActions = UpdateActionsCalculator
            .GetUpdateActions(sourcePhraseTranslations, targetPhraseTranslations);
        await ankiPhraseTranslationsRepository.UpdatePhraseTranslationsAsync(updateActions);
    }
}
