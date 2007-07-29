
#region usings

using System;
using org.hanzify.llf.Data.Builder.Clause;
using org.hanzify.llf.Data.Definition;

#endregion

namespace org.hanzify.llf.Data
{
	[Serializable]
	public class CK
	{
		private string _ColumnName;
		private static CK _Col = new CK("");

		public CK(string ColumnName)
		{
			_ColumnName = ColumnName;
		}

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

        #region KeyValue

        public static KeyValueClause operator > (CK Key, object Value)
		{
			return new KeyValueClause(Key._ColumnName, Value, CompareOpration.GreatThan);
		}

        public KeyValueClause Gt(object Value)
        {
            return new KeyValueClause(this._ColumnName, Value, CompareOpration.GreatThan);
        }

		public static KeyValueClause operator < (CK Key, object Value)
		{
			return new KeyValueClause(Key._ColumnName, Value, CompareOpration.LessThan);
		}

        public KeyValueClause Lt(object Value)
        {
            return new KeyValueClause(this._ColumnName, Value, CompareOpration.LessThan);
        }

        public static KeyValueClause operator >=(CK Key, object Value)
		{
			return new KeyValueClause(Key._ColumnName, Value, CompareOpration.GreatOrEqual);
		}

        public KeyValueClause Ge(object Value)
        {
            return new KeyValueClause(this._ColumnName, Value, CompareOpration.GreatOrEqual);
        }

        public static KeyValueClause operator <=(CK Key, object Value)
		{
			return new KeyValueClause(Key._ColumnName, Value, CompareOpration.LessOrEqual);
		}

        public KeyValueClause Le(object Value)
        {
            return new KeyValueClause(this._ColumnName, Value, CompareOpration.LessOrEqual);
        }

        public static KeyValueClause operator ==(CK Key, object Value)
		{
			if ( Value == null )
			{
				return new KeyValueClause(Key._ColumnName, DBNull.Value, CompareOpration.Is);
			}
			else
			{
				return new KeyValueClause(Key._ColumnName, Value, CompareOpration.Equal);
			}
		}

        public KeyValueClause Eq(object Value)
        {
            return this == Value;
        }

        public static KeyValueClause operator !=(CK Key, object Value)
		{
            if (Value == null)
            {
                return new KeyValueClause(Key._ColumnName, DBNull.Value, CompareOpration.IsNot);
            }
            else
            {
                return new KeyValueClause(Key._ColumnName, Value, CompareOpration.NotEqual);
            }
		}

        public KeyValueClause Ne(object Value)
        {
            return this != Value;
        }

        public KeyValueClause Like(string Value)
        {
            return new KeyValueClause(this._ColumnName, (Value), CompareOpration.Like);
        }

        public KeyValueClause MiddleLike(string Value)
        {
            return new KeyValueClause(this._ColumnName, ("%" + Value + "%"), CompareOpration.Like);
        }

        public KeyValueClause LeftLike(string Value)
        {
            return new KeyValueClause(this._ColumnName, (Value + "%"), CompareOpration.Like);
        }

        public KeyValueClause RightLike(string Value)
        {
            return new KeyValueClause(this._ColumnName, ("%" + Value), CompareOpration.Like);
        }

        #endregion

        #region KeyKey

        public KeyKeyClause Gt(CK Key2)
        {
            return new KeyKeyClause(this._ColumnName, Key2._ColumnName, CompareOpration.GreatThan);
        }

        public KeyKeyClause Lt(CK Key2)
        {
            return new KeyKeyClause(this._ColumnName, Key2._ColumnName, CompareOpration.LessThan);
        }

        public KeyKeyClause Ge(CK Key2)
        {
            return new KeyKeyClause(this._ColumnName, Key2._ColumnName, CompareOpration.GreatOrEqual);
        }

        public KeyKeyClause Le(CK Key2)
        {
            return new KeyKeyClause(this._ColumnName, Key2._ColumnName, CompareOpration.LessOrEqual);
        }

        public KeyKeyClause Eq(CK Key2)
        {
            return new KeyKeyClause(this._ColumnName, Key2._ColumnName, CompareOpration.Equal);
        }

        public KeyKeyClause Ne(CK Key2)
        {
            return new KeyKeyClause(this._ColumnName, Key2._ColumnName, CompareOpration.NotEqual);
        }

        public KeyKeyClause Like(CK Key2)
        {
            return new KeyKeyClause(this._ColumnName, Key2._ColumnName, CompareOpration.Like);
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
    }
}
