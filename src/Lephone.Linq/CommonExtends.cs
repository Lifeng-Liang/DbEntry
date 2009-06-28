using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Lephone.Data;
using Lephone.Data.QuerySyntax;
using Lephone.Data.Definition;
using Lephone.Linq;
using Lephone.Util;
using Lephone.Util.Text;
using Lephone.Web;

public static class CommonExtends
{
    #region Query

    public static IAfterWhere<T> Where<T>(this IWhere<T> t, Expression<Func<T, bool>> expr) where T : class, IDbObject
    {
        var me = (QueryContent<T>)t;
        if (expr != null)
        {
            me.m_where = ExpressionParser<T>.Parse(expr);
        }
        return me;
    }

    public static IRangeable<T> OrderBy<T>(this IAfterWhere<T> t, Expression<Func<T, object>> expr) where T : class, IDbObject
    {
        return AddOrderBy((QueryContent<T>)t, expr, true);
    }

    public static IRangeable<T> OrderByDescending<T>(this IAfterWhere<T> t, Expression<Func<T, object>> expr) where T : class, IDbObject
    {
        return AddOrderBy((QueryContent<T>)t, expr, false);
    }

    public static IRangeable<T> ThenBy<T>(this IRangeable<T> t, Expression<Func<T, object>> expr) where T : class, IDbObject
    {
        return AddOrderBy((QueryContent<T>)t, expr, true);
    }

    public static IRangeable<T> ThenByDescending<T>(this IRangeable<T> t, Expression<Func<T, object>> expr) where T : class, IDbObject
    {
        return AddOrderBy((QueryContent<T>)t, expr, false);
    }

    private static IRangeable<T> AddOrderBy<T>(QueryContent<T> me, Expression<Func<T, object>> expr, bool isAsc) where T : class, IDbObject
    {
        string n = GetColumnName(expr);
        if (me.m_order == null)
        {
            me.m_order = new OrderBy();
        }
        me.m_order.OrderItems.Add(isAsc ? new ASC(n) : new DESC(n));
        return me;
    }

    internal static MemberExpression GetMemberExpression(this LambdaExpression expr)
    {
        if (expr.Body is MemberExpression)
        {
            return (MemberExpression)expr.Body;
        }
        if (expr.Body is UnaryExpression)
        {
            return (MemberExpression)((UnaryExpression)expr.Body).Operand;
        }
        return null;
    }

    public static decimal? GetMax<T>(this IAfterWhere<T> t, Expression<Func<T, object>> expr) where T : class, IDbObject
    {
        string n = GetColumnName(expr);
        return ((QueryContent<T>)t).GetMax(n);
    }

    public static DateTime? GetMaxDate<T>(this IAfterWhere<T> t, Expression<Func<T, object>> expr) where T : class, IDbObject
    {
        string n = GetColumnName(expr);
        return ((QueryContent<T>)t).GetMaxDate(n);
    }

    public static decimal? GetMin<T>(this IAfterWhere<T> t, Expression<Func<T, object>> expr) where T : class, IDbObject
    {
        string n = GetColumnName(expr);
        return ((QueryContent<T>)t).GetMin(n);
    }

    public static DateTime? GetMinDate<T>(this IAfterWhere<T> t, Expression<Func<T, object>> expr) where T : class, IDbObject
    {
        string n = GetColumnName(expr);
        return ((QueryContent<T>)t).GetMinDate(n);
    }

    public static decimal? GetSum<T>(this IAfterWhere<T> t, Expression<Func<T, object>> expr) where T : class, IDbObject
    {
        string n = GetColumnName(expr);
        return ((QueryContent<T>)t).GetSum(n);
    }

    private static string GetColumnName<T>(Expression<Func<T, object>> expr) where T : class, IDbObject
    {
        MemberExpression e = expr.GetMemberExpression();
        if (e != null)
        {
            string n = ExpressionParser<T>.GetColumnName(e.Member.Name);
            return n;
        }
        throw new LinqException("get column name error!");
    }

    public static T GetObject<T>(this DbContext c, Expression<Func<T, bool>> expr) where T : class, IDbObject
    {
        var wc = ExpressionParser<T>.Parse(expr);
        return c.GetObject<T>(wc);
    }

    public static int Delete<T>(this DbContext c, Expression<Func<T, bool>> expr) where T : class, IDbObject
    {
        var wc = ExpressionParser<T>.Parse(expr);
        return c.Delete<T>(wc);
    }

