using System;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Definition;
using Lephone.Data.Common;

namespace Lephone.Data
{
    //[Serializable]
    //public class FieldNameGetter<T>
    //{
    //    private static readonly ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));

    //    public CK this[string fieldName]
    //    {
    //        get
    //        {
    //            foreach (MemberHandler m in oi.Fields)
    //            {
    //                if(m.IsLazyLoad)
    //                {
    //                    if(m.MemberInfo.Name == "$" + fieldName)
    //                    {
    //                        return new CK(m.Name);
    //                    }
    //                }
    //                else if (m.MemberInfo.Name == fieldName)
    //                {
    //                    return new CK(m.Name);
    //                }
    //            }
    //            throw new DataException("Can't find the field: " + fieldName);
    //        }
    //    }
    //}

    //[Serializable]
    //public class CK<T>
    //{
    //    private static readonly FieldNameGetter<T> _Field = new FieldNameGetter<T>();

    //    public static FieldNameGetter<T> Field
    //    {
    //        get { return _Field; }
    //    }
    //}

	[Serializable]
	public class CK
	{
		private static readonly CK Col = new CK("");

		public CK this[string columnName]
		{
			get { return new CK(columnName); }
		}

		public static CK K
		{
			get { return Col; }
        }

        public static CK Column
        {
            get { return Col; }
        }

	    private ColumnFunction _function;

        public readonly string ColumnName;

        public CK(string columnName)
        {
            this.ColumnName = columnName;
        }

        #region KeyValue

        public static KeyValueClause operator > (CK key, object value)
		{
			return new KeyValueClause(key.ColumnName, value, CompareOpration.GreatThan, key._function);
		}

        public KeyValueClause Gt(object value)
        {
            return new KeyValueClause(ColumnName, value, CompareOpration.GreatThan, _function);
        }

		public static KeyValueClause operator < (CK key, object value)
		{
            return new KeyValueClause(key.ColumnName, value, CompareOpration.LessThan, key._function);
		}

        public KeyValueClause Lt(object value)
        {
            return new KeyValueClause(ColumnName, value, CompareOpration.LessThan, _function);
        }

        public static KeyValueClause operator >=(CK key, object value)
		{
            return new KeyValueClause(key.ColumnName, value, CompareOpration.GreatOrEqual, key._function);
		}

        public KeyValueClause Ge(object value)
        {
            return new KeyValueClause(ColumnName, value, CompareOpration.GreatOrEqual, _function);
        }

        public static KeyValueClause operator <=(CK key, object value)
		{
            return new KeyValueClause(key.ColumnName, value, CompareOpration.LessOrEqual, key._function);
		}

        public KeyValueClause Le(object value)
        {
            return new KeyValueClause(ColumnName, value, CompareOpration.LessOrEqual, _function);
        }

        public static KeyValueClause operator ==(CK key, object value)
		{
            return new KeyValueClause(key.ColumnName, value, CompareOpration.Equal, key._function);
		}

        public KeyValueClause Eq(object value)
        {
            return this == value;
        }

        public static KeyValueClause operator !=(CK key, object value)
		{
            return new KeyValueClause(key.ColumnName, value, CompareOpration.NotEqual, key._function);
		}

        public KeyValueClause Ne(object value)
        {
            return this != value;
        }

        public KeyValueClause Like(string value)
        {
            return new KeyValueClause(ColumnName, (value), CompareOpration.Like, _function);
        }

        public KeyValueClause MiddleLike(string value)
        {
            return new KeyValueClause(ColumnName, ("%" + value + "%"), CompareOpration.Like, _function);
        }

        public KeyValueClause LeftLike(string value)
        {
            return new KeyValueClause(ColumnName, (value + "%"), CompareOpration.Like, _function);
        }

        public KeyValueClause RightLike(string value)
        {
            return new KeyValueClause(ColumnName, ("%" + value), CompareOpration.Like, _function);
        }

        #endregion

        #region KeyKey

        public static KeyKeyClause operator >(CK key, CK key2)
        {
            return key.Gt(key2);
        }

        public static KeyKeyClause operator <(CK key, CK key2)
        {
            return key.Lt(key2);
        }

        public KeyKeyClause Gt(CK key2)
        {
            return new KeyKeyClause(ColumnName, key2.ColumnName, CompareOpration.GreatThan);
        }

        public KeyKeyClause Lt(CK key2)
        {
            return new KeyKeyClause(ColumnName, key2.ColumnName, CompareOpration.LessThan);
        }

        public static KeyKeyClause operator >=(CK key, CK key2)
        {
            return key.Ge(key2);
        }

        public static KeyKeyClause operator <=(CK key, CK key2)
        {
            return key.Le(key2);
        }

        public KeyKeyClause Ge(CK key2)
        {
            return new KeyKeyClause(ColumnName, key2.ColumnName, CompareOpration.GreatOrEqual);
        }

        public KeyKeyClause Le(CK key2)
        {
            return new KeyKeyClause(ColumnName, key2.ColumnName, CompareOpration.LessOrEqual);
        }

        public static KeyKeyClause operator ==(CK key, CK key2)
        {
            return key.Eq(key2);
        }

        public static KeyKeyClause operator !=(CK key, CK key2)
        {
            return key.Ne(key2);
        }

        public KeyKeyClause Eq(CK key2)
        {
            return new KeyKeyClause(this, key2, CompareOpration.Equal);
        }

        public KeyKeyClause Ne(CK key2)
        {
            return new KeyKeyClause(this, key2, CompareOpration.NotEqual);
        }

        public KeyKeyClause Like(CK key2)
        {
            return new KeyKeyClause(ColumnName, key2.ColumnName, CompareOpration.Like);
        }

        #endregion

        #region Misc

        public override int GetHashCode()
		{
			throw new ApplicationException("Do not use this function !");
		}

		public override bool Equals(object obj)
		{
			throw new ApplicationException("Do not use this function !");
		}

        #endregion

	    public CK ToLower()
	    {
	        _function = ColumnFunction.ToLower;
	        return this;
	    }

        public CK ToUpper()
        {
            _function = ColumnFunction.ToUpper;
            return this;
        }
    }
}
