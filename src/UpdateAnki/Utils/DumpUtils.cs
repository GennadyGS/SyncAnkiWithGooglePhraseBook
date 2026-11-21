using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UpdateAnki.Configuration;
using UpdateAnki.Extensions;

namespace UpdateAnki.Utils;

internal static class DumpUtils
{
    public static void DumpObject<T>(
        T obj, string baseFileName, string suffix, CommandLineOptions commandLineOptions)
    {
        var normalizedSuffix = Regex.Replace(suffix, "\\W+", "_");
        var fileName =
            $"{baseFileName}_{normalizedSuffix}_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.json";
        var dumpFilePath = commandLineOptions.GetFullLogFilePath(fileName);
        var content = JsonConvert.SerializeObject(obj, Formatting.Indented);
        File.WriteAllText(dumpFilePath, content);
    }
}
