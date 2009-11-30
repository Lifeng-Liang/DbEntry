using System;
using System.Text;
using System.Collections;
using System.Data;
using Lephone.Data.Common;
using Lephone.Data.SqlEntry;
using Lephone.Data.Driver;
using Lephone.Data.Builder;
using Lephone.Util;
using Lephone.Util.Text;

namespace Lephone.Data.Dialect
{
	public class DbDialect
	{
		protected static readonly NotImplementedException ShouldBeImplemented = new NotImplementedException("should be implemented by subclass if needed.");
		protected static readonly NotSupportedException DoesNotSupportNativeKey = new NotSupportedException( "Dialect does not support native key generation" );
        protected static readonly NotSupportedException DoesNotSupportPagedSelect = new NotSupportedException("Database not support paged select statement.");
        protected static readonly DataException PagedMustHaveOrder = new DataException("Paged select must have Order And Values not Empty.");

        protected readonly Hashtable TypeNames = Hashtable.Synchronized(new Hashtable());

		public DbDialect()
        {
            TypeNames[DataType.String]  = "TEXT";
            TypeNames[DataType.DateTime]    = "DATETIME";
            TypeNames[DataType.Date]    = "DATE";
            TypeNames[DataType.Time]    = "DATETIME";
            TypeNames[DataType.Boolean] = "BOOL";

            TypeNames[DataType.Byte]    = "TINYINT";
            TypeNames[DataType.SByte]   = "";
            TypeNames[DataType.Decimal] = "DECIMAL";
            TypeNames[DataType.Double]  = "FLOAT";
            TypeNames[DataType.Single]  = "REAL";

            TypeNames[DataType.Int32]   = "INT";
            TypeNames[DataType.UInt32]  = "INT";
            TypeNames[DataType.Int64]   = "BIGINT";
            TypeNames[DataType.UInt64]  = "BIGINT";
            TypeNames[DataType.Int16]   = "SMALLINT";
            TypeNames[DataType.UInt16]  = "SMALLINT";

            TypeNames[DataType.Guid]    = "UNIQUEIDENTIFIER";
            TypeNames[DataType.Binary]  = "BINARY";

            TypeNames[typeof(string)]   = "VARCHAR";
            TypeNames[typeof(byte[])]   = "BINARY";
        }

        public virtual string DbNowString
        {
            get { return "NOW()"; }
        }

        public virtual string GetUserId(string ConnectionString)
        {
            return null;
        }

        public virtual IDataReader GetDataReader(IDataReader dr, Type ReturnType)
        {
            return dr;
        }

        public virtual bool NotSupportPostFix
        {
            get { return false; }
        }

        public virtual bool NeedBracketForJoin
        {
            get { return true; }
        }

        public virtual bool NeedCommitCreateFirst
        {
            get { return false; }
        }

        public virtual bool SupportDirctionOfEachColumnInIndex
        {
            get { return true; }
        }

        public virtual object ExecuteInsert(DataProvider dp, InsertStatementBuilder sb, ObjectInfo oi)
        {
            if (oi.HasOnePremarykey && oi.KeyFields[0].FieldType == typeof(Guid))
            {
                Guid key = Guid.NewGuid();
                sb.Values[0].Value = key;
                SqlStatement sql = sb.ToSqlStatement(dp.Dialect);
                oi.LogSql(sql);
                dp.ExecuteNonQuery(sql);
                return key;
            }
            return ExecuteInsertIntKey(dp, sb, oi);
        }

	    protected virtual object ExecuteInsertIntKey(DataProvider dp, InsertStatementBuilder sb, ObjectInfo oi)
        {
            SqlStatement sql = sb.ToSqlStatement(dp.Dialect);
            sql.SqlCommandText = AddIdentitySelectToInsert(sql.SqlCommandText);
            oi.LogSql(sql);
            return dp.ExecuteScalar(sql);
        }

        public virtual void ExecuteDropSequence(DataProvider dp, string TableName)
        {
        }

        public virtual DbStructInterface GetDbStructInterface()
        {
            return new DbStructInterface(null, null, null, null, null);
        }

        public virtual string GetTypeName(DataType dt, bool IsUnicode, int Length)
        {
            object key = dt;
            if(Length > 0)
            {
                if (dt == DataType.String)
                {
                    key = typeof(string);
                }
                else if(dt == DataType.Binary)
                {
                    key = typeof (byte[]);
                }
            }
            var s =(string)TypeNames[key];
            if (Length > 0)
            {
                if(dt == DataType.Binary)
                {
                    s += GetLengthStringForBlob(Length);
                }
                else
                {
                    s += " (" + Length + ")";
                }
            }
            if (IsUnicode)
            {
                s = GetUnicodeTypeString(s);
            }
            return s;
        }

        protected virtual string GetLengthStringForBlob(int Length)
        {
            return " (" + Length + ")";
        }

        public virtual string GetUnicodeTypeString(string AsciiTypeString)
        {
            return "N" + AsciiTypeString;
        }

        public virtual DbDriver CreateDbDriver(string ConnectionString, string DbProviderFactoryName, bool AutoCreateTable)
        {
            return new CommonDbDriver(this, ConnectionString, DbProviderFactoryName, AutoCreateTable);
        }

