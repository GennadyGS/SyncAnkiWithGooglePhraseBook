using Microsoft.Extensions.DependencyInjection;
using UpdateAnki.Models;
using UpdateAnki.Services;

namespace UpdateAnki;

internal static class ServiceConfigurator
{
    private const string HttpClientName = "AnkiConnect";

    public static IServiceCollection RegisterServices(
        this ServiceCollection services, AnkiConnectSettings ankiConnectSettings)
    {
        services.AddHttpClient(HttpClientName)
            .ConfigureHttpClient(httpClient =>
            {
                httpClient.BaseAddress = ankiConnectSettings.Uri;
            })
            .AddTypedClient<AnkiPhraseTranslationsRepository>();
        return services
            .AddScoped<JsonPhraseTranslationsReader>()
            .AddScoped<UpdateAnkiService>();
    }
}
