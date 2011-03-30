using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Lephone.Data.Definition;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Common;
using Lephone.Data.Model.Member;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Model.Linq
{
    public static class ExpressionParser<T>
    {
        private static readonly Dictionary<string, string> Jar;

        static ExpressionParser()
        {
            Jar = new Dictionary<string, string>();
            var ctx = ModelContext.GetInstance(typeof(T));
            foreach (MemberHandler m in ctx.Info.Members)
            {
                string key = m.MemberInfo.Name;
                if(key.StartsWith("$"))
                {
                    key = key.Substring(1);
                }
                Jar.Add(key, m.Name);
            }
        }

        public static string GetColumnName(string fieldName)
        {
            string s = Jar[fieldName];
            if (s != null)
            {
                return s;
            }
            throw new DataException("Can't find the field: " + fieldName);
        }

        public static Condition Parse(Expression<Func<T, bool>> expr)
        {
            return Parse(expr.Body);
        }

        private static Condition Parse(Expression expr)
        {
            if (expr is BinaryExpression)
            {
                return ParseBinary((BinaryExpression)expr);
            }
            if (expr is MethodCallExpression)
            {
                return ParseMethodCall((MethodCallExpression)expr);
            }
            if(expr is UnaryExpression)
            {
                return ParseUnary((UnaryExpression)expr);
            }
            if (IsBooleanFieldOrProperty(expr))
            {
                var key = GetColumnName(((MemberExpression)expr).Member.Name);
                return new KeyValueClause(key, true, CompareOpration.Equal, ColumnFunction.None);
            }
            throw new LinqException("Not supported operation!");
        }

        private static bool IsBooleanFieldOrProperty(Expression expr)
        {
            if (expr is MemberExpression)
            {
                var member = ((MemberExpression)expr);
                if(member.Member.MemberType == MemberTypes.Field || member.Member.MemberType == MemberTypes.Property)
                {
                    if(member.Type == typeof(bool))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static Condition ParseUnary(UnaryExpression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Not:
                    return !Parse(expr.Operand);
            }
            throw new LinqException("Not supported operation!");
        }

        private static Condition ParseBinary(BinaryExpression e)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Equal:
                    return GetClause(e, CompareOpration.Equal);
                case ExpressionType.GreaterThan:
                    return GetClause(e, CompareOpration.GreatThan);
                case ExpressionType.GreaterThanOrEqual:
                    return GetClause(e, CompareOpration.GreatOrEqual);
                case ExpressionType.LessThan:
                    return GetClause(e, CompareOpration.LessThan);
                case ExpressionType.LessThanOrEqual:
                    return GetClause(e, CompareOpration.LessOrEqual);
                case ExpressionType.NotEqual:
                    return GetClause(e, CompareOpration.NotEqual);
                case ExpressionType.AndAlso:
                    return Parse(e.Left) && Parse(e.Right);
                case ExpressionType.OrElse:
                    return Parse(e.Left) || Parse(e.Right);
                default:
                    throw new LinqException("Not supported operation!");
            }
        }

        private static Condition ParseMethodCall(MethodCallExpression e)
        {
            switch (e.Method.Name)
            {
                case "StartsWith":
                    return ParseLikeCall(e, "", "%");
                case "EndsWith":
                    return ParseLikeCall(e, "%", "");
                case "Contains":
                    return ParseLikeCall(e, "%", "%");
                case "In":
                case "InStatement":
                    return ParseInCall(e);
                case "IsNull":
                    return ParseNull(e, true);
                case "IsNotNull":
                    return ParseNull(e, false);
            }
            throw new LinqException("Unknown function : " + e.Method.Name);
        }

        private static Condition ParseNull(MethodCallExpression e, bool isNull)
        {
            ColumnFunction function;
            MemberExpression member;
            string key = GetMemberName(e.Arguments[0], out function, out member);
            return new KeyValueClause(
                new KeyValue(key, null, member.Type),
                isNull ? CompareOpration.Is : CompareOpration.IsNot);
        }

        private static Condition ParseInCall(MethodCallExpression e)
        {
            ColumnFunction function;
            MemberExpression member;
            string key = GetMemberName(e.Arguments[0], out function, out member);
            var list = new List<object>();
            var ie = GetRightValue(e.Arguments[1]);
            if (ie is IEnumerable)
            {
                foreach (var obj in (IEnumerable)GetRightValue(e.Arguments[1]))
                {
                    list.Add(obj);
                }
            }
            else
            {
                list.Add(ie);
            }
            return new InClause(key, list.ToArray());
        }

        private static Condition ParseLikeCall(MethodCallExpression e, string left, string right)
        {
            ColumnFunction function;
            MemberExpression member;
            string key = GetMemberName(e.Object, out function, out member);
            if(e.Arguments.Count == 1)
            {
                object value = GetRightValue(e.Arguments[0]);
                if (value != null && value.GetType() == typeof(string))
                {
                    return new KeyValueClause(key, left + value + right, CompareOpration.Like, function);
                }
            }
            throw new LinqException("'Like' clause only supported one Parameter and the Parameter should be string and not allow NULL.");
        }

        private static string GetColumnName(MemberExpression expr)
        {
            string mn = expr.Member.Name;
            if (expr.Expression is MemberExpression && mn == "Id")
            {
                mn = ((MemberExpression)expr.Expression).Member.Name;
            }
            return GetColumnName(mn);
        }

        public static string GetMemberName(Expression expr, out ColumnFunction function, out MemberExpression obj)
        {
            if (expr.NodeType == ExpressionType.Convert)
            {
                expr = ((UnaryExpression)expr).Operand;
            }
            if (expr is MemberExpression)
            {
                function = ColumnFunction.None;
                obj = (MemberExpression)expr;
                return GetColumnName(obj);
            }
            if(expr is MethodCallExpression)
            {
                var e = (MethodCallExpression) expr;
                if(e.Method.Name == "ToLower" && e.Object is MemberExpression)
                {
                    function = ColumnFunction.ToLower;
                    obj = (MemberExpression)e.Object;
                    return GetColumnName(obj);
                }
                if (e.Method.Name == "ToUpper" && e.Object is MemberExpression)
                {
                    function = ColumnFunction.ToUpper;
                    obj = (MemberExpression)e.Object;
                    return GetColumnName(obj);
                }
            }
            throw new LinqException("The expression must be 'Column op const' or 'Column op Column'");
        }

        private static Condition GetClause(BinaryExpression e, CompareOpration co)
        {
            ColumnFunction function;
            MemberExpression left;
            var key = GetMemberName(e.Left, out function, out left);
            string pn = left.Expression.ToString();

            if (e.Right.NodeType == ExpressionType.MemberAccess)
            {
                var right = (MemberExpression)e.Right;
                if (right.Expression != null && right.Expression.ToString() == pn)
                {
                    string key2 = GetColumnName(right.Member.Name);
                    return new KeyKeyClause(key, key2, co);
                }
            }

            object value = GetRightValue(e.Right);

            if (value == null)
            {
                if (co == CompareOpration.Equal)
                {
                    return new KeyValueClause(key, null, CompareOpration.Is, ColumnFunction.None);
                }
                if (co == CompareOpration.NotEqual)
                {
                    return new KeyValueClause(key, null, CompareOpration.IsNot, ColumnFunction.None);
                }
                throw new LinqException("NULL value only supported Equal and NotEqual!");
            }
            return new KeyValueClause(key, value, co, function);
        }

        private static object GetRightValue(Expression right)
        {
            object value 
                = right.NodeType == ExpressionType.Constant 
                      ? ((ConstantExpression)right).Value 
                      : Expression.Lambda(right).Compile().DynamicInvoke();

            //else if (Right.NodeType == ExpressionType.Convert
            //    || Right.NodeType == ExpressionType.MemberAccess)
            //{
            //    value = Expression.Lambda(Right).Compile().DynamicInvoke();
            //}
            //else
            //{
            //    throw new LinqException("Unsupported expression.");
            //}

            return value;
        }
    }
}