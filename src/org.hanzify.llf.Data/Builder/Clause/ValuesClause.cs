
#region usings

using System;
using System.Text;
using Lephone.Util.Text;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

#endregion

namespace Lephone.Data.Builder.Clause
{
	[Serializable]
	public class ValuesClause : KeyValueCollection, IClause
	{
		private const string StatementTemplate = "({0}) Values ({1})";

		public ValuesClause() {}

		public string ToSqlText(ref DataParamterCollection dpc, DbDialect dd)
		{
			StringBuilder sb1 = new StringBuilder();
			StringBuilder sb2 = new StringBuilder();
			foreach ( KeyValue kv in this )
			{
				string dpStr;
                if (DataSetting.UsingParamter)
                {
                    dpStr = string.Format(dd.ParamterPrefix + "{0}_{1}", kv.Key, dpc.Count);
                    DataParamter dp = new DataParamter(dpStr, kv.NullableValue, kv.ValueType);
                    dpc.Add(dp);
                }
                else
                {
                    dpStr = DataTypeParser.ParseToString(kv.Value, dd);
                }

				sb1.Append( dd.QuoteForColumnName(kv.Key) );
				sb1.Append(",");

				sb2.Append( dpStr );
				sb2.Append(",");
			}
			return string.Format( StatementTemplate, StringHelper.GetStringLeft(sb1.ToString()), StringHelper.GetStringLeft(sb2.ToString()) );
		}
	}
}
