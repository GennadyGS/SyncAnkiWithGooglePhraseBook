using Microsoft.Extensions.Configuration;
using UpdateAnki.Constants;
using UpdateAnki.Models;

namespace UpdateAnki.Extensions;

internal static class ConfigurationExtensions
{
    public static AnkiSettings GetAnkiSettings(this IConfiguration configuration) =>
        configuration.GetSection(ConfigurationSectionNames.AnkiSettings)
            .Get<AnkiSettings>()
            .ThrowIfNull();

    public static AnkiConnectSettings GetAnkiConnectSettings(this IConfiguration configuration) =>
        configuration.GetSection(ConfigurationSectionNames.AnkiConnectSettings)
            .Get<AnkiConnectSettings>()
            .ThrowIfNull();
}