    #endregion

    #region Web

    public static void AddAndCondition<T>(this DbEntryDataSource<T> ds, Expression<Func<T, bool>> condition) where T : class, IDbObject
    {
        var c = ExpressionParser<T>.Parse(condition);
        ds.Condition &= c;
    }

    #endregion

    #region String

    public static bool IsIndentityName(this string s)
    {
        return StringHelper.IsIndentityName(s);
    }

    public static string[] Split(this string s, char c, int count)
    {
        return StringHelper.Split(s, c, count);
    }

    public static string EnsureEndsWith(this string s, char c)
    {
        return StringHelper.EnsureEndsWith(s, c);
    }

    public static string EnsureEndsWith(this string s, string s1)
    {
        return StringHelper.EnsureEndsWith(s, s1);
    }

    public static string MultiLineAddPrefix(this string Source)
    {
        return StringHelper.MultiLineAddPrefix(Source);
    }

    public static string MultiLineAddPrefix(this string Source, string Prefix)
    {
        return StringHelper.MultiLineAddPrefix(Source, Prefix);
    }

    public static string MultiLineAddPrefix(this string Source, string Prefix, char SplitBy)
    {
        return StringHelper.MultiLineAddPrefix(Source, Prefix, SplitBy);
    }

    public static string GetLeft(this string s)
    {
        return StringHelper.GetStringLeft(s);
    }

    public static string GetLeft(this string s, int n)
    {
        return StringHelper.GetStringLeft(s, n);
    }

    public static string ToCString(this string s)
    {
        return StringHelper.GetCString(s);
    }

    public static int GetAnsiLength(this string s)
    {
        return StringHelper.GetAnsiLength(s);
    }

    public static string GetMultiByteSubString(this string s, int Count)
    {
        return StringHelper.GetMultiByteSubString(s, Count);
    }

    public static string Capitalize(this string s)
    {
        return StringHelper.Capitalize(s);
    }

    #endregion

    #region Misc

    public static byte[] Cut(this byte[] bs, int length)
    {
        return StringHelper.GetBytesByLength(bs, length);
    }

    public static string EnumToString(this Enum o)
    {
        return StringHelper.EnumToString(o);
    }

    public static string ReadToEnd(this Stream s)
    {
        return StringHelper.StreamReadToEnd(s);
    }

    public static string ReadToEnd(this Stream s, long Position)
    {
        return StringHelper.StreamReadToEnd(s, Position);
    }

    public static string ReadToEnd(this StreamReader s)
    {
        return StringHelper.StreamReadToEnd(s);
    }

    public static bool IsChildOf(this Type tc, Type tf)
    {
        return ClassHelper.IsChildOf(tf, tc);
    }

    public static string ToBase32String(this Guid guid)
    {
        var bs = guid.ToByteArray();
        return Base32StringCoding.Decode(bs);
    }

    public static string First(this string[] array)
    {
        if(array == null || array.Length == 0)
        {
            return null;
        }
        return array[0];
    }

    public static string Last(this string[] array)
    {
        if (array == null || array.Length == 0)
        {
            return null;
        }
        return array[array.Length - 1];
    }

    public static string[] RemoveFirst(this string[] array)
    {
        if(array == null)
        {
            return null;
        }
        if(array.Length == 0)
        {
            return array;
        }
        return new List<string>(array).RemoveFirst().ToArray();
    }

    public static string[] RemoveLast(this string[] array)
    {
        if (array == null)
        {
            return null;
        }
        if (array.Length == 0)
        {
            return array;
        }
        return new List<string>(array).RemoveLast().ToArray();
    }

    public static T First<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return default(T);
        }
        return list[0];
    }

    public static T Last<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return default(T);
        }
        return list[list.Count - 1];
    }

    public static List<T> RemoveFirst<T>(this List<T> list)
    {
        if(list == null)
        {
            return null;
        }
        if(list.Count == 0)
        {
            return list;
        }
        list.RemoveAt(0);
        return list;
    }

    public static List<T> RemoveLast<T>(this List<T> list)
    {
        if (list == null)
        {
            return null;
        }
        if (list.Count == 0)
        {
            return list;
        }
        list.RemoveAt(list.Count - 1);
        return list;
    }

    public static bool IsNullOrEmpty(this string s)
    {
        return string.IsNullOrEmpty(s);
    }

	#endregion
}
