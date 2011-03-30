using System;
using System.Linq;
using System.Collections.Generic;
using Lephone.Data.Builder;

public static class CommonExtends
{
    public static bool IsNull<T>(this T key) where T : struct
    {
        throw new Exception("It's only for Linq");
    }

    public static bool IsNotNull<T>(this T key) where T : struct
    {
        throw new Exception("It's only for Linq");
    }

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

    public static T GetAttribute<T>(this List<Attribute> attributes) where T : Attribute
    {
        return (T)attributes.FirstOrDefault(o => o is T);
    }
}
