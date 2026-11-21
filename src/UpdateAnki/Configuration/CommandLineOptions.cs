using CommandLine;

namespace UpdateAnki.Configuration;

internal sealed record CommandLineOptions
{
    [Option('i', "input", HelpText = "Input file path", Required = true)]
    public required string InputFilePath { get; init; }

    [Option('l', "logPath", HelpText = "Log directory path")]
    public string? LogDirectoryPath { get; init; }

    [Option("what-if", HelpText = "What if")]
    public bool WhatIf { get; init; }
}
