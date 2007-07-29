
#region usings

using System;
using System.Collections;
using System.Collections.Specialized;

using org.hanzify.llf.Data.Common;

#endregion

namespace org.hanzify.llf.Data.SqlEntry
{
	internal static class DataTypeParser
	{
		private static HybridDictionary Types;

		static DataTypeParser()
		{
			Types = new HybridDictionary();
            Types[typeof(string)]   = DataType.String;
            Types[typeof(DateTime)] = DataType.Date;
            Types[typeof(bool)]     = DataType.Boolean;

            Types[typeof(byte)]     = DataType.Byte;
            Types[typeof(sbyte)]    = DataType.SByte;
            Types[typeof(decimal)]  = DataType.Decimal;
            Types[typeof(double)]   = DataType.Double;
            Types[typeof(float)]    = DataType.Single;

            Types[typeof(int)]      = DataType.Int32;
            Types[typeof(uint)]     = DataType.UInt32;
            Types[typeof(long)]     = DataType.Int64;
            Types[typeof(ulong)]    = DataType.UInt64;
            Types[typeof(short)]    = DataType.Int16;
            Types[typeof(ushort)]   = DataType.UInt16;

            Types[typeof(Guid)]     = DataType.Guid;
            Types[typeof(byte[])]   = DataType.Binary;
            Types[typeof(Enum)]     = DataType.Int32;
        }

		public static DataType Parse(object o)
		{
			return Parse(o.GetType());
		}

		public static DataType Parse(Type t)
		{
			if ( t.IsEnum )
			{
				t = typeof(Enum);
			}
			if ( Types.Contains(t) )
			{
				return (DataType)Types[t];
			}
            if ( NullableHelper.IsNullableType(t) )
            {
                return (DataType)NullableHelper.GetDataType(t);
            }
			throw new ArgumentOutOfRangeException(t.ToString());
		}

		public static string ParseToString(object o, Dialect.DbDialect dd)
		{
            if (o == null)
            {
                return "NULL";
            }
			Type ot = o.GetType();
			if ( typeof(bool) == ot )
			{
				return Convert.ToInt32(o).ToString();
			}
			else if	( typeof(string) == ot )
			{
				string s = o.ToString();
				s = s.Replace("'", "''");
				return string.Format("N'{0}'", s);
			}
			else if ( typeof(DateTime) == ot )
			{
                return dd.QuoteDateTimeValue(o.ToString());
			}
            else if (ot.IsEnum)
            {
                return Convert.ToInt32(o).ToString();
            }
            else if (typeof(byte[]) == ot)
            {
                throw new ApplicationException("Sql without paramter can not support blob, please using paramter mode.");
            }
			{
				return o.ToString();
			}
		}
	}
}
