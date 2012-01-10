using System;
using System.Reflection;
using System.Web.UI;
using Leafing.Core;

namespace Leafing.Web
{
    public class SmartPageBase : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            var t = GetType();
            const BindingFlags flag = BindingFlags.DeclaredOnly 
                | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (var fieldInfo in t.GetFields(flag))
            {
                var attr = fieldInfo.GetAttribute<HttpParameterAttribute>(false);
                if (attr != null)
                {
                    ProcessParameterInit(fieldInfo, attr.AllowEmpty);
                }
                var inMaster = fieldInfo.GetAttribute<InMasterAttribute>(false);
                if(inMaster != null)
                {
                    ProcessInMasterInit(fieldInfo);
                }
            }
            base.OnLoad(e);
        }

        private void ProcessParameterInit(FieldInfo fi, bool allowEmpty)
        {
            var s = Request[fi.Name];
            object px = GetValue(s, allowEmpty, fi.Name, fi.FieldType);
            fi.SetValue(this, px);
        }

        private void ProcessInMasterInit(FieldInfo info)
        {
            if(Master != null)
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

        internal static object GetValue(string s, bool allowEmpty, string name, Type type)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (!allowEmpty)
                {
                    throw new WebException(string.Format("The Parameter {0} can't be empty", name));
                }
                if (type.IsValueType)
                {
                    if (type.IsGenericType)
                    {
                        return null;
                    }
                    return Util.GetEmptyValue(type);
                }
            }
            return ClassHelper.ChangeType(s, type);
        }
    }
}
