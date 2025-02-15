using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;

namespace UpdateAnki.Utils;

internal static class HttpContentFactory
{
    private static readonly Encoding DefaultEncoding = Encoding.UTF8;

    public static HttpContent CreateJsonContentWithFixedLength<TValue>(
        TValue value, JsonSerializerSettings jsonSettings, Encoding? encoding = null)
    {
        var jsonData = JsonConvert.SerializeObject(value, jsonSettings);
        var establishedEncoding = encoding ?? DefaultEncoding;
        var result =
            new StringContent(jsonData, establishedEncoding, MediaTypeNames.Application.Json);
        result.Headers.ContentLength = establishedEncoding.GetByteCount(jsonData);
        return result;
    }
}