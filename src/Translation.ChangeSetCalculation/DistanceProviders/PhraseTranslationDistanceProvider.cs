using DistanceProviders;
using DistanceProviders.Constants;
using Translation.ChangeSetCalculation.Comparers;
using Translation.Models;

namespace Translation.ChangeSetCalculation.DistanceProviders;

internal sealed class PhraseTranslationDistanceProvider(
    PhraseTranslationMatchComparer matchComparer) : IDistanceProvider<PhraseTranslation>
{
    private const double CaseDistance = 0.01;

    private readonly PhraseTranslationMatchComparer _matchComparer = matchComparer;

    private readonly IDistanceProvider<string> _stringDistanceProvider =
        new StringDistanceProvider(new SoftCaseCharDistanceProvider(CaseDistance));

    public double GetDistance(PhraseTranslation source, PhraseTranslation target)
    {
        if (!_matchComparer.Equals(source, target))
        {
            return Distance.MaxValue;
        }

        return
            _stringDistanceProvider.GetDistance(GetSourceText(source), GetSourceText(target)) +
            _stringDistanceProvider.GetDistance(GetTargetText(source), GetTargetText(target));
    }

    private static string GetSourceText(PhraseTranslation translation) => translation.Source.Text;

    private static string GetTargetText(PhraseTranslation translation) => translation.Target.Text;
}
