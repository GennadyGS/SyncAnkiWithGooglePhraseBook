using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UpdateAnki.Configuration;
using UpdateAnki.Extensions;
using UpdateAnki.Services;

namespace UpdateAnki;

internal static class Program
{
    private const int ErrorExitCode = 1;
    private const string ConfigurationFileName = "appsettings.json";

    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        try
        {
            return await RunCommandLineParserAsync(args);
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "An unhandled error occurred");
            return ErrorExitCode;
        }
    }

    private static async Task<int> RunCommandLineParserAsync(string[] args) =>
        (await Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsedAsync(async opt =>
            {
                await UpdateAnkiAsync(opt);
            }))
        .WithNotParsed(errors =>
        {
            foreach (var error in errors)
            {
                Log.Logger.Error("Command line parser error: {Error}", error);
            }
        })
        .MapResult(_ => 0, _ => 1);

    private static async Task UpdateAnkiAsync(CommandLineOptions commandLineOptions)
    {
        SetEnvironmentVariables(commandLineOptions);
        var configuration = LoadConfiguration();
        var ankiSettings = configuration.GetAnkiSettings();
        var updateAnkiService = CreateUpdateAnkiService(configuration);
        await updateAnkiService.UpdateAnkiFromJsonFileAsync(ankiSettings, commandLineOptions);
    }

    private static void SetEnvironmentVariables(CommandLineOptions commandLineOptions)
    {
        var logDirectoryPath = commandLineOptions.EstablishLogDirectoryPath();
        Environment.SetEnvironmentVariable(EnvVariables.LogPath, logDirectoryPath);
    }

    private static UpdateAnkiService CreateUpdateAnkiService(IConfiguration configuration)
    {
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
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(ConfigurationFileName, optional: false)
            .Build();

    private static class EnvVariables
    {
        public const string LogPath = nameof(LogPath);
    }
}
