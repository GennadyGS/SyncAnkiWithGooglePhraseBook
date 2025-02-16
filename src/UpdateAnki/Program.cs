using Microsoft.Extensions.Configuration;
using UpdateAnki.Extensions;
using UpdateAnki.Services;
using UpdateAnki.Utils;

namespace UpdateAnki;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var fileName = args[0];
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        var ankiSettings = configuration.GetAnkiSettings();
        var sourcePhraseTranslations =
            await JsonPhraseTranslationRepository.LoadPhraseTranslationsAsync(fileName);
        var currentTargetPhraseTranslations = await AnkiPhraseTranslationsRepository
            .LoadPhraseTranslationsFromAnkiAsync(ankiSettings);
        var updateActions = CollectionSynchronizer
            .GetUpdateActions(sourcePhraseTranslations, currentTargetPhraseTranslations);
        await AnkiPhraseTranslationsRepository
            .UpdatePhraseTranslationsAsync(ankiSettings, updateActions);
    }
}
