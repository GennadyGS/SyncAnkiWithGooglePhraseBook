using UpdateAnki.Models;

namespace UpdateAnki.Utils;

internal static class CollectionSynchronizer
{
    public static UpdateActions<TKey, TValue> GetUpdateActions<TKey, TValue>(
        IReadOnlyCollection<TValue> source, IDictionary<TKey, TValue> target) =>
        new();
}
