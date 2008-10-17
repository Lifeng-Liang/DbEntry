using System;
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
                var attr = ClassHelper.GetAttribute<HttpParameterAttribute>(fieldInfo, false);
                if (attr != null)
                {
                    ProcessParamterInit(fieldInfo, attr.AllowEmpty);
                }
            }
            base.OnLoad(e);
        }

        private void ProcessParamterInit(FieldInfo fi, bool allowEmpty)
        {
            var s = Request[fi.Name];
            object px = GetValue(s, allowEmpty, fi.Name, fi.FieldType);
            fi.SetValue(this, px);
        }

        private static object GetValue(string s, bool allowEmpty, string name, Type type)
        {
            object px;
            if (string.IsNullOrEmpty(s))
            {
                if (!allowEmpty)
                {
                    throw new WebException(string.Format("The paramter {0} can't be empty", name));
                }
                if (type.IsValueType && type.IsGenericType)
                {
                    px = null;
                }
                else
                {
                    px = CommonHelper.GetEmptyValue(type);
                }
            }
            else
            {
                px = ClassHelper.ChangeType(s, type);
            }
            return px;
        }
    }
}
