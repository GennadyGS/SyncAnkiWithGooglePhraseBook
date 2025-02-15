using System.Net.Http.Json;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using UpdateAnki.Exceptions;
using UpdateAnki.Models;

namespace UpdateAnki.Extensions;

internal static class HttpConnectionExtensions
{
    private const int AnkiConnectApiVersion = 6;

    private static readonly Encoding Encoding = Encoding.UTF8;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static async Task<dynamic> InvokeAnkiCommandAsync(
        this HttpClient httpClient, string action, dynamic parameters) =>
        await InvokeAnkiCommandAsync<dynamic, dynamic>(httpClient, action, parameters);

    public static async Task<TResult> InvokeAnkiCommandAsync<TParams, TResult>(
        this HttpClient httpClient, string action, TParams parameters)
    {
        var ankiRequest = new AnkiRequest<TParams>
        {
            Action = action,
            Version = AnkiConnectApiVersion,
            Params = parameters,
        };

        var content = CreateJsonContentWithFixedLength(ankiRequest, JsonOptions);
        var responseMessage = await httpClient.PostAsync(new Uri("/", UriKind.Relative), content);
        responseMessage.EnsureSuccessStatusCode();
        var ankiResponse =
            await responseMessage.Content.ReadFromJsonAsync<AnkiResponse<TResult>>(JsonOptions);
        var establishedAnkiResponse = ankiResponse.ThrowIfNull();
        if (establishedAnkiResponse.Error is not null)
        {
            throw new AnkiException(establishedAnkiResponse.Error);
        }

        return establishedAnkiResponse.Result ??
            throw new NullReferenceException("Result cannot be null");
    }

    private static HttpContent CreateJsonContentWithFixedLength<TValue>(
        TValue value, JsonSerializerOptions jsonSerializerOptions)
    {
        var jsonData = JsonSerializer.Serialize(value, jsonSerializerOptions);
        var content = new StringContent(jsonData, Encoding, MediaTypeNames.Application.Json);
        content.Headers.ContentLength = Encoding.GetByteCount(jsonData);
        return content;
    }
}