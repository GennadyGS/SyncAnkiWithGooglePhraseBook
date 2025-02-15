using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Translation.Models;
using UpdateAnki.Extensions;
using UpdateAnki.Models;

namespace UpdateAnki;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        var ankiSettings = configuration.GetSection("AnkiSettings")
            .Get<AnkiSettings>()
            .ThrowIfNull();
        var sourcePhraseTranslations = LoadPhraseTranslationsFromFile(args[0]);
        var currentTargetPhraseTranslations =
            await LoadPhraseTranslationsFromAnkiDeckAsync(ankiSettings);
    }

    private static PhraseTranslation[] LoadPhraseTranslationsFromFile(string fileName)
    {
        var sourceFileContent = File.ReadAllText(fileName);
        var options = new JsonSerializerOptions
        {
            RespectNullableAnnotations = true,
        };
        return JsonSerializer.Deserialize<PhraseTranslation[]>(sourceFileContent, options)!;
    }

    private static async Task<PhraseTranslation[]> LoadPhraseTranslationsFromAnkiDeckAsync(
        AnkiSettings ankiSettings)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = ankiSettings.AnkiConnectUri,
        };

        var noteIds = await httpClient.FindNotes($"\"deck:{ankiSettings.RootDeckName}\"");
        var notesInfo = await httpClient.GetNotesInfo(noteIds);
        return [];
    }
}