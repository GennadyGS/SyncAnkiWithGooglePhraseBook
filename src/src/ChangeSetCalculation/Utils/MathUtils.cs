namespace UpdateAnki.Utils;

public static class MathUtils
{
    public static bool EqualWithTolerance(double a, double b, double epsilon = double.Epsilon) =>
        Math.Abs(a - b) < epsilon;
}
