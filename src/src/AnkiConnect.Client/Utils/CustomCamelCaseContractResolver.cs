using Newtonsoft.Json.Serialization;

namespace UpdateAnki.Utils;

internal sealed class CustomCamelCaseContractResolver : CamelCasePropertyNamesContractResolver
{
    protected override JsonContract CreateContract(Type objectType)
    {
        if (!objectType.IsGenericType ||
            objectType.GetGenericTypeDefinition() != typeof(Dictionary<,>) ||
            objectType.GetGenericArguments()[0] != typeof(string))
        {
            return base.CreateContract(objectType);
        }

        var contract = base.CreateContract(objectType);
        if (contract is JsonDictionaryContract dictionaryContract)
        {
            dictionaryContract.DictionaryKeyResolver = key => key;
        }

        return contract;
    }
}
