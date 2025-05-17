using ChangeSetCalculation;
using ChangeSetCalculation.Models;
using Translation.Models;
using UpdateAnki.Comparers;

namespace UpdateAnki.Utils;

internal static class PhraseTranslationChangeSetCalculator
{
    public static ChangeSet<PhraseTranslation, KeyValuePair<long, PhraseTranslation>>
        CalculateChangeSet(
            IReadOnlyCollection<PhraseTranslation> sourcePhraseTranslations,
            IReadOnlyDictionary<long, PhraseTranslation> targetPhraseTranslations) =>
        ChangeSetCalculator.CalculateChangeSet(
            sourcePhraseTranslations,
            targetPhraseTranslations,
            s => s,
            t => t.Value,
            deleteExcessMatched: true,
            deleteUnmatched: false,
            matchComparer: new PhraseTranslationMatchComparer(),
            keyDistanceProvider: new PhraseTranslationDistanceProvider());
}
