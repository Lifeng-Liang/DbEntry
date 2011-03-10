using System;
using System.Text;
using Lephone.Core.Text;
using Lephone.Data.Common;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder.Clause
{
	[Serializable]
	public class ValuesClause : KeyValueCollection, IClause
	{
		private const string StatementTemplate = "({0}) VALUES ({1})";

		public string ToSqlText(DataParameterCollection dpc, DbDialect dd)
		{
			var sb1 = new StringBuilder();
			var sb2 = new StringBuilder();
			foreach ( KeyValue kv in this )
			{
				string dpStr;
                if (kv.ValueType == typeof(AutoValue))
                {
                    dpStr = AutoValue.DbNow == (AutoValue)kv.Value ? dd.DbNowString : "0";
                }
                else
                {
                    if (DataSettings.UsingParameter)
                    {
                        dpStr = dd.QuoteParameter(string.Format("{0}_{1}", DataParameter.LegalKey(kv.Key), dpc.Count));
                        var dp = new DataParameter(dpStr, kv.NullableValue, kv.ValueType);
                        dpc.Add(dp);
                    }
                    else
                    {
                        dpStr = DataTypeParser.ParseToString(kv.Value, dd);
                    }
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
