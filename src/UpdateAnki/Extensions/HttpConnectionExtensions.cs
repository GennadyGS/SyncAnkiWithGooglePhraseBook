using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UpdateAnki.Exceptions;
using UpdateAnki.Models;
using UpdateAnki.Utils;

namespace UpdateAnki.Extensions;

internal static class HttpConnectionExtensions
{
    private const int AnkiConnectApiVersion = 6;

    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
    };

    public static async Task<dynamic?> InvokeAnkiCommandAsync(
        this HttpClient httpClient, string action, dynamic parameters) =>
        await InvokeAnkiCommandAsync<dynamic, dynamic>(httpClient, action, parameters);

    public static async Task<TResult?> InvokeAnkiCommandAsync<TParams, TResult>(
        this HttpClient httpClient, string action, TParams parameters)
    {
        var ankiRequest = new AnkiRequest<TParams>
        {
            Action = action,
            Version = AnkiConnectApiVersion,
            Params = parameters,
        };

        var content =
            HttpContentFactory.CreateJsonContentWithFixedLength(ankiRequest, JsonSettings);
        var responseMessage = await httpClient.PostAsync(new Uri("/", UriKind.Relative), content);
        responseMessage.EnsureSuccessStatusCode();
        var ankiResponse = await responseMessage.GetAnkiResponseAsync<TResult>(JsonSettings);
        if (ankiResponse.Error is not null)
        {
            throw new AnkiException(ankiResponse.Error);
        }

        return ankiResponse.Result;
    }
}
