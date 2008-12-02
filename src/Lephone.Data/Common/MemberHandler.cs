using System;
using System.Reflection;
using Lephone.Util;
using Lephone.Data.Definition;
using Comm = Lephone.Data.Common;
using Lephone.Util.Text;

namespace Lephone.Data.Common
{
	public class MemberHandler
    {
        #region MemberHandler Impl

        public class EnumMemberHandler : MemberHandler
        {
            public EnumMemberHandler(MemberAdapter fi)
                : base(fi)
            {
            }

            internal EnumMemberHandler(MemberAdapter fi, FieldType ft, PropertyInfo pi)
                : base(fi, ft, pi)
            {
            }

            protected override void InnerSetValue(object obj, object value)
            {
                MemberInfo.SetValue(obj, Enum.Parse(MemberInfo.MemberType, value.ToString()));
            }
        }

        public class BooleanMemberHandler : MemberHandler
        {
            public BooleanMemberHandler(MemberAdapter fi)
                : base(fi)
            {
            }

            internal BooleanMemberHandler(MemberAdapter fi, FieldType ft, PropertyInfo pi)
                : base(fi, ft, pi)
            {
            }

            protected override void InnerSetValue(object obj, object value)
            {
                MemberInfo.SetValue(obj, Convert.ToBoolean(value));
            }
        }

        public class NullableBooleanMemberHandler : NullableMemberHandler
        {
            public NullableBooleanMemberHandler(MemberAdapter fi)
                : base(fi)
            {
            }

            internal NullableBooleanMemberHandler(MemberAdapter fi, FieldType ft, PropertyInfo pi)
                : base(fi, ft, pi)
            {
            }

            protected override void InnerSetValue(object obj, object value)
            {
                object oo = ci.Invoke(new object[] { Convert.ToBoolean(value) });
                MemberInfo.SetValue(obj, oo);
            }
        }

        public class NullableMemberHandler : MemberHandler
        {
            protected readonly ConstructorInfo ci;
            protected readonly PropertyInfo NullableField;

            public NullableMemberHandler(MemberAdapter fi)
                : base(fi)
            {
                ci = NullableHelper.GetConstructorInfo(fi.MemberType);
                NullableField = fi.MemberType.GetProperty("Value");
            }

            internal NullableMemberHandler(MemberAdapter fi, FieldType ft, PropertyInfo pi)
                : base(fi, ft, pi)
            {
                ci = NullableHelper.GetConstructorInfo(fi.MemberType);
                NullableField = fi.MemberType.GetProperty("Value");
            }

            protected override void InnerSetValue(object obj, object value)
            {
                if (value != null)
                {
                    if (NullableField.PropertyType == typeof(Date) && value.GetType() != typeof(DateTime))
                    {
                        value = (Date) (DateTime) value;
                    }
                    else if (NullableField.PropertyType == typeof(Time) && value.GetType() != typeof(DateTime))
                    {
                        value = (Time) (DateTime) value;
                    }
                    else if (NullableField.PropertyType == typeof (Guid) && value.GetType() != typeof (Guid))
                    {
                        value = new Guid(value.ToString());
                    }
                }
                object oo = ci.Invoke(new[] { value });
                MemberInfo.SetValue(obj, oo);
            }

            public override object GetValue(object obj)
            {
                object r = MemberInfo.GetValue(obj);
                if (r == null)
                {
                    return null;
                }
                return NullableField.GetValue(r, null);
            }
        }

        #endregion

        public readonly MemberAdapter MemberInfo;
        public readonly string Name;
        public readonly bool IsKey;
        public readonly bool IsDbGenerate;
        public readonly object UnsavedValue;
        public readonly bool AllowNull;
        public readonly int MinLength;
        public readonly int MaxLength;
	    public readonly string LengthErrorMessage;
        public readonly bool IsUnicode;
        public readonly string Regular;
	    public readonly string RegularErrorMessage;
        public readonly bool IsHasOne;
        public readonly bool IsBelongsTo;
        public readonly bool IsHasMany;
        public readonly bool IsHasAndBelongsToMany;
	    public readonly string CrossTableName;
        public readonly bool IsLazyLoad;
        public readonly bool IsCreatedOn;
        public readonly bool IsUpdatedOn;
        public readonly bool IsSavedOn;
        public readonly bool IsLockVersion;
        public readonly bool IsCount;
        public readonly bool IsAutoSavedValue;
	    public readonly bool IsSimpleField;
	    public readonly bool IsRelationField;
        public readonly string OrderByString;
	    public readonly string UniqueErrorMessage;

        public Type FieldType
        {
            get { return MemberInfo.MemberType; }
        }

