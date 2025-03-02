using UpdateAnki.Models;

namespace UpdateAnki.Utils;

public static class UpdateActionsCalculator
{
    public static UpdateActions<TKey, TValue> GetUpdateActions<TKey, TValue>(
        IReadOnlyCollection<TValue> source,
        IDictionary<TKey, TValue> target,
        bool deleteUnmatched = false,
        bool deleteExcessMatched = false,
        IComparer<TValue>? matchComparer = null,
        Func<TValue, TValue, double>? valueEditDistanceProvider = null) =>
        new();
}
