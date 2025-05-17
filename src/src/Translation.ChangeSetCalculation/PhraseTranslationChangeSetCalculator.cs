using ChangeSetCalculation;
using ChangeSetCalculation.Models;
using Translation.ChangeSetCalculation.Comparers;
using Translation.ChangeSetCalculation.DistanceProviders;
using Translation.Models;

namespace Translation.ChangeSetCalculation;

public static class PhraseTranslationChangeSetCalculator
{
    public static ChangeSet<PhraseTranslation, KeyValuePair<long, PhraseTranslation>>
        CalculateChangeSet(
            IReadOnlyCollection<PhraseTranslation> sourcePhraseTranslations,
            IReadOnlyDictionary<long, PhraseTranslation> targetPhraseTranslations)
    {
        var matchComparer = new PhraseTranslationMatchComparer();
        var distanceProvider = new PhraseTranslationDistanceProvider(matchComparer);
        return ChangeSetCalculator.CalculateChangeSet(
            sourcePhraseTranslations,
            targetPhraseTranslations,
            s => s,
            t => t.Value,
            deleteExcessMatched: true,
            deleteUnmatched: false,
            matchComparer: matchComparer,
            keyDistanceProvider: distanceProvider);
    }
}
