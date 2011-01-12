namespace Lephone.Data
{
	public class DESC : ASC
	{
		public DESC(string key) : base(key) {}

		public override string ToString(Dialect.DbDialect dd)
		{
			return dd.QuoteForColumnName( Key ) + " DESC";
		}

		public static explicit operator DESC (string key)
		{
			return new DESC(key);
		}
	}
}
