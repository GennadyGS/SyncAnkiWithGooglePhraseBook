using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace SyncAnkiWithGooglePhraseBookExtension;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.UseBrowserExtension(_ =>
            builder.RootComponents.AddBackgroundWorker<BackgroundWorker>());
        await builder.Build().RunAsync();
    }
}