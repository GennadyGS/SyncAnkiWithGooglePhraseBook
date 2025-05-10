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
                ExpectedDistance = 3.0 / 7,
            },
            new TestCase
            {
                Source = "flaw",
                Target = "lawn",
                ExpectedDistance = 0.5,
            },
            new TestCase
            {
                Source = "distance",
                Target = "instance",
                ExpectedDistance = 0.25,
            },
            new TestCase
            {
                Source = "example",
                Target = "samples",
                ExpectedDistance = 3.0 / 7,
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
                ExpectedDistance = 1,
            },
            new TestCase
            {
                Source = "test",
                Target = string.Empty,
                ExpectedDistance = 1,
            },
        ]);

    public static TheoryData<TestCase> GetIgnoreCaseTestCases() =>
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
                ExpectedDistance = 3.0 / 7,
            },
            new TestCase
            {
                Source = string.Empty,
                Target = "test",
                ExpectedDistance = 1,
            },
            new TestCase
            {
                Source = "test",
                Target = string.Empty,
                ExpectedDistance = 1,
            },
        ]);

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Layout",
        "MEN003:Method is too long",
        Justification = "Method does not contain logic, just data declarations")]
    public static TheoryData<SoftCaseTestCase> GetSoftCaseTestCases() =>
        TheoryDataBuilder.TheoryData(
        [
            new SoftCaseTestCase
            {
                CaseWeight = 0.7,
                Source = "kitten",
                Target = "sitting",
                ExpectedDistance = 3.0 / 7,
            },
            new SoftCaseTestCase
            {
                CaseWeight = 0.4,
                Source = "CASE",
                Target = "case",
                ExpectedDistance = 1.6 / 4,
            },
            new SoftCaseTestCase
            {
                CaseWeight = 0.3,
                Source = "MisMatch",
                Target = "match",
                ExpectedDistance = 3.3 / 8,
            },
            new SoftCaseTestCase
            {
                CaseWeight = 0.6,
                Source = "Edit",
                Target = "Distance",
                ExpectedDistance = 6.6 / 8,
            },
            new SoftCaseTestCase
            {
                CaseWeight = 0.5,
                Source = "abc",
                Target = "aBc",
                ExpectedDistance = 0.5 / 3,
            },
            new SoftCaseTestCase
            {
                CaseWeight = 0.5,
                Source = "abc",
                Target = "abcd",
                ExpectedDistance = 1.0 / 4,
            },
            new SoftCaseTestCase
            {
                CaseWeight = 0.5,
                Source = "abcd",
                Target = "abc",
                ExpectedDistance = 1.0 / 4,
            },
            new SoftCaseTestCase
            {
                CaseWeight = 0.5,
                Source = "abc",
                Target = "aXc",
                ExpectedDistance = 1.0 / 3,
            },
            new SoftCaseTestCase
            {
                CaseWeight = 0.5,
                Source = "abc",
                Target = "aBcX",
                ExpectedDistance = 1.5 / 4,
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

    [Theory]
    [MemberData(nameof(GetSoftCaseTestCases))]
    public void GetDistance_ShouldReturnCorrectDistance_ForSoftCaseDistanceProvider(
        SoftCaseTestCase testCase)
    {
        var sut = StringDistanceProviders.CreateSoftCase(testCase.CaseWeight);

        var distance = sut.GetDistance(testCase.Source, testCase.Target);

        distance.Should().BeApproximately(testCase.ExpectedDistance, double.Epsilon);
    }

    public class TestCase
    {
        public required string Source { get; init; }

        public required string Target { get; init; }

        public required double ExpectedDistance { get; init; }
    }

    public sealed class SoftCaseTestCase : TestCase
    {
        public required double CaseWeight { get; init; }
    }
}
