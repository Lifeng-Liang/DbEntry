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
		public string ToSqlText(DataParameterCollection dpc, DbDialect dd)
		{
			var sb = new StringBuilder("SET ");
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
                    if (DataSettings.UsingParameter)
                    {
                        dpStr = string.Format(dd.ParameterPrefix + "{0}_{1}", DataParameter.LegalKey(kv.Key), dpc.Count);
                        var dp = new DataParameter(dpStr, kv.Value, kv.ValueType);
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
