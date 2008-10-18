using System;
using System.Reflection;
using System.Web.UI;
using Lephone.Util;

namespace Lephone.Web
{
    public class SmartMasterPageBase : MasterPage
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
            object px = SmartPageBase.GetValue(s, allowEmpty, fi.Name, fi.FieldType);
            fi.SetValue(this, px);
        }
    }
}
