namespace UpdateAnki.Utils;

public static class MathUtils
{
    public static bool EqualWithTolerance(double a, double b, double tolerance = 1e-10) =>
        Math.Abs(a - b) < tolerance;
}
