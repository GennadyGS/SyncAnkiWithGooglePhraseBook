using Newtonsoft.Json;
using UpdateAnki.Models;

namespace UpdateAnki.Extensions;

internal static class HttpResponseMessageExtensions
{
    public static async Task<AnkiResponse<TResult>> GetAnkiResponse<TResult>(
        this HttpResponseMessage responseMessage, JsonSerializerSettings jsonSettings)
    {
        var ankiResponseString = await responseMessage.Content.ReadAsStringAsync();
        var ankiResponse =
            JsonConvert.DeserializeObject<AnkiResponse<TResult>>(ankiResponseString, jsonSettings);
        return ankiResponse.ThrowIfNull();
    }
}
