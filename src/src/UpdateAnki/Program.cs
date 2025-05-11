using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
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
        var updateAnkiService = CreateUpdateAnkiService(configuration);
        await updateAnkiService.UpdateAnkiFromJsonFileAsync(ankiSettings, fileName);
    }

    private static UpdateAnkiService CreateUpdateAnkiService(IConfiguration configuration)
    {
        Environment.SetEnvironmentVariable("BaseDirectory", AppContext.BaseDirectory);
        var ankiConnectSettings = configuration.GetAnkiConnectSettings();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        var serviceProvider = new ServiceCollection()
            .RegisterServices(ankiConnectSettings)
            .AddLogging(builder => builder.AddSerilog(dispose: true))
            .BuildServiceProvider();
        return serviceProvider.GetRequiredService<UpdateAnkiService>();
    }

    private static IConfigurationRoot LoadConfiguration() =>
        new ConfigurationBuilder()
            .AddJsonFile(ConfigurationFileName, optional: false)
            .Build();
}
