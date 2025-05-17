using Common.Extensions;
using Microsoft.Extensions.Configuration;
using UpdateAnki.Configuration.Sections;
using UpdateAnki.Constants;
using UpdateAnki.Models;

namespace UpdateAnki.Extensions;

internal static class ConfigurationExtensions
{
    public static AnkiSettings GetAnkiSettings(this IConfiguration configuration) =>
        configuration
            .GetSectionSafe<AnkiSettingsSection>(ConfigurationSectionNames.AnkiSettings)
            .ToAnkiSettings();

    public static AnkiConnectSettings GetAnkiConnectSettings(this IConfiguration configuration)
    {
        var section = configuration.GetSectionSafe<AnkiConnectSettingsSection>(
            ConfigurationSectionNames.AnkiConnectSettings);
        return new AnkiConnectSettings
        {
            Uri = section.Uri.ThrowIfNull(),
        };
    }

    private static T GetSectionSafe<T>(this IConfiguration configuration, string key) =>
        configuration
            .GetSection(key)
            .Get<T>(opt => opt.ErrorOnUnknownConfiguration = true)
        ?? throw new InvalidOperationException($"Section {key} is missing in configuration.");

    private static AnkiSettings ToAnkiSettings(this AnkiSettingsSection section) =>
        new()
        {
            RootDeckName = section.RootDeckName.ThrowIfNull(),
            ModelNamePattern = section.ModelNamePattern.ThrowIfNull(),
            TranslationDirections =
                section.TranslationDirections.ThrowIfNull().ToTranslationDirections(),
        };

    private static IReadOnlyCollection<TranslationDirection> ToTranslationDirections(
        this IReadOnlyCollection<TranslationDirectionSection?> sections) =>
        sections
            .Select(td => td.ThrowIfNull())
            .Select(td => td.ToTranslationDirection())
            .ToList();

    private static TranslationDirection ToTranslationDirection(
        this TranslationDirectionSection section) =>
        new()
        {
            SourceLanguage = section.SourceLanguage.ThrowIfNull(),
            TargetLanguage = section.TargetLanguage.ThrowIfNull(),
        };
}
