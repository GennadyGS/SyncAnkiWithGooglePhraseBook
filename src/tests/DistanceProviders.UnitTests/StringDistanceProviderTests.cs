using FluentAssertions;
using TestUtils;
using Xunit;

namespace DistanceProviders.UnitTests;

public sealed class StringDistanceProviderTests
{
    public static TheoryData<TestCase> GetDefaultTestCases() =>
        TheoryDataBuilder.TheoryData(
        [
            new TestCase
            {
                Source = string.Empty,
                Target = string.Empty,
                ExpectedDistance = 0,
            },
            new TestCase
            {
                Source = "kitten",
                Target = "sitting",
                ExpectedDistance = 3,
            },
            new TestCase
            {
                Source = "flaw",
                Target = "lawn",
                ExpectedDistance = 2,
            },
            new TestCase
            {
                Source = "distance",
                Target = "instance",
                ExpectedDistance = 2,
            },
            new TestCase
            {
                Source = "example",
                Target = "samples",
                ExpectedDistance = 3,
            },
            new TestCase
            {
                Source = "test",
                Target = "test",
                ExpectedDistance = 0,
            },
            new TestCase
            {
                Source = string.Empty,
                Target = "test",
                ExpectedDistance = 4,
            },
            new TestCase
            {
                Source = "test",
                Target = string.Empty,
                ExpectedDistance = 4,
            },
        ]);

    public static TheoryData<TestCase> GetIgnoreCaseTestCases() =>
        TheoryDataBuilder.TheoryData(
        [
            new TestCase
            {
                Source = "Test",
                Target = "test",
                ExpectedDistance = 0,
            },
            new TestCase
            {
                Source = "Example",
                Target = "example",
                ExpectedDistance = 0,
            },
            new TestCase
            {
                Source = "Kitten",
                Target = "SITTING",
                ExpectedDistance = 3,
            },
        ]);

    [Theory]
    [MemberData(nameof(GetDefaultTestCases))]
    public void GetDistance_ShouldReturnCorrectDistance_ForDefaultDistanceProvider(
        TestCase testCase)
    {
        var sut = StringDistanceProviders.Default;

        var distance = sut.GetDistance(testCase.Source, testCase.Target);

        distance.Should().BeApproximately(testCase.ExpectedDistance, double.Epsilon);
    }

    [Theory]
    [MemberData(nameof(GetIgnoreCaseTestCases))]
    public void GetDistance_ShouldReturnCorrectDistance_ForIgnoreCaseDistanceProvider(
        TestCase testCase)
    {
        var sut = StringDistanceProviders.OrdinalIgnoreCase;

        var distance = sut.GetDistance(testCase.Source, testCase.Target);

        distance.Should().BeApproximately(testCase.ExpectedDistance, double.Epsilon);
    }

    public sealed class TestCase
    {
        public required string Source { get; init; }

        public required string Target { get; init; }

        public required double ExpectedDistance { get; init; }
    }
}
