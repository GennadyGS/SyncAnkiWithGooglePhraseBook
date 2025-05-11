using Newtonsoft.Json;
using UpdateAnki.Configuration;
using UpdateAnki.Extensions;

namespace UpdateAnki.Utils;

internal static class DumpUtils
{
    public static void DumpObject<T>(
        T obj, string baseFileName, CommandLineOptions commandLineOptions)
    {
        var dumpFileName = $"{baseFileName}_{DateTime.UtcNow:yyyy-MM-dd-hh-mm-ss}.json";
        var dumpFilePath = commandLineOptions.GetFullLogFilePath(dumpFileName);
        File.WriteAllText(dumpFilePath, JsonConvert.SerializeObject(obj, Formatting.Indented));
    }
}
