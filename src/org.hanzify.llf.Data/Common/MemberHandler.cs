
#region usings

using System;
using System.Reflection;

using org.hanzify.llf.util;
using org.hanzify.llf.Data.Common;

#endregion

namespace org.hanzify.llf.Data.Common
{
	internal class MemberHandler
    {
        #region MemberHandler Impl

        public class EnumMemberHandler : MemberHandler
        {
            public EnumMemberHandler(MemberAdapter fi, string Name)
                : base(fi, Name)
            {
            }

            protected override void InnerSetValue(object obj, object value)
            {
                fi.SetValue(obj, Enum.Parse(fi.MemberType, value.ToString()));
            }
        }

        public class BooleanMemberHandler : MemberHandler
        {
            public BooleanMemberHandler(MemberAdapter fi, string Name)
                : base(fi, Name)
            {
            }

            protected override void InnerSetValue(object obj, object value)
            {
                fi.SetValue(obj, Convert.ToBoolean(value));
            }
        }

        public class NullableMemberHandler : MemberHandler
        {
            private ConstructorInfo ci;
            private PropertyInfo NullableField;

            public NullableMemberHandler(MemberAdapter fi, string Name)
                : base(fi, Name)
            {
                ci = NullableHelper.GetConstructorInfo(fi.MemberType);
                NullableField = fi.MemberType.GetProperty("Value");
            }

            protected override void InnerSetValue(object obj, object value)
            {
                object oo = ci.Invoke(new object[] { value });
                fi.SetValue(obj, oo);
            }

            public override object GetValue(object obj)
            {
                object r = fi.GetValue(obj);
                if (r == null)
                {
                    return null;
                }
                return NullableField.GetValue(r, null);
            }
        }

        #endregion

        protected MemberAdapter fi;

        public string Name;
        public bool IsKey;
        public bool IsSystemGeneration;
        public object UnsavedValue;
        public bool AllowNull;
        public int MaxLength;
        public bool IsUnicode;
        public string Regular;
        public bool IsHasOne;
        public bool IsBelongsTo;
        public bool IsHasMany;
        public bool IsHasAndBelongsToMany;
        public bool IsLazyLoad;
        public string OrderByString = null;

        public MemberAdapter MemberInfo
        {
            get { return fi; }
        }

        public Type FieldType
        {
            get { return fi.MemberType; }
        }

        protected MemberHandler(MemberAdapter fi, string Name)
            : this(fi, Name, false, false, null, false, 0, false, null)
        {
        }

        private MemberHandler(MemberAdapter fi, string Name, bool IsKey, bool IsSystemGen,
            object UnsavedValue, bool AllowNull, int MaxLength, bool IsUnicode, string Regular)
		{
			this.fi = fi;
            this.Name = Name;
            this.IsKey = IsKey;
            this.IsSystemGeneration = IsSystemGen;
            this.UnsavedValue = UnsavedValue;
            this.AllowNull = AllowNull;
            this.MaxLength = MaxLength;
            this.IsUnicode = IsUnicode;
            this.Regular = Regular;
        }

        public void SetValue(object obj, object value)
		{
			if ( value == System.DBNull.Value )
			{
				fi.SetValue(obj, null);
			}
			else
			{
                InnerSetValue(obj, value);
			}
		}

        protected virtual void InnerSetValue(object obj, object value)
        {
            fi.SetValue(obj, value);
        }

        public virtual object GetValue(object obj)
		{
            return fi.GetValue(obj);
		}

        public static MemberHandler NewObject(MemberAdapter fi, string Name)
        {
            if (fi.MemberType.IsEnum)
            {
                return new EnumMemberHandler(fi, Name);
            }
            else if (fi.MemberType == typeof(bool))
            {
                return new BooleanMemberHandler(fi, Name);
            }
            else if (NullableHelper.IsNullableType(fi.MemberType))
            {
                return new NullableMemberHandler(fi, Name);
            }
            else
            {
                return new MemberHandler(fi, Name);
            }
        }
    }
}
