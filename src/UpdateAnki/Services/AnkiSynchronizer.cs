using UpdateAnki.Models;
using UpdateAnki.Utils;

namespace UpdateAnki.Services;

internal static class UpdateAnkiService
{
    public static async Task UpdateAnkiFromJsonFileAsync(
        string sourceFileName, AnkiSettings ankiSettings)
    {
        var sourcePhraseTranslations =
            await JsonPhraseTranslationsRepository.LoadPhraseTranslationsAsync(sourceFileName);
        var targetPhraseTranslations =
            await AnkiPhraseTranslationsRepository.LoadPhraseTranslationsAsync(ankiSettings);
        var updateActions = UpdateActionsCalculator
            .GetUpdateActions(sourcePhraseTranslations, targetPhraseTranslations);
        await AnkiPhraseTranslationsRepository
            .UpdatePhraseTranslationsAsync(ankiSettings, updateActions);
    }
}
