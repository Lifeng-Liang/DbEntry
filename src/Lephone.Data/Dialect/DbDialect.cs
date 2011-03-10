using System;
using System.Text;
using System.Collections;
using System.Data;
using Lephone.Data.Common;
using Lephone.Data.Model;
using Lephone.Data.SqlEntry;
using Lephone.Data.Driver;
using Lephone.Data.Builder;
using Lephone.Core;
using Lephone.Core.Text;

namespace Lephone.Data.Dialect
{
	public class DbDialect
	{
		protected static readonly NotImplementedException ShouldBeImplemented = new NotImplementedException("should be implemented by subclass if needed.");
		protected static readonly NotSupportedException DoesNotSupportNativeKey = new NotSupportedException( "Dialect does not support native key generation" );
        protected static readonly NotSupportedException DoesNotSupportPagedSelect = new NotSupportedException("Database not support paged select statement.");
        protected static readonly DataException PagedMustHaveOrder = new DataException("Paged selection must have Order And Values not Empty.");

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

        public virtual void InitConnection(DataProvider provider, IDbConnection conn)
        {
        }

        public virtual string DbNowString
        {
            get { return "NOW()"; }
        }

        public virtual string GetUserId(string connectionString)
        {
            return null;
        }

        public virtual IDataReader GetDataReader(IDataReader dr, Type returnType)
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

        public virtual object ExecuteInsert(InsertStatementBuilder sb, ObjectInfo info, DataProvider provider)
        {
            if (info.HasOnePrimaryKey && info.KeyMembers[0].MemberType == typeof(Guid))
            {
                if(info.KeyMembers[0].Is.DbGenerateGuid)
                {
                    Guid key = Guid.NewGuid();
                    sb.Values[0].Value = key;
                }
                SqlStatement sql = sb.ToSqlStatement(provider.Dialect, info.AllowSqlLog);
                provider.ExecuteNonQuery(sql);
                return sb.Values[0].Value;
            }
            return ExecuteInsertIntKey(sb, info, provider);
        }

        protected virtual object ExecuteInsertIntKey(InsertStatementBuilder sb, ObjectInfo info, DataProvider provider)
        {
            SqlStatement sql = sb.ToSqlStatement(provider.Dialect, info.AllowSqlLog);
            sql.SqlCommandText = AddIdentitySelectToInsert(sql.SqlCommandText);
            return provider.ExecuteScalar(sql);
        }

        public virtual void ExecuteDropSequence(DataProvider dp, string tableName)
        {
        }

        public virtual DbStructInterface GetDbStructInterface()
        {
            return new DbStructInterface(null, null, null, null, null);
        }

        public virtual string GetTypeName(DataType dt, bool isUnicode, int length, int decimalPart)
        {
            object key = dt;
            if(length > 0)
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
            if (length > 0 || decimalPart > 0)
            {
                if(dt == DataType.Binary)
                {
                    s += GetLengthStringForBlob(length);
                }
                else
                {
                    if(decimalPart > 0)
                    {
                        s += " (" + length + "," + decimalPart + ")";
                    }
                    else
                    {
                        s += " (" + length + ")";
                    }
                }
            }
            if (isUnicode)
            {
                s = GetUnicodeTypeString(s);
            }
            return s;
        }

        protected virtual string GetLengthStringForBlob(int length)
        {
            return " (" + length + ")";
        }

        public virtual string GetUnicodeTypeString(string asciiTypeString)
        {
            return "N" + asciiTypeString;
        }

        public virtual DbDriver CreateDbDriver(string name, string connectionString, string dbProviderFactoryName, bool autoCreateTable)
        {
            return new CommonDbDriver(this, name, connectionString, dbProviderFactoryName, autoCreateTable);
        }

        public virtual string GetConnectionString(string connectionString)
        {
            return ProcessConnectionnString(connectionString);
        }

        protected static string ProcessConnectionnString(string connectionString)
        {
            string s = connectionString.Trim();
            s = s.Replace("{BaseDirectory}", SystemHelper.BaseDirectory);
            s = s.Replace("{TempDirectory}", SystemHelper.TempDirectory);
            s = s.Replace("{CurrentDirectory}", SystemHelper.CurrentDirectory);
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

        public virtual SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb)
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

        public virtual string GetCreateSequenceString(string tableName)
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

		public virtual string QuoteParameter(string parameterName)
		{
		    return '@' + parameterName;
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
                var bytes = StringHelper.HashMd5(n);
                var s = Base32StringCoding.Decode(bytes);
                return s;
            }
            return null;
        }

        public virtual SqlStatement GetAddDescriptionSql(ObjectInfo info)
        {
            return null;
        }
    }
}
