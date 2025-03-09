using FluentAssertions;
using UpdateAnki.Models;
using UpdateAnki.UnitTests.Utils;
using UpdateAnki.Utils;

namespace UpdateAnki.UnitTests;

public sealed class UpdateActionsCalculatorTests
{
    public static TheoryData<TestCase<int, string>> GetTestCases() =>
        TheoryDataBuilder.TheoryData([
            new TestCase<int, string>
            {
                Source = ["One", "two", "three"],
                Target = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } },
                ExpectedResult = new UpdateActions<int, string>
                {
                    ToAdd = new List<string> { "three" },
                    ToUpdate = [KeyValuePair.Create(1, "One")],
                },
            },
        ]);

    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void GetUpdateActions_ShouldReturnCorrectResults(TestCase<int, string> testCase)
    {
        var result = UpdateActionsCalculator.GetUpdateActions(
            testCase.Source,
            testCase.Target,
            testCase.DeleteUnmatched,
            testCase.DeleteExcessMatched,
            StringComparer.OrdinalIgnoreCase);

        result.Should().BeEquivalentTo(testCase.ExpectedResult);
    }

    public sealed record TestCase<TKey, TValue>
        where TKey : notnull
    {
        public required IReadOnlyCollection<TValue> Source { get; init; }

        public required IDictionary<TKey, TValue> Target { get; init; }

        public bool DeleteUnmatched { get; init; }

        public bool DeleteExcessMatched { get; init; }

        public required UpdateActions<TKey, TValue> ExpectedResult { get; init; }
    }
}
