using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Leafing.Web.Mvc.Core
{
    public static class LinkHelper
    {
        public static LinkToInfo LinkTo(string controllerName)
        {
            return new LinkToInfo(controllerName);
        }

        public static LinkToInfo LinkTo<T>(Expression<Action<T>> expr = null) where T : ControllerBase
        {
            var cn = GetControllerNameByType(typeof(T));
            var result = LinkTo(cn);
            if (expr != null)
            {
                var mce = (MethodCallExpression)expr.Body;
                var mn = mce.Method.Name.ToLower();
                result.Action(mn);
                if (mce.Arguments.Count > 0)
                {
                    var ps = GetParameters(mce);
                    if (ps.Count > 0)
                    {
                        result.Parameters(ps);
                    }
                }
            }
            return result;
        }

        public static UrlToInfo UrlTo(string controllerName)
        {
            return new UrlToInfo(controllerName);
        }

        public static UrlToInfo UrlTo<T>(Expression<Action<T>> expr = null) where T : ControllerBase
        {
            var cn = GetControllerNameByType(typeof(T));
            var result = UrlTo(cn);
            if (expr != null)
            {
                var mce = (MethodCallExpression)expr.Body;
                var mn = mce.Method.Name.ToLower();
                result.Action(mn);
                if (mce.Arguments.Count > 0)
                {
                    var ps = GetParameters(mce);
                    if (ps.Count > 0)
                    {
                        result.Parameters(ps);
                    }
                }
            }
            return result;
        }

        private static List<object> GetParameters(MethodCallExpression mce)
        {
            var ps = new List<object>();
            bool hasNull = false;
            foreach(var argument in mce.Arguments)
            {
                var value
                    = argument.NodeType == ExpressionType.Constant
                          ? ((ConstantExpression)argument).Value
                          : Expression.Lambda(argument).Compile().DynamicInvoke();
                if(value != null)
                {
                    if(hasNull)
                    {
                        throw new WebException("Can not pass NULL in the middle of the parameters");
                    }
                    ps.Add(value);
                }
                else
                {
                    hasNull = true;
                }
            }
            return ps;
        }

        public static string GetControllerNameByType(Type type)
        {
            string cn = type.Name;
            if (cn.EndsWith("Controller"))
            {
                cn = cn.Substring(0, cn.Length - 10);
            }
            return cn.ToLower();
        }
    }
}
