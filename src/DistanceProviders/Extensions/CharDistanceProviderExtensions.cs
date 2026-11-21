namespace DistanceProviders.Extensions;

public static class CharDistanceProviderExtensions
{
    public static IDistanceProvider<string> ToStringDistanceProvider(
        this IDistanceProvider<char> source) =>
        new StringDistanceProvider(source);
}
