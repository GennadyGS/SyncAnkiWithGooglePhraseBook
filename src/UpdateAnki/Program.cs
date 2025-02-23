using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UpdateAnki.Extensions;
using UpdateAnki.Models;
using UpdateAnki.Services;

namespace UpdateAnki;

internal static class Program
{
    private const string ConfigurationFileName = "appsettings.json";

    public static async Task Main(string[] args)
    {
        var fileName = args[0];
        var configuration = LoadConfiguration();
        var ankiSettings = configuration.GetAnkiSettings();
        var ankiConnectSettings = configuration.GetAnkiConnectSettings();
        var updateAnkiService = CreateUpdateAnkiService(ankiConnectSettings);
        await updateAnkiService.UpdateAnkiFromJsonFileAsync(ankiSettings, fileName);
    }

    private static UpdateAnkiService CreateUpdateAnkiService(
        AnkiConnectSettings ankiConnectSettings)
    {
        var serviceProvider = new ServiceCollection()
            .RegisterServices(ankiConnectSettings)
            .BuildServiceProvider();
        return serviceProvider.GetRequiredService<UpdateAnkiService>();
    }

    private static IConfigurationRoot LoadConfiguration() =>
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(ConfigurationFileName, optional: false)
            .Build();
}
