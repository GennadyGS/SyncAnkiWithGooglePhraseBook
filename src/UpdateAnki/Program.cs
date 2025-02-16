using Microsoft.Extensions.Configuration;
using UpdateAnki.Extensions;
using UpdateAnki.Services;

namespace UpdateAnki;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var fileName = args[0];
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        var ankiSettings = configuration.GetAnkiSettings();
        await UpdateAnkiService.UpdateAnkiFromJsonFileAsync(fileName, ankiSettings);
    }
}
