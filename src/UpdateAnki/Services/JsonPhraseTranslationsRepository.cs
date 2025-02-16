using System.Text.Json;
using Translation.Models;

namespace UpdateAnki.Services;

internal sealed class JsonPhraseTranslationsRepository(string fileName)
{
    private readonly string _fileName = fileName;

    public async Task<IReadOnlyCollection<PhraseTranslation>> LoadPhraseTranslationsAsync()
    {
        var sourceFileContent = await File.ReadAllTextAsync(_fileName);
        var options = new JsonSerializerOptions
        {
            RespectNullableAnnotations = true,
        };

        return JsonSerializer.Deserialize<PhraseTranslation[]>(sourceFileContent, options)!;
    }
}
