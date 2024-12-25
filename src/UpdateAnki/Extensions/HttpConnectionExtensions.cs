using System.Net.Http.Json;
using System.Text.Json;
using UpdateAnki.Models;

namespace UpdateAnki.Extensions;

internal static class HttpConnectionExtensions
{
    private const int AnkiConnectApiVersion = 6;

    public static async Task<dynamic?> InvokeAnkiCommandAsync(
        this HttpClient httpClient, string action, dynamic parameters) =>
        await InvokeAnkiCommandAsync<dynamic, dynamic>(httpClient, action, parameters);

    private static async Task<TResult?> InvokeAnkiCommandAsync<TParams, TResult>(
        HttpClient httpClient, string action, TParams parameters)
    {
        var request = new AnkiRequest<TParams>
        {
            Action = action,
            Version = AnkiConnectApiVersion,
            Params = parameters,
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        var responseMessage =
            await httpClient.PostAsJsonAsync(new Uri(string.Empty), request, options);
        responseMessage.EnsureSuccessStatusCode();
        return await responseMessage.Content.ReadFromJsonAsync<TResult>();
    }
}