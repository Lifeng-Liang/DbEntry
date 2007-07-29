
using System;

namespace org.hanzify.llf.Data
{
	public class DESC : ASC
	{
		public DESC(string Key) : base(Key) {}

		public override string ToString(Dialect.DbDialect dd)
		{
			return dd.QuoteForColumnName( Key ) + " DESC";
		}

		public static explicit operator DESC (string Key)
		{
			return new DESC(Key);
		}
	}
}
