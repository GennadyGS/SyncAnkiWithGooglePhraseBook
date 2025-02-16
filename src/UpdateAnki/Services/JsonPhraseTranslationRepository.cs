﻿using System.Text.Json;
using Translation.Models;

namespace UpdateAnki.Services;

internal static class JsonPhraseTranslationsRepository
{
    public static async Task<IReadOnlyCollection<PhraseTranslation>> LoadPhraseTranslationsAsync(
        string fileName)
    {
        var sourceFileContent = await File.ReadAllTextAsync(fileName);
        var options = new JsonSerializerOptions
        {
            RespectNullableAnnotations = true,
        };

        return JsonSerializer.Deserialize<PhraseTranslation[]>(sourceFileContent, options)!;
    }
}
