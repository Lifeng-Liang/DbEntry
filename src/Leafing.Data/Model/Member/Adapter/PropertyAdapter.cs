using System;
using System.Reflection;
using Leafing.Core;
using Leafing.Data.Model.Handler;
using Leafing.Data.Model.Handler.Generator;

namespace Leafing.Data.Model.Member.Adapter
{
    internal class PropertyAdapter : MemberAdapter
    {
        internal class PropertyHandler : IMemberHandler
        {
            private readonly PropertyInfo _info;

            public PropertyHandler(PropertyInfo info)
            {
                this._info = info;
            }

            public void SetValue(object obj, object value)
            {
                _info.SetValue(obj, value, null);
            }

            public object GetValue(object obj)
            {
                return _info.GetValue(obj, null);
            }
        }

        protected PropertyInfo Info;
        protected IMemberHandler Handler;

        public PropertyAdapter(PropertyInfo info)
		{
			this.Info = info;
			if (info.GetGetMethod ().IsPublic) {
				var t = MemberHandlerGenerator.Generate (info.DeclaringType, info);
				Handler = (IMemberHandler)ClassHelper.CreateInstance (t);
			} else {
				Handler = new PropertyHandler (info);
			}
		}

        public override Type MemberType
        {
            get { return Info.PropertyType; }
        }

        public override void SetValue(object obj, object value)
        {
            Handler.SetValue(obj, value);
        }

        public override object GetValue(object obj)
        {
            return Handler.GetValue(obj);
        }

        public override T[] GetAttributes<T>(bool inherit)
        {
            return Info.GetAttributes<T>(inherit);
        }

        public override T GetAttribute<T>(bool inherit)
        {
            return Info.GetAttribute<T>(inherit);
        }

        public override bool HasAttribute<T>(bool inherit)
        {
            return Info.HasAttribute<T>(inherit);
        }

        public override string Name
        {
            get { return Info.Name; }
        }

        public override bool IsProperty
        {
            get { return true; }
        }

        public override MemberInfo GetMemberInfo()
        {
            return Info;
        }

        public override Type DeclaringType
        {
            get { return Info.DeclaringType; }
        }
    }
}
