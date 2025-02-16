using UpdateAnki.Models;

namespace UpdateAnki.Services;

internal sealed class AnkiPhraseTranslationsRepositoryFactory
{
    public AnkiPhraseTranslationsRepository CreateRepository(
        AnkiSettings ankiSettings) =>
        new(ankiSettings);
}
