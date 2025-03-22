using System.Collections;

namespace DustCollector.Tests;

// Allows for comparison of floating points within a specified precision
class FloatComparer : IComparer
{
    public FloatComparer(float precision_in)
    {
        precision = precision_in;
    }
    public float precision;

    public int Compare(object? x, object? y)
    {
        if (x is not float)
        {
            throw new ArgumentException("x is of the wrong type, float expected.", nameof(x));
        }
        if (y is not float)
        {
            throw new ArgumentException("y is of the wrong type, float expected.", nameof(y));
        }

        float f1 = (float)x;
        float f2 = (float)y;
        if (f1 + precision < f2) { return -1; }
        if (f2 + precision < f1) { return 1; }
        return 0;
    }
}