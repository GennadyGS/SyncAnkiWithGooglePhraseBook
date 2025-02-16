using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UpdateAnki.Extensions;
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
        var updateAnkiServiceFactory = CreateUpdateAnkiServiceFactory();
        var updateAnkiService = updateAnkiServiceFactory.CreateService(fileName, ankiSettings);
        await updateAnkiService.UpdateAnkiFromJsonFileAsync();
    }

    private static UpdateAnkiServiceFactory CreateUpdateAnkiServiceFactory()
    {
        var services = new ServiceCollection();
        var serviceProvider = ServiceConfigurator
            .RegisterServices(services)
            .BuildServiceProvider();

        return serviceProvider.GetRequiredService<UpdateAnkiServiceFactory>();
    }

    private static IConfigurationRoot LoadConfiguration() =>
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(ConfigurationFileName, optional: false)
            .Build();
}
