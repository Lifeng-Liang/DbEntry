using System;
using System.Collections.Specialized;
using Lephone.Data.Common;

namespace Lephone.Data.SqlEntry
{
	internal static class DataTypeParser
	{
		private static readonly HybridDictionary Types;

		static DataTypeParser()
		{
			Types = new HybridDictionary();
            Types[typeof(string)]   = DataType.String;
            Types[typeof(DateTime)] = DataType.DateTime;
            Types[typeof(Date)]     = DataType.Date;
            Types[typeof(Time)]     = DataType.Time;
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

            Types[typeof(DBNull)]   = DataType.Single; // is that right?
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
                return NullableHelper.GetDataType(t);
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
		    if	( typeof(string) == ot )
		    {
		        string s = o.ToString();
		        s = s.Replace("'", "''");
		        return string.Format("N'{0}'", s);
		    }
		    if ( typeof(DateTime) == ot || typeof(Date) == ot || typeof(Time) == ot )
		    {
		        return dd.QuoteDateTimeValue(o.ToString());
		    }
		    if (ot.IsEnum)
		    {
		        return Convert.ToInt32(o).ToString();
		    }
		    if (typeof(byte[]) == ot)
		    {
		        throw new ApplicationException("Sql without paramter can not support blob, please using paramter mode.");
		    }
		    {
				return o.ToString();
			}
		}
	}
}
