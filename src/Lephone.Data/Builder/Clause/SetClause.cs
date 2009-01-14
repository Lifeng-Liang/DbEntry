using System;
using System.Text;
using Lephone.Util.Text;
using Lephone.Data.Common;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder.Clause
{
	[Serializable]
	public class SetClause : KeyValueCollection, IClause
	{
		public SetClause() {}

		public string ToSqlText(DataParamterCollection dpc, DbDialect dd)
		{
			var sb = new StringBuilder("Set ");
			foreach ( KeyValue kv in this )
			{
				string dpStr;
                if (kv.ValueType == typeof(AutoValue))
                {
                    switch ((AutoValue)kv.Value)
                    {
                        case AutoValue.DbNow:
                            dpStr = dd.DbNowString;
                            break;
                        case AutoValue.Count:
                            dpStr = dd.QuoteForColumnName(kv.Key) + "+1";
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    if (DataSetting.UsingParamter)
                    {
                        dpStr = string.Format(dd.ParamterPrefix + "{0}_{1}", DataParamter.LegalKey(kv.Key), dpc.Count);
                        var dp = new DataParamter(dpStr, kv.Value, kv.ValueType);
                        dpc.Add(dp);
                    }
                    else
                    {
                        dpStr = DataTypeParser.ParseToString(kv.Value, dd);
                    }
                }

				sb.Append( dd.QuoteForColumnName(kv.Key) );
				sb.Append( "=" );
				sb.Append( dpStr );
				sb.Append(",");
			}
			try
			{
				return StringHelper.GetStringLeft(sb.ToString());
			}
			catch ( ArgumentOutOfRangeException )
			{
				return "";
			}
		}
	}
}
