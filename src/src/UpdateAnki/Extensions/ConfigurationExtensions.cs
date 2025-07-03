using Common.Extensions;
using Microsoft.Extensions.Configuration;
using Translation.Models;
using UpdateAnki.Configuration.Sections;
using UpdateAnki.Constants;
using UpdateAnki.Models;

namespace UpdateAnki.Extensions;

internal static class ConfigurationExtensions
{
    public static AnkiSettings GetAnkiSettings(this IConfiguration configuration) =>
        configuration.GetSectionSafe<AnkiSettingsSection>(ConfigurationSectionNames.AnkiSettings)
            .ToAnkiSettings();

    public static AnkiConnectSettings GetAnkiConnectSettings(this IConfiguration configuration)
    {
        var section = configuration.GetSectionSafe<AnkiConnectSettingsSection>(
            ConfigurationSectionNames.AnkiConnectSettings);
        return new AnkiConnectSettings { Uri = section.Uri.ThrowIfNull(), };
    }

    private static T GetSectionSafe<T>(this IConfiguration configuration, string key) =>
        configuration.GetSection(key).Get<T>(opt => opt.ErrorOnUnknownConfiguration = true) ??
        throw new InvalidOperationException($"Section {key} is missing in configuration.");

    private static AnkiSettings ToAnkiSettings(this AnkiSettingsSection section)
    {
        var ankiDeckSettings = section
            .SelectMany(deckSection => deckSection
                .ThrowIfNull()
                .ToAnkiDeckSettingsRecursive(new AnkiDeckSettingsSection()))
            .ToList();
        return new(ankiDeckSettings);
    }

    private static IReadOnlyCollection<AnkiDeckSettings> ToAnkiDeckSettingsRecursive(
        this AnkiDeckSettingsSection section, AnkiDeckSettingsSection context)
    {
        var currentContext = new AnkiDeckSettingsSection
        {
            DeckName = CombineDeckNames(section.DeckName, context.DeckName),
            ModelNamePattern = section.ModelNamePattern ?? context.ModelNamePattern,
            TranslationDirections =
            [
                .. section.TranslationDirections ?? [],
                .. context.TranslationDirections ?? [],
            ],
        };
        if ((section.ChildSettings ?? []).Count == 0)
        {
            return [ToAnkiDeckSettings(currentContext),];
        }

        return section.ChildSettings!
            .SelectMany(child => child.ThrowIfNull().ToAnkiDeckSettingsRecursive(currentContext))
            .ToList();
    }

    private static AnkiDeckSettings ToAnkiDeckSettings(
        AnkiDeckSettingsSection section) =>
        new()
        {
            DeckName = section.DeckName.ThrowIfNull(),
            ModelNamePattern = section.ModelNamePattern.ThrowIfNull(),
            TranslationDirections =
                section.TranslationDirections.ThrowIfNull().ToTranslationDirections(),
        };

    private static string? CombineDeckNames(string? sectionDeckName, string? contextDeckName) =>
        (contextDeckName ?? string.Empty) + (sectionDeckName ?? string.Empty);

    private static IReadOnlyCollection<TranslationDirection> ToTranslationDirections(
        this IReadOnlyCollection<TranslationDirectionSection?> sections) =>
        sections.Select(section => section.ThrowIfNull().ToTranslationDirection()).ToList();

    private static TranslationDirection ToTranslationDirection(
        this TranslationDirectionSection section) =>
        new()
        {
            SourceLanguageCode = section.SourceLanguageCode.ThrowIfNull(),
            TargetLanguageCode = section.TargetLanguageCode.ThrowIfNull(),
        };
}
