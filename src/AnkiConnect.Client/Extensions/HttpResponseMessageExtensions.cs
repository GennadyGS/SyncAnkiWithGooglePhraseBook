using AnkiConnect.Client.Models;
using Common.Extensions;
using Newtonsoft.Json;

namespace AnkiConnect.Client.Extensions;

internal static class HttpResponseMessageExtensions
{
    public static async Task<AnkiResponse<TResult>> GetAnkiResponseAsync<TResult>(
        this HttpResponseMessage responseMessage, JsonSerializerSettings jsonSettings)
    {
        var ankiResponseString = await responseMessage.Content.ReadAsStringAsync();
        var ankiResponse =
            JsonConvert.DeserializeObject<AnkiResponse<TResult>>(ankiResponseString, jsonSettings);
        return ankiResponse.ThrowIfNull();
    }
}
