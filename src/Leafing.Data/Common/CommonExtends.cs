using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Leafing.Data;
using Leafing.Data.Builder;
using Leafing.Data.Definition;
using Leafing.Data.Model.Member;

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

    public static bool NotIn<T>(this T key, params T[] values) where T : struct
    {
        throw new Exception("It's only for Linq");
    }

    public static bool NotIn(this string key, params string[] values)
    {
        throw new Exception("It's only for Linq");
    }

    public static bool NotInStatement<T>(this T key, SelectStatementBuilder statement) where T : struct
    {
        throw new Exception("It's only for Linq");
    }

    public static bool NotInStatement(this string key, SelectStatementBuilder statement)
    {
        throw new Exception("It's only for Linq");
    }

    public static T GetAttribute<T>(this List<Attribute> attributes) where T : Attribute
    {
        return (T)attributes.FirstOrDefault(o => o is T);
    }

    public static DataTable ToDataTable<T>(this List<T> list) where T : IDbObject
    {
        var ctx = ModelContext.GetInstance(typeof(T));
        var dt = new DataTable(ctx.Info.From.MainTableName);
        foreach (MemberHandler m in ctx.Info.SimpleMembers)
        {
            DataColumn dc
                = m.MemberType.IsGenericType
                ? new DataColumn(m.Name, m.MemberType.GetGenericArguments()[0])
                : new DataColumn(m.Name, m.MemberType);
            if (m.Is.AllowNull)
            {
                dc.AllowDBNull = true;
            }
            dt.Columns.Add(dc);
        }
        foreach (T o in list)
        {
            DataRow dr = dt.NewRow();
            foreach (MemberHandler m in ctx.Info.SimpleMembers)
            {
                object ov = m.GetValue(o);
                if (ov == null)
                {
                    dr[m.Name] = DBNull.Value;
                }
                else
                {
                    dr[m.Name]
                        = m.MemberType.IsGenericType
                        ? m.MemberType.GetMethod("get_Value").Invoke(ov, null)
                        : ov;
                }
            }
            dt.Rows.Add(dr);
        }
        return dt;
    }
}
