using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Common;

namespace Lephone.Linq
{
    internal static class ExpressionParser<T> where T : class, IDbObject
    {
        private static readonly Dictionary<string, string> dic;

        static ExpressionParser()
        {
            dic = new Dictionary<string, string>();
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            foreach (MemberHandler m in oi.Fields)
            {
                string key = m.MemberInfo.Name;
                if(key.StartsWith("$"))
                {
                    key = key.Substring(1);
                }
                dic.Add(key, m.Name);
            }
        }

        public static string GetColumnName(string FieldName)
        {
            string s = dic[FieldName];
            if (s != null)
            {
                return s;
            }
            throw new DataException("Can't find the field: " + FieldName);
        }

        public static WhereCondition Parse(Expression<Func<T, bool>> expr)
        {
            return Parse(expr.Body);
        }

        private static WhereCondition Parse(Expression expr)
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
            throw new LinqException("Not supported operation!");
        }

        private static WhereCondition ParseUnary(UnaryExpression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Not:
                    return !Parse(expr.Operand);
            }
            throw new LinqException("Not supported operation!");
        }

        private static WhereCondition ParseBinary(BinaryExpression e)
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

        private static WhereCondition ParseMethodCall(MethodCallExpression e)
        {
            if (e.Arguments.Count == 1)
            {
                ColumnFunction function;
                string key = GetMemberName(e.Object, out function);
                object value = GetRightValue(e.Arguments[0]);
                if (value != null && value.GetType() == typeof(string))
                {
                    switch (e.Method.Name)
                    {
                        case "StartsWith":
                            return new KeyValueClause(key, value + "%", CompareOpration.Like, function);
                        case "EndsWith":
                            return new KeyValueClause(key, "%" + value, CompareOpration.Like, function);
                        case "Contains":
                            return new KeyValueClause(key, "%" + value + "%", CompareOpration.Like, function);
                    }
                }
            }
            throw new LinqException("'Like' clause only supported one Parameter and the Parameter should be string and not allow NULL.");
        }

        private static string GetMemberName(Expression expr, out ColumnFunction function)
        {
            if(expr is MemberExpression)
            {
                function = ColumnFunction.None;
                return GetColumnName(((MemberExpression)expr).Member.Name);
            }
            if(expr is MethodCallExpression)
            {
                var e = (MethodCallExpression) expr;
                if(e.Method.Name == "ToLower" && e.Object is MemberExpression)
                {
                    function = ColumnFunction.ToLower;
                    return GetColumnName(((MemberExpression)e.Object).Member.Name);
                }
                if (e.Method.Name == "ToUpper" && e.Object is MemberExpression)
                {
                    function = ColumnFunction.ToUpper;
                    return GetColumnName(((MemberExpression)e.Object).Member.Name);
                }
            }
            throw new LinqException("'Like' clause only supported one Parameter and the Parameter should be string and not allow NULL.");
        }

        private static WhereCondition GetClause(BinaryExpression e, CompareOpration co)
        {
            Expression l = e.Left;
            if (l.NodeType == ExpressionType.Convert)
            {
                l = ((UnaryExpression)l).Operand;
            }
            ColumnFunction function = ColumnFunction.None;
            if(l is MethodCallExpression)
            {
                var x = (MethodCallExpression) l;
                if(x.Method.Name == "ToLower")
                {
                    l = x.Object;
                    function = ColumnFunction.ToLower;
                }
                else if (x.Method.Name == "ToUpper")
                {
                    l = x.Object;
                    function = ColumnFunction.ToUpper;
                }
            }
            if (l.NodeType == ExpressionType.MemberAccess)
            {
                var left = (MemberExpression)l;
                string pn = left.Expression.ToString();
                string mn = left.Member.Name;
                if(left.Expression is MemberExpression && mn == "Id")
                {
                    mn = ((MemberExpression)left.Expression).Member.Name;
                }
                string key = GetColumnName(mn);

                if (e.Right.NodeType == ExpressionType.MemberAccess)
                {
                    var right = (MemberExpression)e.Right;
                    if (right.Expression.ToString() == pn)
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
            throw new LinqException("The expression must be 'Column op const' or 'Column op Column'");
        }

        private static object GetRightValue(Expression Right)
        {
            object value 
                = Right.NodeType == ExpressionType.Constant 
                ? ((ConstantExpression)Right).Value 
                : Expression.Lambda(Right).Compile().DynamicInvoke();

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