        public virtual string GetConnectionString(string ConnectionString)
        {
            return ProcessConnectionnString(ConnectionString);
        }

        protected static string ProcessConnectionnString(string ConnectionString)
        {
            string s = ConnectionString.Trim();
            s = s.Replace("{BaseDirectory}", SystemHelper.BaseDirectory);
            s = s.Replace("{TempDirectory}", SystemHelper.TempDirectory);
            if (s.StartsWith("@"))
            {
                s = s.Replace("~", SystemHelper.BaseDirectory);
            }
            return s;
        }

        public virtual bool SupportsRangeStartIndex
        {
            get { return true; }
        }

        public virtual SqlStatement GetSelectSqlStatement(SelectStatementBuilder ssb)
        {
            SqlStatement sql = (ssb.Range == null) ?
                GetNormalSelectSqlStatement(ssb) :
                GetPagedSelectSqlStatement(ssb);
            sql.SqlCommandText += ";\n";
            return sql;
        }

        protected virtual SqlStatement GetNormalSelectSqlStatement(SelectStatementBuilder ssb)
        {
            var dpc = new DataParameterCollection();
            string sqlString = string.Format("SELECT {0} FROM {1}{2}{3}{4}",
                ssb.GetColumns(this),
                ssb.From.ToSqlText(dpc, this),
                ssb.Where.ToSqlText(dpc, this),
                ssb.IsGroupBy ? " GROUP BY " + GetFunctionArgs(ssb) : "",
                (ssb.Order == null || ssb.Keys.Count == 0) ? "" : ssb.Order.ToSqlText(dpc, this)
                );
            return new TimeConsumingSqlStatement(CommandType.Text, sqlString, dpc);
        }

        private string GetFunctionArgs(SelectStatementBuilder ssb)
        {
            var ret = new StringBuilder();
            foreach (string s in ssb.GroupbyArgs)
            {
                ret.Append(QuoteForColumnName(s));
                ret.Append(",");
            }
            if(ret.Length > 1)
            {
                ret.Length--;
            }
            return ret.ToString();
        }

        protected virtual SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb)
        {
            throw DoesNotSupportPagedSelect;
        }

        public virtual bool ExecuteEachLine
		{
			get { return false; }
		}

		public virtual string AddIdentitySelectToInsert( string sql )
		{
            return sql + IdentitySelectString;
        }

		public virtual string IdentitySelectString
		{
			get { throw DoesNotSupportNativeKey; }
		}

		public virtual string IdentityColumnString
		{
			get { throw DoesNotSupportNativeKey; }
		}

        public virtual bool IdentityIncludePKString
        {
            get { return false; }
        }

        public virtual string IdentityTypeString
        {
            get { return null; }
        }

        public virtual string PrimaryKeyString
        {
            get { return "PRIMARY KEY"; }
        }

        public virtual string GetCreateSequenceString(string TableName)
        {
            return "";
        }

        public virtual string NullString
        {
            get { return " NULL "; }
        }

        public virtual string NotNullString
        {
            get { return " NOT NULL "; }
        }

		public virtual char OpenQuote
		{
			get { return '"'; }
		}

		public virtual char CloseQuote
		{
			get { return '"'; }
		}

		public virtual char ParameterPrefix
		{
			get { return '@'; }
		}

		protected virtual string Quote( string name )
		{
		    return Quote(name, '.');
		}

        protected virtual string Quote(string name, char splitter)
        {
            string[] ss = name.Split(splitter);
            var ret = new StringBuilder();
            foreach (string s in ss)
            {
                ret.Append(QuoteSingle(s));
                ret.Append(".");
            }
            if (ret.Length > 0)
            {
                ret.Length--;
            }
            return ret.ToString();
        }

        protected virtual string QuoteSingle(string name)
        {
            string quotedName = name.Replace(OpenQuote.ToString(), new string(OpenQuote, 2));
            if (OpenQuote != CloseQuote)
            {
                quotedName = name.Replace(CloseQuote.ToString(), new string(CloseQuote, 2));
            }
            return OpenQuote + quotedName + CloseQuote;
        }

		public virtual string QuoteForColumnName( string columnName )
		{
			return Quote( columnName );
		}

		public virtual string QuoteForTableName( string tableName )
		{
            string[] ss = tableName.Split('.');
            return Quote(ss[0], ':');
		}

        public virtual string QuoteForLimitTableName(string tableName)
        {
            return Quote(tableName);
        }

        public virtual string QuoteDateTimeValue(string theDate)
        {
            return "'" + theDate + "'";
        }

        public virtual string GenIndexName(string n)
        {
            return null;
        }

        protected static string GenIndexName(string n, int maxLength)
        {
            if (string.IsNullOrEmpty(n))
            {
                var bytes = MiscProvider.Instance.NewGuid().ToByteArray();
                var s = Base32StringCoding.Decode(bytes);
                return s;
            }
            return GenIdentityName(n, maxLength);
        }

        protected static string GenIdentityName(string n, int maxLength)
        {
            if(string.IsNullOrEmpty(n))
            {
                throw new DataException("Identity name could not be null or empty");
            }
            if(n.Length > maxLength)
            {
                var bytes = StringHelper.HashMD5(n);
                var s = Base32StringCoding.Decode(bytes);
                return s;
            }
            return null;
        }
    }
}
