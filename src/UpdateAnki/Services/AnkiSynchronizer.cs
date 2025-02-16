using UpdateAnki.Models;
using UpdateAnki.Utils;

namespace UpdateAnki.Services;

internal static class AnkiSynchronizer
{
    public static async Task SyncAnkiFromJsonFileAsync(
        string sourceFileName, AnkiSettings ankiSettings)
    {
        var sourcePhraseTranslations =
            await JsonPhraseTranslationsRepository.LoadPhraseTranslationsAsync(sourceFileName);
        var targetPhraseTranslations =
            await AnkiPhraseTranslationsRepository.LoadPhraseTranslationsAsync(ankiSettings);
        var updateActions = CollectionSynchronizer
            .GetUpdateActions(sourcePhraseTranslations, targetPhraseTranslations);
        await AnkiPhraseTranslationsRepository
            .UpdatePhraseTranslationsAsync(ankiSettings, updateActions);
    }
}
