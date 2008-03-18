
#region usings

using System;
using System.Reflection;

using Lephone.Util.Text;

#endregion

namespace Lephone.Util.Setting
{
	public abstract class ConfigHelperBase
	{
		public ConfigHelperBase() {}

        public void InitClass(Type t)
        {
            InitClass(ClassHelper.StaticFlag, t, null);
        }

        public void InitClass(object obj)
		{
            InitClass(ClassHelper.AllFlag, obj.GetType(), obj);
		}

        private void InitClass(BindingFlags bFlag, Type t, object obj)
        {
            FieldInfo[] fis = t.GetFields(bFlag);

            foreach (FieldInfo fi in fis)
            {
                ShowStringAttribute[] ss = (ShowStringAttribute[])fi.GetCustomAttributes(typeof(ShowStringAttribute), false);
                string Name = (ss != null && ss.Length == 1) ? ss[0].ShowString : fi.Name;
                object o = fi.GetValue(obj);
                o = GetValue(Name, o);
                fi.SetValue(obj, o);
            }
        }

		public string GetValue(string key)
		{
			return (string)GetValue(key, (object)"");
		}

		public string GetValue(string key, string defaultValue)
		{
			return (string)GetValue(key, (object)defaultValue);
		}

		public int GetValue(string key, int defaultValue)
		{
			return (int)GetValue(key, (object)defaultValue);
		}

		public long GetValue(string key, long defaultValue)
		{
			return (long)GetValue(key, (object)defaultValue);
		}

		public bool GetValue(string key, bool defaultValue)
		{
			return bool.Parse(GetValue(key, (object)defaultValue).ToString());
		}

        public float GetValue(string key, float defaultValue)
        {
            return float.Parse(GetValue(key, (object)defaultValue).ToString());
        }

        public double GetValue(string key, double defaultValue)
        {
            return double.Parse(GetValue(key, (object)defaultValue).ToString());
        }

		public DateTime GetValue(string key, DateTime defaultValue)
		{
			return DateTime.Parse(GetValue(key, (object)defaultValue).ToString());
		}

        public Date GetValue(string key, Date defaultValue)
        {
            return Date.Parse(GetValue(key, (object)defaultValue).ToString());
        }

        public Time GetValue(string key, Time defaultValue)
        {
            return Time.Parse(GetValue(key, (object)defaultValue).ToString());
        }

        public object GetValue(string key, object defaultValue)
		{
			string s = GetString(key);
			if( s == null)
			{
				return defaultValue;
			}
            Type t = defaultValue.GetType();
			if( t.IsSubclassOf(typeof(System.Enum)) )
			{
				return Enum.Parse(t, s);
			}
            return ClassHelper.ChangeType(s, t);
		}

		protected abstract string GetString(string key);
	}
}
