using System;
using Lephone.Data.Builder;

public static class CommonExtends
{
    public static bool In<T>(this T key, params T[] values) where T : struct 
    {
        throw new Exception("It's only for Linq");
    }

    public static bool In(this string key, params string[] values)
    {
        throw new Exception("It's only for Linq");
    }

    public static bool InStatement<T>(this T key, SelectStatementBuilder statement) where T : struct 
    {
        throw new Exception("It's only for Linq");
    }

    public static bool InStatement(this string key, SelectStatementBuilder statement)
    {
        throw new Exception("It's only for Linq");
    }
}
