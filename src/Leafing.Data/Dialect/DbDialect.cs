using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Leafing.Data.Common;
using Leafing.Data.Model;
using Leafing.Data.SqlEntry;
using Leafing.Data.Driver;
using Leafing.Data.Builder;
using Leafing.Core;
using Leafing.Core.Text;

namespace Leafing.Data.Dialect
{
	public class DbDialect
	{
		protected static readonly NotImplementedException ShouldBeImplemented = new NotImplementedException("should be implemented by subclass if needed.");
		protected static readonly NotSupportedException DoesNotSupportNativeKey = new NotSupportedException( "Dialect does not support native key generation" );
        protected static readonly NotSupportedException DoesNotSupportPagedSelect = new NotSupportedException("Database not support paged select statement.");
        protected static readonly DataException PagedMustHaveOrder = new DataException("Paged selection must have Order And Values not Empty.");

        protected readonly Dictionary<DataType, string> TypeNames = new Dictionary<DataType, string>();

		public DbDialect()
        {
            TypeNames[DataType.String]  = "VARCHAR";
            TypeNames[DataType.DateTime]    = "DATETIME";
            TypeNames[DataType.Date]    = "DATE";
            TypeNames[DataType.Time]    = "DATETIME";
            TypeNames[DataType.Boolean] = "BOOL";

            TypeNames[DataType.Byte]    = "TINYINT UNSIGNED";
            TypeNames[DataType.SByte]   = "TINYINT";
            TypeNames[DataType.Decimal] = "DECIMAL";
            TypeNames[DataType.Double]  = "FLOAT";
            TypeNames[DataType.Single]  = "REAL";

            TypeNames[DataType.Int32]   = "INT";
            TypeNames[DataType.UInt32]  = "INT UNSIGNED";
            TypeNames[DataType.Int64]   = "BIGINT";
            TypeNames[DataType.UInt64]  = "BIGINT UNSIGNED";
            TypeNames[DataType.Int16]   = "SMALLINT";
            TypeNames[DataType.UInt16]  = "SMALLINT UNSIGNED";

            TypeNames[DataType.Guid]    = "UNIQUEIDENTIFIER";
            TypeNames[DataType.Binary]  = "BINARY";
        }

        public virtual string DefaultDateTimeString()
        {
            return "1990-01-01 00:00:00";
        }

        public virtual void AddColumn(ModelContext ctx, string columnName, object o)
        {
            InnerGetValue(ctx, columnName, o, false);
        }

	    protected static void InnerGetValue(ModelContext ctx, string columnName, object o, bool defaultFirst)
	    {
            var builder = new AlterTableStatementBuilder(ctx.Info.From, defaultFirst);
	        var mem = ctx.Info.Members.FirstOrDefault(p => p.Name == columnName);
	        builder.AddColumn = new ColumnInfo(mem);
	        if(o != null)
	        {
	            builder.DefaultValue = o;
	        }
	        var sql = builder.ToSqlStatement(ctx);
	        ctx.Provider.ExecuteNonQuery(sql);
	    }

	    public virtual void DropColumns(ModelContext ctx, params string[] columns)
        {
            foreach(var column in columns)
            {
                var sb = new StringBuilder("ALTER TABLE ");
                sb.Append(QuoteForTableName(ctx.Info.From.MainTableName));
                sb.Append(" DROP COLUMN ");
                sb.Append(QuoteForColumnName(column));
                sb.Append(";");
                var sql = new SqlStatement(sb.ToString());
                ctx.Provider.ExecuteNonQuery(sql);
            }
        }

        protected virtual string GetStringNameWithLength(string baseType, bool isUnicode, int length)
        {
            if(length == 0)
            {
                return isUnicode ? "NTEXT" : "TEXT";
            }
            if(isUnicode)
            {
                return "N" + baseType + " (" + length + ")";
            }
            return baseType + " (" + length + ")";
        }

        protected virtual string GetBinaryNameWithLength(string baseType, int length)
        {
            if (length == 0)
            {
                return "BINARY";
            }
            return baseType + " (" + length + ")";
        }

        public virtual string GetTypeName(ColumnInfo ci)
        {
            return GetTypeName(DataTypeParser.Parse(ci.ValueType), ci.IsUnicode, ci.Length, ci.DecimalPart);
        }

        public virtual string GetTypeName(DataType dt, bool isUnicode, int length, int decimalPart)
        {
            var s = TypeNames[dt];
            switch (dt)
            {
                case DataType.String:
                    return GetStringNameWithLength(s, isUnicode, length);
                case DataType.Binary:
                    return GetBinaryNameWithLength(s, length);
                case DataType.Decimal:
                    return s + " (" + length + "," + decimalPart + ")";
                default:
                    return s;
            }
        }

        public virtual void InitConnection(DbDriver driver, IDbConnection conn)
        {
        }

        public virtual string DbNowString
        {
            get { return "NOW()"; }
        }

	    public virtual string EmptyString
	    {
	        get { return "''"; }
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
                    Guid key = Util.NewGuid();
                    sb.Values[0].Value = key;
                }
                SqlStatement sql = sb.ToSqlStatement(provider.Dialect, null, info.AllowSqlLog);
                provider.ExecuteNonQuery(sql);
                return sb.Values[0].Value;
            }
            return ExecuteInsertIntKey(sb, info, provider);
        }

        protected virtual object ExecuteInsertIntKey(InsertStatementBuilder sb, ObjectInfo info, DataProvider provider)
        {
            SqlStatement sql = sb.ToSqlStatement(provider.Dialect, null, info.AllowSqlLog);
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

        public virtual DbDriver CreateDbDriver(string name, string connectionString, string dbProviderFactoryName, AutoScheme autoScheme)
        {
            return new CommonDbDriver(this, name, connectionString, dbProviderFactoryName, autoScheme);
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

        public virtual SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb, List<string> queryRequiredFields)
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
                var bytes = Util.NewGuid().ToByteArray();
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
