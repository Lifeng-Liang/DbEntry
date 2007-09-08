
using System;
using System.Text;
using System.Collections;
using System.Data;
using Lephone.Data.Common;
using Lephone.Data.SqlEntry;
using Lephone.Data.Driver;
using Lephone.Data.Builder;
using Lephone.Util;
using Lephone.Util.Logging;

namespace Lephone.Data.Dialect
{
	public class DbDialect
	{
		protected static readonly NotImplementedException ShouldBeImplemented = new NotImplementedException("should be implemented by subclass if needed.");
		protected static readonly NotSupportedException DoesNotSupportNativeKey = new NotSupportedException( "Dialect does not support native key generation" );
        protected static readonly NotSupportedException DoesNotSupportPagedSelect = new NotSupportedException("Database not support paged select statement.");
        protected static readonly DbEntryException PagedMustHaveOrder = new DbEntryException("Paged select must have Order And Values not Empty.");

        protected readonly Hashtable TypeNames = Hashtable.Synchronized(new Hashtable());

		public DbDialect()
        {
            TypeNames[DataType.String]  = "text";
            TypeNames[DataType.Date]    = "datetime";
            TypeNames[DataType.Boolean] = "bool";

            TypeNames[DataType.Byte]    = "tinyint";
            TypeNames[DataType.SByte]   = "";
            TypeNames[DataType.Decimal] = "decimal";
            TypeNames[DataType.Double]  = "float";
            TypeNames[DataType.Single]  = "real";

            TypeNames[DataType.Int32]   = "int";
            TypeNames[DataType.UInt32]  = "";
            TypeNames[DataType.Int64]   = "bigint";
            TypeNames[DataType.UInt64]  = "";
            TypeNames[DataType.Int16]   = "smallint";
            TypeNames[DataType.UInt16]  = "";

            TypeNames[DataType.Guid]    = "uniqueidentifier";
            TypeNames[DataType.Binary]  = "binary";

            TypeNames[typeof(string)]   = "varchar";
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
            object key = (dt == DataType.String && Length > 0) ?
                (object)typeof(string) :
                (object)dt;
            string s =(string)TypeNames[key];
            if (IsUnicode)
            {
                s = UnicodeTypePrefix + s;
            }
            if (Length > 0)
            {
                return s + " (" + Length.ToString() + ")";
            }
            return s;
        }

        public virtual string UnicodeTypePrefix
        {
            get { return "n"; }
        }

        public virtual string NewGuidString()
        {
            throw new NotImplementedException();
        }

        public virtual DbDriver CreateDbDriver(string ConnectionString, string DbProviderFactoryName)
        {
            return new CommonDbDriver(this, ConnectionString, DbProviderFactoryName);
        }

        public virtual string GetConnectionString(string ConnectionString)
        {
            return ProcessConnectionnString(ConnectionString);
        }

        protected string ProcessConnectionnString(string ConnectionString)
        {
            string s = ConnectionString.Trim();
            if (s.StartsWith("@"))
            {
                s = s.Replace("{BaseDirectory}", SystemHelper.BaseDirectory)
                    .Replace("~", SystemHelper.BaseDirectory);
            }
            return s;
        }

        public virtual bool SupportsRange
        {
            get { return false; }
        }

        public virtual bool SupportsRangeStartIndex
        {
            get { return SupportsRange; }
        }

        public virtual SqlStatement GetSelectSqlStatement(SelectStatementBuilder ssb)
        {
            SqlStatement Sql = (ssb.Range == null) ?
                GetNormalSelectSqlStatement(ssb) :
                GetPagedSelectSqlStatement(ssb);
            Sql.SqlCommandText += ";\n";
            return Sql;
        }

        protected virtual SqlStatement GetNormalSelectSqlStatement(SelectStatementBuilder ssb)
        {
            DataParamterCollection dpc = new DataParamterCollection();
            string SqlString = string.Format("Select {0} From {1}{2}{3}{4}",
                ssb.GetColumns(this),
                ssb.From.ToSqlText(ref dpc, this),
                ssb.Where.ToSqlText(ref dpc, this),
                ssb.IsGroupBy ? " Group By " + this.QuoteForColumnName(ssb.CountCol) : "",
                (ssb.Order == null || ssb.Keys.Count == 0) ? "" : ssb.Order.ToSqlText(ref dpc, this)
                );
            return new TimeConsumingSqlStatement(CommandType.Text, SqlString, dpc);
        }

        protected virtual SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb)
        {
            throw DoesNotSupportPagedSelect;
        }

        public virtual bool ExecuteEachLine
		{
			get { return false; }
		}

		public virtual bool SupportsIdentitySelectInInsert
		{
			get { return false; }
		}

		public virtual bool SupportsIdentityColumns
		{
			get { return false; }
		}

		public virtual string NullColumnString
		{
			get { return String.Empty; }
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

        public virtual string IdentityTypeString
        {
            get { return null; }
        }

        public virtual string PrimaryKeyString
        {
            get { throw DoesNotSupportNativeKey; }
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

		public virtual string IdentityInsertString
		{
			get { return ""; }
		}

		public virtual char OpenQuote
		{
			get { return '"'; }
		}

		public virtual char CloseQuote
		{
			get { return '"'; }
		}

		public virtual char ParamterPrefix
		{
			get { return '@'; }
		}

		protected virtual string CascadeConstraintsString
		{
			get { return String.Empty; }
		}

		public virtual string GetDropTableString( string tableName )
		{
			StringBuilder buf = new StringBuilder( "drop table " );
			if( SupportsIfExistsBeforeTableName )
			{
				buf.Append( "if exists " );
			}
			
			buf.Append( tableName ).Append( CascadeConstraintsString );
			
			if( SupportsIfExistsAfterTableName )
			{
				buf.Append( " if exists" );
			}
			return buf.ToString();
		}

		protected virtual bool SupportsIfExistsBeforeTableName
		{
			get { return false; }
		}

		protected virtual bool SupportsIfExistsAfterTableName
		{
			get { return false; }
		}

		public virtual int MaxAnsiStringSize
		{
			get { throw ShouldBeImplemented; }
		}

		public virtual int MaxBinarySize
		{
			get { throw ShouldBeImplemented; }
		}

		public virtual int MaxBinaryBlobSize
		{
			get { throw ShouldBeImplemented; }
		}

		public virtual int MaxStringClobSize
		{
			get { throw ShouldBeImplemented; }
		}

		public virtual int MaxStringSize
		{
			get { throw ShouldBeImplemented; }
		}

		protected virtual string Quote( string name )
		{
            string[] ss = name.Split('.');
            StringBuilder ret = new StringBuilder();
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

		public virtual string QuoteForAliasName( string aliasName )
		{
			return Quote( aliasName );
		}

		public virtual string QuoteForColumnName( string columnName )
		{
			return Quote( columnName );
		}

		public virtual string QuoteForTableName( string tableName )
		{
            string[] ss = tableName.Split('.');
            return QuoteSingle(ss[0]);
		}

        public virtual string QuoteForLimitTableName(string tableName)
        {
            return Quote(tableName);
        }

        public virtual string QuoteForSchemaName(string schemaName)
		{
			return Quote( schemaName );
		}

        public virtual string QuoteDateTimeValue(string theDate)
        {
            return "'" + theDate + "'";
        }
    }
}
