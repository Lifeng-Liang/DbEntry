using System;
using System.Reflection;
using System.Web.UI;
using Lephone.Core;

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
                    ProcessParameterInit(fieldInfo, attr.AllowEmpty);
                }
                var inMaster = ClassHelper.GetAttribute<InMasterAttribute>(fieldInfo, false);
                if (inMaster != null)
                {
                    ProcessInMasterInit(fieldInfo);
                }
            }
            base.OnLoad(e);
        }

        private void ProcessParameterInit(FieldInfo fi, bool allowEmpty)
        {
            var s = Request[fi.Name];
            object px = SmartPageBase.GetValue(s, allowEmpty, fi.Name, fi.FieldType);
            fi.SetValue(this, px);
        }

        private void ProcessInMasterInit(FieldInfo info)
        {
            var name = info.Name;
            var fi = Master.GetType().GetField(name, ClassHelper.AllFlag);
            if (fi == null)
            {
                throw new WebException("Don't have field '{0}' in the master page", name);
            }
            object v = fi.GetValue(Master);
            info.SetValue(this, v);
        }
    }
}
