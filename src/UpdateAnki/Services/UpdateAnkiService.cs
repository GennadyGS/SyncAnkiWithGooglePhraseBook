using UpdateAnki.Models;
using UpdateAnki.Utils;

namespace UpdateAnki.Services;

internal sealed class UpdateAnkiService
{
    public async Task UpdateAnkiFromJsonFileAsync(
        string sourceFileName, AnkiSettings ankiSettings)
    {
        var ankiPhraseTranslationsRepository = new AnkiPhraseTranslationsRepository();
        var jsonPhraseTranslationsRepository = new JsonPhraseTranslationsRepository();
        var sourcePhraseTranslations = await jsonPhraseTranslationsRepository
            .LoadPhraseTranslationsAsync(sourceFileName);
        var targetPhraseTranslations =
            await ankiPhraseTranslationsRepository.LoadPhraseTranslationsAsync(ankiSettings);
        var updateActions = UpdateActionsCalculator
            .GetUpdateActions(sourcePhraseTranslations, targetPhraseTranslations);
        await ankiPhraseTranslationsRepository
            .UpdatePhraseTranslationsAsync(ankiSettings, updateActions);
    }
}
