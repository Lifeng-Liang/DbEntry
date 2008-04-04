
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Common;
using System.Collections;

namespace Lephone.Linq
{
    internal static class ExpressionParser<T> where T : IDbObject
    {
        private static Dictionary<string, string> dic;

        static ExpressionParser()
        {
            dic = new Dictionary<string, string>();
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            foreach (MemberHandler m in oi.Fields)
            {
                dic.Add(m.MemberInfo.Name, m.Name);
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
            else if (expr is MethodCallExpression)
            {
                return ParseMethodCall((MethodCallExpression)expr);
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
            if (e.Arguments.Count == 1 && e.Object is MemberExpression)
            {
                string key = GetColumnName(((MemberExpression)e.Object).Member.Name);
                object value = GetRightValue(e.Arguments[0]);
                if (value != null && value.GetType() == typeof(string))
                {
                    switch (e.Method.Name)
                    {
                        case "StartsWith":
                            return new KeyValueClause(key, value + "%", CompareOpration.Like);
                        case "EndsWith":
                            return new KeyValueClause(key, "%" + value, CompareOpration.Like);
                        case "Contains":
                            return new KeyValueClause(key, "%" + value + "%", CompareOpration.Like);
                    }
                }
            }
            throw new LinqException("'Like' clause only supported one paramter and the paramter should be string and not allow NULL.");
        }

        private static WhereCondition GetClause(BinaryExpression e, CompareOpration co)
        {
            Expression l = e.Left;
            if (l.NodeType == ExpressionType.Convert)
            {
                l = ((UnaryExpression)l).Operand;
            }
            if (l.NodeType == ExpressionType.MemberAccess)
            {
                var left = (MemberExpression)l;
                string pn = left.Expression.ToString();
                string key = GetColumnName(left.Member.Name);

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
                        return new KeyValueClause(key, null, CompareOpration.Is);
                    }
                    else if (co == CompareOpration.NotEqual)
                    {
                        return new KeyValueClause(key, null, CompareOpration.IsNot);
                    }
                    throw new LinqException("NULL value only supported Equal and NotEqual!");
                }
                return new KeyValueClause(key, value, co);
            }
            throw new LinqException("The expression must be 'Column op const' or 'Column op Column'");
        }

        private static object GetRightValue(Expression Right)
        {
            object value;

            if (Right.NodeType == ExpressionType.Constant)
            {
                value = ((ConstantExpression)Right).Value;
            }
            else
            {
                value = Expression.Lambda(Right).Compile().DynamicInvoke();
            }

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
