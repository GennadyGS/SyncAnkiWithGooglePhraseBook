using AnkiConnect.Client.Models;
using AnkiConnect.Client.Utils;
using Newtonsoft.Json;
using UpdateAnki.Utils;

namespace AnkiConnect.Client.Extensions;

internal static class HttpConnectionExtensions
{
    private const int AnkiConnectApiVersion = 6;

    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        ContractResolver = new CustomCamelCaseContractResolver(),
    };

    extension(HttpClient httpClient)
    {
        public async Task<dynamic?> InvokeAnkiCommandAsync(
            string action, dynamic parameters) =>
            await InvokeAnkiCommandAsync<dynamic, dynamic>(httpClient, action, parameters);

        public async Task<TResult?> InvokeAnkiCommandAsync<TParams, TResult>(
            string action, TParams parameters)
        {
            var ankiResponse =
                await httpClient.PostAnkiCommandAsync<TParams, TResult>(action, parameters);
            if (ankiResponse.Error is not null)
            {
                throw new AnkiException(ankiResponse.Error);
            }

            return ankiResponse.Result;
        }

        public async Task<AnkiResponse<TResult>> PostAnkiCommandAsync<TParams, TResult>(
            string action, TParams parameters)
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
            return await responseMessage.GetAnkiResponseAsync<TResult>(JsonSettings);
        }
    }
}
