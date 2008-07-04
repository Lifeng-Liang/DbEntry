using System;
using System.IO;
using System.Linq.Expressions;
using Lephone.Data;
using Lephone.Data.QuerySyntax;
using Lephone.Data.Definition;
using Lephone.Linq;
using Lephone.Util;
using Lephone.Util.Text;

public static class CommonExtends
{
    #region Query

    public static IAfterWhere<T> Where<T>(this IWhere<T> t, Expression<Func<T, bool>> expr) where T : class, IDbObject
    {
        var me = (QueryContent<T>)t;
        me.m_where = ExpressionParser<T>.Parse(expr);
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

    private static IRangeable<T> AddOrderBy<T>(QueryContent<T> me, LambdaExpression expr, bool isAsc) where T : class, IDbObject
    {
        MemberExpression e = expr.GetMemberExpression();
        if (e != null)
        {
            string n = ExpressionParser<T>.GetColumnName(e.Member.Name);
            if (me.m_order == null)
            {
                me.m_order = new OrderBy();
            }
            me.m_order.OrderItems.Add(isAsc ? new ASC(n) : new DESC(n));
            return me;
        }
        throw new LinqException("OrderBy error!");
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

    public static T GetObject<T>(this DbContext c, Expression<Func<T, bool>> expr) where T : class, IDbObject
    {
        var wc = ExpressionParser<T>.Parse(expr);
        return c.GetObject<T>(wc);
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

	#endregion
}
