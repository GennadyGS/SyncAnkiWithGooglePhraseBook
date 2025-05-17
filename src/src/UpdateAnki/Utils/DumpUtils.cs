using Newtonsoft.Json;
using UpdateAnki.Configuration;
using UpdateAnki.Extensions;

namespace UpdateAnki.Utils;

internal static class DumpUtils
{
    public static void DumpObject<T>(
        T obj, string baseFileName, CommandLineOptions commandLineOptions)
    {
        var dumpFileName = $"{baseFileName}_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.json";
        var dumpFilePath = commandLineOptions.GetFullLogFilePath(dumpFileName);
        var content = JsonConvert.SerializeObject(obj, Formatting.Indented);
        File.WriteAllText(dumpFilePath, content);
    }
}
