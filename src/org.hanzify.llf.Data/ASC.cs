
using System;

namespace org.hanzify.llf.Data
{
	public class ASC
	{
		internal string Key;

		public ASC(string Key)
		{
			this.Key = Key;
		}

		public virtual string ToString(Dialect.DbDialect dd)
		{
			return dd.QuoteForColumnName( Key ) + " ASC";
		}

		public static explicit operator ASC (string Key)
		{
			return new ASC(Key);
		}
	}
}
