namespace DistanceProviders;

public interface IDistanceProvider<in T>
{
    public double GetDistance(T source, T target);
}
