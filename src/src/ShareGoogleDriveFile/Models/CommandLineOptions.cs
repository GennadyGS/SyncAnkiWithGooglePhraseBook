using CommandLine;

namespace ShareGoogleDriveFile.Models;

internal sealed class CommandLineOptions
{
    [Option('i', "fileId", HelpText = "Input File ID.", Required = true)]
    public required string FileId { get; init; }

    [Option('u', "userEmail", HelpText = "User Email.", Required = true)]
    public required string UserEmail { get; init; }
}
