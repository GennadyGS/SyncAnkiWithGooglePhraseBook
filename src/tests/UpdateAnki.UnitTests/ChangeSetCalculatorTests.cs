using DistanceProviders;
using FluentAssertions;
using TestUtils;
using UpdateAnki.Models;
using UpdateAnki.Utils;
using Xunit;

namespace UpdateAnki.UnitTests;

public sealed class ChangeSetCalculatorTests
{
    public static TheoryData<TestCase<string, KeyValuePair<int, string>>> GetDefaultTestCases() =>
        TheoryDataBuilder.TheoryData([
            new TestCase<string, KeyValuePair<int, string>>
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
                ExpectedResult = new ChangeSet<string, KeyValuePair<int, string>>
                {
                    ToAdd = ["three"],
                    ToUpdate = [(source: "One", target: KeyValuePair.Create(1, "one"))],
                    ToDelete = [KeyValuePair.Create(4, "four")],
                },
            },
        ]);

    public static TheoryData<TestCase<string, KeyValuePair<int, string>>>
        GetIgnoreCaseTestCases() =>
        TheoryDataBuilder.TheoryData([
            new TestCase<string, KeyValuePair<int, string>>
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
                DeleteUnmatched = true,
                ExpectedResult = new ChangeSet<string, KeyValuePair<int, string>>
                {
                    ToAdd = ["three"],
                    ToUpdate = [],
                    ToDelete = [KeyValuePair.Create(4, "four")],
                },
            },
        ]);

    public static TheoryData<SoftCaseTestCase<string, KeyValuePair<int, string>>>
        GetSoftCaseTestCases() =>
        TheoryDataBuilder.TheoryData([
            new SoftCaseTestCase<string, KeyValuePair<int, string>>
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
                CaseWeight = 0.5,
                DeleteUnmatched = true,
                ExpectedResult = new ChangeSet<string, KeyValuePair<int, string>>
                {
                    ToAdd = ["ONE", "OnE", "three"],
                    ToUpdate = [(source: "Two", target: KeyValuePair.Create(2, "two"))],
                    ToDelete = [KeyValuePair.Create(4, "four")],
                },
            },
        ]);

    [Theory]
    [MemberData(nameof(GetDefaultTestCases))]
    public void GetUpdateActions_ShouldReturnCorrectResults_ForDefaultDistanceProvider(
        TestCase<string, KeyValuePair<int, string>> testCase)
    {
        RunTestCase(testCase, StringDistanceProviders.Default);
    }

    [Theory]
    [MemberData(nameof(GetIgnoreCaseTestCases))]
    public void GetUpdateActions_ShouldReturnCorrectResults_ForIgnoreCaseDistanceProvider(
        TestCase<string, KeyValuePair<int, string>> testCase)
    {
        RunTestCase(testCase, StringDistanceProviders.OrdinalIgnoreCase);
    }

    [Theory]
    [MemberData(nameof(GetSoftCaseTestCases))]
    public void GetUpdateActions_ShouldReturnCorrectResults_ForSoftCaseDistanceProvider(
        SoftCaseTestCase<string, KeyValuePair<int, string>> testCase)
    {
        RunTestCase(testCase, StringDistanceProviders.CreateSoftCase(testCase.CaseWeight));
    }

    private static void RunTestCase(
        TestCase<string, KeyValuePair<int, string>> testCase,
        IDistanceProvider<string> valueDistanceProvider)
    {
        var result = ChangeSetCalculator.CalculateChangeSet(
            testCase.Source,
            testCase.Target,
            s => s,
            t => t.Value,
            testCase.DeleteUnmatched,
            testCase.DeleteExcessMatched,
            testCase.MatchComparer,
            valueDistanceProvider);

        result.Should().BeEquivalentTo(testCase.ExpectedResult);
    }

    public record TestCase<TSource, TTarget>
    {
        public required int Id { get; init; }

        public required IReadOnlyCollection<TSource> Source { get; init; }

        public required IReadOnlyCollection<TTarget> Target { get; init; }

        public bool DeleteUnmatched { get; init; }

        public bool DeleteExcessMatched { get; init; }

        public IEqualityComparer<string>? MatchComparer { get; init; }

        public required ChangeSet<TSource, TTarget> ExpectedResult { get; init; }
    }

    public sealed record SoftCaseTestCase<TSource, TTarget> : TestCase<TSource, TTarget>
    {
        public required double CaseWeight { get; init; }
    }
}