        protected MemberHandler(MemberAdapter fi)
        {
            var fn = fi.GetAttribute<DbColumnAttribute>(false);
            string memberName = (fn == null) ? fi.Name : fn.Name;

            MemberInfo = fi;
            Name = memberName;

            var dk = fi.GetAttribute<DbKeyAttribute>(false);
            if (dk != null)
            {
                IsKey = true;
                if (dk.IsDbGenerate)
                {
                    IsDbGenerate = true;
                }
                if (dk.UnsavedValue == null && dk.IsDbGenerate)
                {
                    UnsavedValue = CommonHelper.GetEmptyValue(fi.MemberType, false, "Unknown type of db key must set UnsavedValue");
                    if (fi.MemberType == typeof(Guid))
                    {
                        IsDbGenerate = false;
                    }
                }
                else
                {
                    UnsavedValue = dk.UnsavedValue;
                }
            }

            if (fi.MemberType.IsGenericType)
            {
                Type t = typeof(HasOne<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t)
                {
                    IsHasOne = true;
                }
                Type t0 = typeof(HasMany<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t0)
                {
                    IsHasMany = true;
                }
                Type t1 = typeof(BelongsTo<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t1)
                {
                    IsBelongsTo = true;
                    if (fn == null)
                    {
                        Type ot = fi.MemberType.GetGenericArguments()[0];
                        string n = ObjectInfo.GetObjectFromClause(ot).GetMainTableName();
                        Name = NameMapper.Instance.UnmapName(n) + "_Id";
                    }
                }
                Type t2 = typeof(HasAndBelongsToMany<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t2)
                {
                    IsHasAndBelongsToMany = true;
                    var ctn = fi.GetAttribute<CrossTableNameAttribute>(false);
                    if(ctn != null)
                    {
                        CrossTableName = ctn.Name;
                    }
                    if (fn == null)
                    {
                        Type ot1 = fi.MemberType.GetGenericArguments()[0];
                        string n1 = ObjectInfo.GetObjectFromClause(ot1).GetMainTableName();
                        Name = NameMapper.Instance.UnmapName(n1) + "_Id";
                    }
                }
                Type t3 = typeof(LazyLoadField<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t3)
                {
                    IsLazyLoad = true;
                }
            }

            if (fi.GetAttribute<AllowNullAttribute>(false) != null || NullableHelper.IsNullableType(fi.MemberType))
            {
                AllowNull = true;
            }

            if (fi.GetAttribute<SpecialNameAttribute>(false) != null)
            {
                if (fi.Name == "CreatedOn")
                {
                    if (fi.MemberType == typeof(DateTime))
                    {
                        IsCreatedOn = true;
                    }
                    else
                    {
                        throw new DataException("CreatedOn must be datetime type.");
                    }
                }
                else if (fi.Name == "UpdatedOn")
                {
                    if (fi.MemberType == typeof(DateTime?))
                    {
                        IsUpdatedOn = true;
                    }
                    else
                    {
                        throw new DataException("UpdatedOn must be nullable datetime type.");
                    }
                }
                else if (fi.Name == "SavedOn")
                {
                    if (fi.MemberType == typeof(DateTime))
                    {
                        IsSavedOn = true;
                    }
                    else
                    {
                        throw new DataException("SavedOn must be datetime type.");
                    }
                }
                else if (fi.Name == "LockVersion")
                {
                    if (fi.MemberType == typeof(int))
                    {
                        IsLockVersion = true;
                    }
                    else
                    {
                        throw new DataException("LockVersion must be int type.");
                    }
                }
                else if (fi.Name == "Count")
                {
                    if(fi.MemberType == typeof(int))
                    {
                        IsCount = true;
                    }
                    else
                    {
                        throw new DataException("Count must be int type.");
                    }
                }
                else
                {
                    throw new DataException("Only CreatedOn and UpdatedOn are supported as special name.");
                }
                if (IsCreatedOn || IsUpdatedOn || IsSavedOn || IsCount || IsLockVersion)
                {
                    IsAutoSavedValue = true;
                }
            }

            var lengthAttribute = fi.GetAttribute<LengthAttribute>(false);
            if (lengthAttribute != null)
            {
                if (fi.MemberType.IsSubclassOf(typeof(ValueType)))
                {
                    throw new DataException("ValueType couldn't set LengthAttribute!");
                }
                MinLength = lengthAttribute.Min;
                MaxLength = lengthAttribute.Max;
                LengthErrorMessage = lengthAttribute.ErrorMessage;
            }

            if (fi.MemberType == typeof(string) ||
                (IsLazyLoad && fi.MemberType.GetGenericArguments()[0] == typeof(string)))
            {
                IsUnicode = true;
            }
            var sf = fi.GetAttribute<StringColumnAttribute>(false);
            if (sf != null)
            {
                if (!(fi.MemberType == typeof(string) || (IsLazyLoad && fi.MemberType.GetGenericArguments()[0] == typeof(string))))
                {
                    throw new DataException("StringFieldAttribute must set for String Type Field!");
                }
                IsUnicode = sf.IsUnicode;
                Regular = sf.Regular;
                RegularErrorMessage = sf.ErrorMessage;
            }
            var os = fi.GetAttribute<OrderByAttribute>(false);
            if (os != null)
            {
                OrderByString = os.OrderBy;
            }
            //TODO: if the column of object really have multiple index attirbute, the current process is right?
            var indexs = fi.GetAttributes<IndexAttribute>(false);
            foreach (var index in indexs)
            {
                if(index.UNIQUE && !string.IsNullOrEmpty(index.UniqueErrorMessage))
                {
                    UniqueErrorMessage = index.UniqueErrorMessage;
                    break;
                }
            }
            IsRelationField = (IsHasOne || IsHasMany || IsHasAndBelongsToMany || IsBelongsTo);
            IsSimpleField = !(IsRelationField || IsLazyLoad);
        }

