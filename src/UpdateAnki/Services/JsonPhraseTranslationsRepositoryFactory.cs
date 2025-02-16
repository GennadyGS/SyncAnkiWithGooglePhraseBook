namespace UpdateAnki.Services;

internal sealed class JsonPhraseTranslationsRepositoryFactory
{
    public JsonPhraseTranslationsRepository CreateRepository(string sourceFileName) =>
        new(sourceFileName);
}
