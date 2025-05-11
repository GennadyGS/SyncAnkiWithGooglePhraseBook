using CommandLine;

namespace UpdateAnki.Configuration;

internal sealed class CommandLineOptions
{
    [Option('i', "input", HelpText = "Input file path", Required = true)]
    public string InputFilePath { get; init; } = string.Empty;

    [Option("what-if", HelpText = "What if")]
    public bool WhatIf { get; init; }
}
