using DistanceProviders;
using DistanceProviders.Extensions;
using FluentAssertions;
using UpdateAnki.Models;
using UpdateAnki.UnitTests.Utils;
using UpdateAnki.Utils;

namespace UpdateAnki.UnitTests;

public sealed class ModificationActionsCalculatorTests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Layout",
        "MEN003:Method is too long",
        Justification = "Method does not contain logic, just data declarations.")]
    public static TheoryData<TestCase<int, string>> GetTestCases() =>
        TheoryDataBuilder.TheoryData((TestCase<int, string>[])[
            new TestCase<int, string>
            {
                Id = 1,
                Source = ["One", "two", "three"],
                Target = new Dictionary<int, string>
                {
                    [1] = "one",
                    [2] = "two",
                    [4] = "four",
                },
                MatchComparer = StringComparer.OrdinalIgnoreCase,
                DeleteUnmatched = true,
                ExpectedResult = new ModificationActions<int, string>
                {
                    ToAdd = ["three"],
                    ToUpdate = [KeyValuePair.Create(1, "One")],
                    ToDelete = [4],
                },
            },
            new TestCase<int, string>
            {
                Id = 2,
                Source = ["One", "two", "three"],
                Target = new Dictionary<int, string>
                {
                    [1] = "one",
                    [2] = "two",
                    [4] = "four",
                },
                MatchComparer = StringComparer.OrdinalIgnoreCase,
                ValueDistanceProvider = StringComparer.OrdinalIgnoreCase.ToDistanceProvider(),
                DeleteUnmatched = true,
                ExpectedResult = new ModificationActions<int, string>
                {
                    ToAdd = ["three"],
                    ToUpdate = [],
                    ToDelete = [4],
                },
            },
            new TestCase<int, string>
            {
                Id = 3,
                Source = ["ONE", "one", "OnE", "Two", "three"],
                Target = new Dictionary<int, string>
                {
                    [1] = "one",
                    [2] = "two",
                    [4] = "four",
                },
                MatchComparer = StringComparer.OrdinalIgnoreCase,
                ValueDistanceProvider =
                    new StringEditDistanceProvider(new CaseSoftCharEditDistanceProvider(0.5)),
                DeleteUnmatched = true,
                ExpectedResult = new ModificationActions<int, string>
                {
                    ToAdd = ["ONE", "OnE", "three"],
                    ToUpdate = [KeyValuePair.Create(2, "Two")],
                    ToDelete = [4],
                },
            },
        ]);

    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void GetUpdateActions_ShouldReturnCorrectResults(TestCase<int, string> testCase)
    {
        var result = ModificationActionsCalculator.GetModificationActions(
            testCase.Source,
            testCase.Target,
            testCase.DeleteUnmatched,
            testCase.DeleteExcessMatched,
            testCase.MatchComparer,
            testCase.ValueDistanceProvider);

        result.Should().BeEquivalentTo(testCase.ExpectedResult);
    }

    public sealed record TestCase<TKey, TValue>
        where TKey : notnull
    {
        public required int Id { get; init; }

        public required IReadOnlyCollection<TValue> Source { get; init; }

        public required IDictionary<TKey, TValue> Target { get; init; }

        public bool DeleteUnmatched { get; init; }

        public bool DeleteExcessMatched { get; init; }

        public IEqualityComparer<string>? MatchComparer { get; init; }

        public IDistanceProvider<TValue>? ValueDistanceProvider { get; init; }

        public required ModificationActions<TKey, TValue> ExpectedResult { get; init; }
    }
}
