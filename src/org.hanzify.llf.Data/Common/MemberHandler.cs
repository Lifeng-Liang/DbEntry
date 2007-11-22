
using System;
using System.Reflection;
using System.Reflection.Emit;

using Lephone.Util;
using Lephone.Data.Definition;
using Comm = Lephone.Data.Common;

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

        public class NullableMemberHandler : MemberHandler
        {
            private ConstructorInfo ci;
            private PropertyInfo NullableField;

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
                object oo = ci.Invoke(new object[] { value });
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
        public readonly bool IsUnicode;
        public readonly string Regular;
        public readonly bool IsHasOne;
        public readonly bool IsBelongsTo;
        public readonly bool IsHasMany;
        public readonly bool IsHasAndBelongsToMany;
        public readonly bool IsLazyLoad;
        public readonly bool IsCreatedOn;
        public readonly bool IsUpdatedOn;
        public readonly bool IsSavedOn;
        public readonly bool IsLockVersion;
        public readonly bool IsAutoSavedValue;
        public readonly string OrderByString = null;

        public Type FieldType
        {
            get { return MemberInfo.MemberType; }
        }

        protected MemberHandler(MemberAdapter fi)
        {
            DbColumnAttribute fn = fi.GetAttribute<DbColumnAttribute>(false);
            string Name = (fn == null) ? fi.Name : fn.Name;

            this.MemberInfo = fi;
            this.Name = Name;

            DbKeyAttribute dk = fi.GetAttribute<DbKeyAttribute>(false);
            if (dk != null)
            {
                this.IsKey = true;
                if (dk.IsDbGenerate)
                {
                    this.IsDbGenerate = true;
                }
                if (dk.UnsavedValue == null && dk.IsDbGenerate)
                {
                    this.UnsavedValue = CommonHelper.GetEmptyValue(fi.MemberType, false, "Unknown type of db key must set UnsavedValue");
                    if (fi.MemberType == typeof(Guid))
                    {
                        this.IsDbGenerate = false;
                    }
                }
                else
                {
                    this.UnsavedValue = dk.UnsavedValue;
                }
            }

            if (fi.MemberType.IsGenericType)
            {
                Type t = typeof(HasOne<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t)
                {
                    this.IsHasOne = true;
                }
                Type t0 = typeof(HasMany<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t0)
                {
                    this.IsHasMany = true;
                }
                Type t1 = typeof(BelongsTo<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t1)
                {
                    this.IsBelongsTo = true;
                    if (fn == null)
                    {
                        Type ot = fi.MemberType.GetGenericArguments()[0];
                        string n = DbObjectHelper.GetObjectFromClause(ot).GetMainTableName();
                        this.Name = n + "_Id";
                    }
                }
                Type t2 = typeof(HasAndBelongsToMany<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t2)
                {
                    this.IsHasAndBelongsToMany = true;
                    if (fn == null)
                    {
                        Type ot1 = fi.MemberType.GetGenericArguments()[0];
                        string n1 = DbObjectHelper.GetObjectFromClause(ot1).GetMainTableName();
                        this.Name = n1 + "_Id";
                    }
                }
                Type t3 = typeof(LazyLoadField<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t3)
                {
                    this.IsLazyLoad = true;
                }
            }

            if (fi.GetAttribute<AllowNullAttribute>(false) != null || NullableHelper.IsNullableType(fi.MemberType))
            {
                this.AllowNull = true;
            }

            if (fi.GetAttribute<SpecialNameAttribute>(false) != null)
            {
                if (fi.Name == "CreatedOn")
                {
                    if (fi.MemberType == typeof(DateTime))
                    {
                        this.IsCreatedOn = true;
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
                        this.IsUpdatedOn = true;
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
                        this.IsSavedOn = true;
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
                        this.IsLockVersion = true;
                    }
                    else
                    {
                        throw new DataException("LockVersion must be int type.");
                    }
                }
                else
                {
                    throw new DataException("Only CreatedOn and UpdatedOn are supported as special name.");
                }
                if (this.IsCreatedOn || this.IsUpdatedOn || this.IsSavedOn)
                {
                    this.IsAutoSavedValue = true;
                }
            }

            LengthAttribute ml = fi.GetAttribute<LengthAttribute>(false);
            if (ml != null)
            {
                if (fi.MemberType.IsSubclassOf(typeof(ValueType)))
                {
                    throw new DataException("ValueType couldn't set MaxLengthAttribute!");
                }
                this.MinLength = ml.Min;
                this.MaxLength = ml.Max;
            }

            if (fi.MemberType == typeof(string) ||
                (this.IsLazyLoad && fi.MemberType.GetGenericArguments()[0] == typeof(string)))
            {
                this.IsUnicode = true;
            }
            StringColumnAttribute sf = fi.GetAttribute<StringColumnAttribute>(false);
            if (sf != null)
            {
                if (!(fi.MemberType == typeof(string) || (this.IsLazyLoad && fi.MemberType.GetGenericArguments()[0] == typeof(string))))
                {
                    throw new DataException("StringFieldAttribute must set for String Type Field!");
                }
                this.IsUnicode = sf.IsUnicode;
                this.Regular = sf.Regular;
            }
            OrderByAttribute os = fi.GetAttribute<OrderByAttribute>(false);
            if (os != null)
            {
                this.OrderByString = os.OrderBy;
            }
        }

        internal MemberHandler(MemberAdapter fi, FieldType ft, PropertyInfo pi)
        {
            this.MemberInfo = fi;
            this.Name = "";

            IsHasOne = (ft == Comm.FieldType.HasOne);
            IsHasMany = (ft == Comm.FieldType.HasMany);
            IsHasAndBelongsToMany = (ft == Comm.FieldType.HasAndBelongsToMany);
            IsBelongsTo = (ft == Comm.FieldType.BelongsTo);
            IsLazyLoad = (ft == Comm.FieldType.LazyLoad);
            OrderByAttribute[] obs = (OrderByAttribute[])pi.GetCustomAttributes(typeof(OrderByAttribute), true);
            if (obs != null && obs.Length > 0)
            {
                OrderByString = obs[0].OrderBy;
            }
        }

        public void SetValue(object obj, object value)
		{
			if ( value == System.DBNull.Value )
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
            else if (fi.MemberType == typeof(bool))
            {
                return new BooleanMemberHandler(fi);
            }
            else if (NullableHelper.IsNullableType(fi.MemberType))
            {
                return new NullableMemberHandler(fi);
            }
            else
            {
                return new MemberHandler(fi);
            }
        }

        internal static MemberHandler NewObject(FieldInfo fi, FieldType ft, PropertyInfo pi)
        {
            MemberAdapter m = MemberAdapter.NewObject(fi);
            if (m.MemberType.IsEnum)
            {
                return new EnumMemberHandler(m, ft, pi);
            }
            else if (m.MemberType == typeof(bool))
            {
                return new BooleanMemberHandler(m, ft, pi);
            }
            else if (NullableHelper.IsNullableType(m.MemberType))
            {
                return new NullableMemberHandler(m, ft, pi);
            }
            else
            {
                return new MemberHandler(m, ft, pi);
            }
        }
    }
}
