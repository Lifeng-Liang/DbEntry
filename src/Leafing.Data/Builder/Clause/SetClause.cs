using System;
using System.Collections.Generic;
using System.Text;
using Leafing.Core.Text;
using Leafing.Data.Common;
using Leafing.Data.Dialect;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Builder.Clause
{
    public enum KvOpertation
    {
        None,
        Now,
        Add,
        Sub,
    }

    public class KeyOpValue : KeyValue
    {
        public KvOpertation Operation;

        public KeyOpValue(string key, object value, Type type)
            : base(key, value, type)
        {
            this.Operation = KvOpertation.None;
        }

        public KeyOpValue(string key, object value, KvOpertation op)
            : base(key, value)
        {
            this.Operation = op;
        }
    }

	[Serializable]
	public class SetClause : List<KeyOpValue>, IClause
	{
		public string ToSqlText(DataParameterCollection dpc, DbDialect dd)
		{
			var sb = new StringBuilder("SET ");
			foreach ( var kv in this )
			{
				string dpStr;
			    switch(kv.Operation)
			    {
			        case KvOpertation.None:
			            if(DataSettings.UsingParameter)
			            {
			                dpStr = dd.QuoteParameter(string.Format("{0}_{1}", DataParameter.LegalKey(kv.Key), dpc.Count));
			                var dp = new DataParameter(dpStr, kv.Value, kv.ValueType);
			                dpc.Add(dp);
			            }
			            else
			            {
			                dpStr = DataTypeParser.ParseToString(kv.Value, dd);
			            }
			            break;
			        case KvOpertation.Now:
			            dpStr = dd.DbNowString;
			            break;
			        case KvOpertation.Add:
			            dpStr = dd.QuoteForColumnName(kv.Key) + "+(" + kv.Value + ")";
			            break;
			        case KvOpertation.Sub:
			            dpStr = dd.QuoteForColumnName(kv.Key) + "-(" + kv.Value + ")";
			            break;
                    default:
			            throw new ApplicationException("Application Error");
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
