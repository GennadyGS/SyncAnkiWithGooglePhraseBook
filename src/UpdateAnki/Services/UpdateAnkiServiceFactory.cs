using UpdateAnki.Models;

namespace UpdateAnki.Services;

internal sealed class UpdateAnkiServiceFactory
{
    public UpdateAnkiService CreateService(string fileName, AnkiSettings ankiSettings) =>
        new(fileName, ankiSettings);
}
