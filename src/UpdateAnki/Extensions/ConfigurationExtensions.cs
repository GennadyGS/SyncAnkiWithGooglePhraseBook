using Microsoft.Extensions.Configuration;
using UpdateAnki.Models;

namespace UpdateAnki.Extensions;

internal static class ConfigurationExtensions
{
    public static AnkiSettings GetAnkiSettings(this IConfiguration configuration) =>
        configuration.GetSection("AnkiSettings")
            .Get<AnkiSettings>()
            .ThrowIfNull();
}
