using DistanceProviders;
using Translation.Models;
using UpdateAnki.Comparers;

namespace UpdateAnki.Utils;

internal sealed class PhraseTranslationDistanceProvider(
    PhraseTranslationMatchComparer matchComparer) : IDistanceProvider<PhraseTranslation>
{
    private const double CaseDistance = 0.01;

    private readonly PhraseTranslationMatchComparer _matchComparer = matchComparer;

    private readonly IDistanceProvider<string> _stringDistanceProvider =
        new StringDistanceProvider(new SoftCaseCharDistanceProvider(CaseDistance));

    public double GetDistance(PhraseTranslation source, PhraseTranslation target) =>
        _matchComparer.Equals(source, target)
            ? _stringDistanceProvider.GetDistance(GetTargetText(source), GetTargetText(target))
            : 1;

    private static string GetTargetText(PhraseTranslation translation) => translation.Target.Text;
}
