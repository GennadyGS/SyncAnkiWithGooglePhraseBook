using UpdateAnki.Configuration;

namespace UpdateAnki.Extensions;

internal static class CommandLineOptionsExtensions
{
    private const string DefaultLogDirectoryName = "logs";

    extension(CommandLineOptions commandLineOptions)
    {
        public string GetFullLogFilePath(string filePath) =>
            Path.Combine(commandLineOptions.EstablishLogDirectoryPath(), filePath);

        public string EstablishLogDirectoryPath() =>
            commandLineOptions.LogDirectoryPath ??
            Path.Combine(AppContext.BaseDirectory, DefaultLogDirectoryName);
    }
}
