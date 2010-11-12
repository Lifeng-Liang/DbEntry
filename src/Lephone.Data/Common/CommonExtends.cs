using System;

public static class CommonExtends
{
    public static bool In(this ValueType key, params ValueType[] values)
    {
        throw new Exception("It's only for Linq");
    }

    public static bool In(this string key, params string[] values)
    {
        throw new Exception("It's only for Linq");
    }
}
