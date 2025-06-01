using ChangeSetCalculation.Models;
using FluentAssertions;
using TestUtils;
using Translation.Models;
using Xunit;

namespace Translation.ChangeSetCalculation.UnitTests;

public sealed class PhraseTranslationChangeSetCalculatorTests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Layout",
        "MEN003:Method is too long",
        Justification = "Declarative code only.")]
    public static TheoryData<TestCase> GetTestCases() =>
        TheoryDataBuilder.TheoryData(
        [
            new TestCase
            {
                Id = 1,
                Source =
                [
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
                ],
                Target = new Dictionary<long, PhraseTranslation>
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
                },
                ExpectedResult = new ChangeSet<PhraseTranslation, KeyValuePair<long, PhraseTranslation>>
                {
                    ToAdd = [],
                    ToUpdate = [
                        new UpdatePair<PhraseTranslation, KeyValuePair<long, PhraseTranslation>>(
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
                            KeyValuePair.Create(
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
                },
            },
        ]);

    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void CalculateChangeSet_ReturnsEmpty_WhenNoChanges(TestCase testCase)
    {
        var result = PhraseTranslationChangeSetCalculator
            .CalculateChangeSet(testCase.Source, testCase.Target);

        result.Should().BeEquivalentTo(testCase.ExpectedResult);
    }

    public sealed record TestCase
    {
        public required int Id { get; init; }

        public required PhraseTranslation[] Source { get; init; }

        public required IReadOnlyDictionary<long, PhraseTranslation> Target { get; init; }

        public required ChangeSet<PhraseTranslation, KeyValuePair<long, PhraseTranslation>>
            ExpectedResult { get; init; }
    }
}
