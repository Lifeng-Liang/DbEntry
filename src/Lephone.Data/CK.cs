using System;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Definition;
using Lephone.Data.Common;

namespace Lephone.Data
{
    [Serializable]
    public class FieldNameGetter<T>
    {
        private static readonly ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));

        public CK this[string FieldName]
        {
            get
            {
                foreach (MemberHandler m in oi.Fields)
                {
                    if (m.MemberInfo.Name == FieldName)
                    {
                        return new CK(m.Name);
                    }
                }
                throw new DataException("Can't find the field: " + FieldName);
            }
        }
    }

    [Serializable]
    public class CK<T>
    {
        private static readonly FieldNameGetter<T> _Field = new FieldNameGetter<T>();

        public static FieldNameGetter<T> Field
        {
            get { return _Field; }
        }
    }

	[Serializable]
	public class CK
	{
		private static readonly CK _Col = new CK("");

		public CK this[string ColumnName]
		{
			get { return new CK(ColumnName); }
		}

		public static CK K
		{
			get { return _Col; }
        }

        public static CK Column
        {
            get { return _Col; }
        }

	    private ColumnFunction function;

        public readonly string ColumnName;

        public CK(string ColumnName)
        {
            this.ColumnName = ColumnName;
        }

        #region KeyValue

        public static KeyValueClause operator > (CK Key, object Value)
		{
			return new KeyValueClause(Key.ColumnName, Value, CompareOpration.GreatThan, Key.function);
		}

        public KeyValueClause Gt(object Value)
        {
            return new KeyValueClause(ColumnName, Value, CompareOpration.GreatThan, function);
        }

		public static KeyValueClause operator < (CK Key, object Value)
		{
            return new KeyValueClause(Key.ColumnName, Value, CompareOpration.LessThan, Key.function);
		}

        public KeyValueClause Lt(object Value)
        {
            return new KeyValueClause(ColumnName, Value, CompareOpration.LessThan, function);
        }

        public static KeyValueClause operator >=(CK Key, object Value)
		{
            return new KeyValueClause(Key.ColumnName, Value, CompareOpration.GreatOrEqual, Key.function);
		}

        public KeyValueClause Ge(object Value)
        {
            return new KeyValueClause(ColumnName, Value, CompareOpration.GreatOrEqual, function);
        }

        public static KeyValueClause operator <=(CK Key, object Value)
		{
            return new KeyValueClause(Key.ColumnName, Value, CompareOpration.LessOrEqual, Key.function);
		}

        public KeyValueClause Le(object Value)
        {
            return new KeyValueClause(ColumnName, Value, CompareOpration.LessOrEqual, function);
        }

        public static KeyValueClause operator ==(CK Key, object Value)
		{
            return new KeyValueClause(Key.ColumnName, Value, CompareOpration.Equal, Key.function);
		}

        public KeyValueClause Eq(object Value)
        {
            return this == Value;
        }

        public static KeyValueClause operator !=(CK Key, object Value)
		{
            return new KeyValueClause(Key.ColumnName, Value, CompareOpration.NotEqual, Key.function);
		}

        public KeyValueClause Ne(object Value)
        {
            return this != Value;
        }

        public KeyValueClause Like(string Value)
        {
            return new KeyValueClause(ColumnName, (Value), CompareOpration.Like, function);
        }

        public KeyValueClause MiddleLike(string Value)
        {
            return new KeyValueClause(ColumnName, ("%" + Value + "%"), CompareOpration.Like, function);
        }

        public KeyValueClause LeftLike(string Value)
        {
            return new KeyValueClause(ColumnName, (Value + "%"), CompareOpration.Like, function);
        }

        public KeyValueClause RightLike(string Value)
        {
            return new KeyValueClause(ColumnName, ("%" + Value), CompareOpration.Like, function);
        }

        #endregion

        #region KeyKey

        public static KeyKeyClause operator >(CK Key, CK Key2)
        {
            return Key.Gt(Key2);
        }

        public static KeyKeyClause operator <(CK Key, CK Key2)
        {
            return Key.Lt(Key2);
        }

        public KeyKeyClause Gt(CK Key2)
        {
            return new KeyKeyClause(ColumnName, Key2.ColumnName, CompareOpration.GreatThan);
        }

        public KeyKeyClause Lt(CK Key2)
        {
            return new KeyKeyClause(ColumnName, Key2.ColumnName, CompareOpration.LessThan);
        }

        public static KeyKeyClause operator >=(CK Key, CK Key2)
        {
            return Key.Ge(Key2);
        }

        public static KeyKeyClause operator <=(CK Key, CK Key2)
        {
            return Key.Le(Key2);
        }

        public KeyKeyClause Ge(CK Key2)
        {
            return new KeyKeyClause(ColumnName, Key2.ColumnName, CompareOpration.GreatOrEqual);
        }

        public KeyKeyClause Le(CK Key2)
        {
            return new KeyKeyClause(ColumnName, Key2.ColumnName, CompareOpration.LessOrEqual);
        }

        public static KeyKeyClause operator ==(CK Key, CK Key2)
        {
            return Key.Eq(Key2);
        }

        public static KeyKeyClause operator !=(CK Key, CK Key2)
        {
            return Key.Ne(Key2);
        }

        public KeyKeyClause Eq(CK Key2)
        {
            return new KeyKeyClause(this, Key2, CompareOpration.Equal);
        }

        public KeyKeyClause Ne(CK Key2)
        {
            return new KeyKeyClause(this, Key2, CompareOpration.NotEqual);
        }

        public KeyKeyClause Like(CK Key2)
        {
            return new KeyKeyClause(ColumnName, Key2.ColumnName, CompareOpration.Like);
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
	        function = ColumnFunction.ToLower;
	        return this;
	    }

        public CK ToUpper()
        {
            function = ColumnFunction.ToUpper;
            return this;
        }
    }
}
