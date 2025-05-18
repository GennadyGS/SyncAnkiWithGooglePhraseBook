using ChangeSetCalculation.Models;
using FluentAssertions;
using Translation.Models;
using Xunit;

namespace Translation.ChangeSetCalculation.UnitTests;

public sealed class PhraseTranslationChangeSetCalculatorTests
{
    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Layout",
        "MEN003:Method is too long",
        Justification = "Pending")]
    public void CalculateChangeSet_ReturnsEmpty_WhenNoChanges()
    {
        var source = new PhraseTranslation[]
        {
            new()
            {
                Source = new Phrase
                {
                    Text = "hassle",
                    LanguageCode = "en",
                },
                Target = new Phrase
                {
                    Text = "хлопоты",
                    LanguageCode = "ru",
                },
            },
        };
        var target = new Dictionary<long, PhraseTranslation>
        {
            [1465978079469] = new()
            {
                Source = new Phrase
                {
                    Text = "hassle",
                    LanguageCode = "en",
                },
                Target = new Phrase
                {
                    Text = "стычка",
                    LanguageCode = "ru",
                },
            },
        };

        var result = PhraseTranslationChangeSetCalculator.CalculateChangeSet(source, target);

        var expectedResult = new ChangeSet<PhraseTranslation, KeyValuePair<long, PhraseTranslation>>
        {
            ToAdd = [],
            ToUpdate = [
                (
                    source: new()
                    {
                        Source = new Phrase
                        {
                            Text = "hassle",
                            LanguageCode = "en",
                        },
                        Target = new Phrase
                        {
                            Text = "хлопоты",
                            LanguageCode = "ru",
                        },
                    },
                    target: KeyValuePair.Create(
                        1465978079469,
                        new PhraseTranslation
                        {
                            Source = new Phrase
                            {
                                Text = "hassle",
                                LanguageCode = "en",
                            },
                            Target = new Phrase
                            {
                                Text = "стычка",
                                LanguageCode = "ru",
                            },
                        })),
            ],
            ToDelete = [],
        };
        result.Should().BeEquivalentTo(expectedResult);
    }
}
