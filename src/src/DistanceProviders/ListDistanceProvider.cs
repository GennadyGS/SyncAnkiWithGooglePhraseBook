namespace DistanceProviders;

public sealed class ListDistanceProvider<T>(IDistanceProvider<T> itemDistanceProvider)
    : IDistanceProvider<IReadOnlyList<T>>
{
    private readonly IDistanceProvider<T> _itemDistanceProvider = itemDistanceProvider;

    public double GetDistance(IReadOnlyList<T> source, IReadOnlyList<T> target)
    {
        if (source.Count == 0)
        {
            return target.Count;
        }

        if (target.Count == 0)
        {
            return source.Count;
        }

        var matrix = CreateDistanceMatrix(source.Count, target.Count);

        for (var si = 1; si <= source.Count; si++)
        {
            for (var ti = 1; ti <= target.Count; ti++)
            {
                var distance = _itemDistanceProvider.GetDistance(target[ti - 1], source[si - 1]);

                matrix[si, ti] = Math.Min(
                    Math.Min(matrix[si - 1, ti] + 1, matrix[si, ti - 1] + 1),
                    matrix[si - 1, ti - 1] + distance);
            }
        }

        return matrix[source.Count, target.Count];
    }

    private static double[,] CreateDistanceMatrix(int sourceLength, int targetLength)
    {
        var matrix = new double[sourceLength + 1, targetLength + 1];

        for (var si = 0; si <= sourceLength; matrix[si, 0] = si++)
        {
            // do nothing
        }

        for (var ti = 0; ti <= targetLength; matrix[0, ti] = ti++)
        {
            // do nothing
        }

        return matrix;
    }
}
