namespace UpdateAnki.UnitTests.Utils;

public static class TheoryDataBuilder
{
    public static TheoryData<T> TheoryData<T>(params IEnumerable<T> testCases)
    {
        var result = new TheoryData<T>();
        foreach (var testCase in testCases)
        {
            result.Add(testCase);
        }

        return result;
    }

    public static TheoryData<T1, T2> TheoryData<T1, T2>(
        params IEnumerable<(T1 fst, T2 snd)> testCases)
    {
        var result = new TheoryData<T1, T2>();
        foreach (var (arg1, arg2) in testCases)
        {
            result.Add(arg1, arg2);
        }

        return result;
    }
}
