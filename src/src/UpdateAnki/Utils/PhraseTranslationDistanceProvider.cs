using DistanceProviders;
using Translation.Models;

namespace UpdateAnki.Utils;

internal sealed class PhraseTranslationDistanceProvider : IDistanceProvider<PhraseTranslation>
{
    private const double CaseDistance = 0.01;

    private readonly IDistanceProvider<string> _stringDistanceProvider =
        new StringDistanceProvider(new SoftCaseCharDistanceProvider(CaseDistance));

    public double GetDistance(PhraseTranslation source, PhraseTranslation target) =>
        _stringDistanceProvider.GetDistance(GetTargetText(source), GetTargetText(target));

    private static string GetTargetText(PhraseTranslation translation) => translation.Target.Text;
}
