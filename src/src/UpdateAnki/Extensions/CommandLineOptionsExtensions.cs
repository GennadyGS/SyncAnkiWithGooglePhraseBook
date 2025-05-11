using UpdateAnki.Configuration;

namespace UpdateAnki.Extensions;

internal static class CommandLineOptionsExtensions
{
    private const string DefaultLogDirectoryName = "logs";

    public static string GetFullLogFilePath(
        this CommandLineOptions commandLineOptions, string filePath) =>
        Path.Combine(commandLineOptions.EstablishLogDirectoryPath(), filePath);

    public static string EstablishLogDirectoryPath(this CommandLineOptions commandLineOptions) =>
        commandLineOptions.LogDirectoryPath ??
        Path.Combine(AppContext.BaseDirectory, DefaultLogDirectoryName);
}
