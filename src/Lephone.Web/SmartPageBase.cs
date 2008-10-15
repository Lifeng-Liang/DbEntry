using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using Lephone.Util;

namespace Lephone.Web
{
    public class SmartPageBase : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            var t = GetType();
            var flag = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (var fieldInfo in t.GetFields(flag))
            {
                var a = ClassHelper.GetAttribute<HttpParameterAttribute>(fieldInfo, false);
                if (a != null)
                {
                    ProcessParamterInit(fieldInfo, a.AllowEmpty);
                }
            }
            foreach (var methodInfo in t.GetMethods(flag))
            {
                var a = ClassHelper.GetAttribute<PageLoadAttribute>(methodInfo, false);
                if(a != null)
                {
                    ProcessPageLoad(methodInfo);
                    break;
                }
            }
            base.OnLoad(e);
        }

        private void ProcessPageLoad(MethodInfo mi)
        {
            var pis = mi.GetParameters();
            var parameters = new List<object>();
            foreach (ParameterInfo pi in pis)
            {
                var s = Request[pi.Name];
                if(string.IsNullOrEmpty(s))
                {
                    throw new WebException(string.Format("The paramter {0} can' be empty", pi.Name));
                }
                object px = ClassHelper.ChangeType(s, pi.ParameterType);
                parameters.Add(px);
            }
            mi.Invoke(this, parameters.ToArray());
        }

        private void ProcessParamterInit(FieldInfo fi, bool allowEmpty)
        {
            var s = Request[fi.Name];
            if(string.IsNullOrEmpty(s) && !allowEmpty)
            {
                throw new WebException(string.Format("The paramter {0} can' be empty", fi.Name));
            }
            object px = ClassHelper.ChangeType(s, fi.FieldType);
            fi.SetValue(this, px);
        }
    }
}