        internal MemberHandler(MemberAdapter fi, FieldType ft, PropertyInfo pi)
        {
            MemberInfo = fi;
            Name = "";

            IsHasOne = (ft == Comm.FieldType.HasOne);
            IsHasMany = (ft == Comm.FieldType.HasMany);
            IsHasAndBelongsToMany = (ft == Comm.FieldType.HasAndBelongsToMany);
            IsBelongsTo = (ft == Comm.FieldType.BelongsTo);
            IsLazyLoad = (ft == Comm.FieldType.LazyLoad);
            var obs = (OrderByAttribute[])pi.GetCustomAttributes(typeof(OrderByAttribute), true);
            if (obs != null && obs.Length > 0)
            {
                OrderByString = obs[0].OrderBy;
            }
            IsRelationField = (IsHasOne || IsHasMany || IsHasAndBelongsToMany || IsBelongsTo);
            IsSimpleField = !(IsRelationField || IsLazyLoad);
        }

        public void SetValue(object obj, object value)
		{
			if ( value == DBNull.Value )
			{
				MemberInfo.SetValue(obj, null);
			}
			else
			{
                InnerSetValue(obj, value);
			}
		}

        protected virtual void InnerSetValue(object obj, object value)
        {
            if (MemberInfo.MemberType == typeof(Date) && value.GetType() == typeof(DateTime))
            {
                value = (Date)(DateTime)value;
            }
            else if (MemberInfo.MemberType == typeof(Time) && value.GetType() == typeof(DateTime))
            {
                value = (Time)(DateTime)value;
            }
            else if (MemberInfo.MemberType == typeof(Guid) && value != null && value.GetType() != typeof(Guid))
            {
                value = new Guid(value.ToString());
            }
            MemberInfo.SetValue(obj, value);
        }

        public virtual object GetValue(object obj)
		{
            return MemberInfo.GetValue(obj);
		}

        public static MemberHandler NewObject(MemberAdapter fi)
        {
            if (fi.MemberType.IsEnum)
            {
                return new EnumMemberHandler(fi);
            }
            if (fi.MemberType == typeof(bool))
            {
                return new BooleanMemberHandler(fi);
            }
            if (NullableHelper.IsNullableType(fi.MemberType))
            {
                if(fi.MemberType.GetGenericArguments()[0] == typeof(bool))
                {
                    return new NullableBooleanMemberHandler(fi);
                }
                return new NullableMemberHandler(fi);
            }
            return new MemberHandler(fi);
        }

	    internal static MemberHandler NewObject(FieldInfo fi, FieldType ft, PropertyInfo pi)
        {
            MemberAdapter m = MemberAdapter.NewObject(fi);
            if (m.MemberType.IsEnum)
            {
                return new EnumMemberHandler(m, ft, pi);
            }
	        if (m.MemberType == typeof(bool))
	        {
	            return new BooleanMemberHandler(m, ft, pi);
	        }
	        if (NullableHelper.IsNullableType(m.MemberType))
	        {
                if (m.MemberType.GetGenericArguments()[0] == typeof(bool))
                {
                    return new NullableBooleanMemberHandler(m, ft, pi);
                }
                return new NullableMemberHandler(m, ft, pi);
	        }
	        return new MemberHandler(m, ft, pi);
        }
    }
}
