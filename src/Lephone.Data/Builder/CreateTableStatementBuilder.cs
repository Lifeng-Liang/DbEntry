using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder
{
    public class CreateTableStatementBuilder : ISqlStatementBuilder
    {
        internal string TableName;
        private readonly List<ColumnInfo> _Columns;
        private readonly List<DbIndex> _Indexes;

        public CreateTableStatementBuilder(string TableName)
        {
            this.TableName = TableName;
            _Columns = new List<ColumnInfo>();
            _Indexes = new List<DbIndex>();
        }

        public SqlStatement ToSqlStatement(DbDialect dd)
        {
            bool isMutiKey = IsMutiKey();
            string keys = "";
            StringBuilder sql = new StringBuilder();
            sql.Append("CREATE TABLE ");
            sql.Append(dd.QuoteForLimitTableName(TableName));
            sql.Append(" (");

            foreach (ColumnInfo ci in _Columns)
            {
                string NullDefine = ci.AllowNull ? dd.NullString : dd.NotNullString;
                sql.Append("\n\t");
                sql.Append(dd.QuoteForColumnName(ci.Key));
                sql.Append(" ");
                if (ci.IsDbGenerate && dd.IdentityTypeString != null)
                {
                    sql.Append(dd.IdentityTypeString);
                }
                else
                {
                    sql.Append(dd.GetTypeName(DataTypeParser.Parse(ci.ValueType), ci.IsUnicode, ci.Length));
                }
                if (ci.IsDbGenerate)
                {
                    sql.Append(" ").Append(dd.IdentityColumnString);
                }
                if (ci.IsKey)
                {
                    if (isMutiKey)
                    {
                        sql.Append(NullDefine);
                        keys += dd.QuoteForColumnName(ci.Key) + ", ";
                    }
                    else if (ci.ValueType == typeof(Guid))
                    {
                        sql.Append(" PRIMARY KEY");
                    }
                    else
                    {
                        if (!dd.IdentityIncludePKString)
                        {
                            sql.Append(" PRIMARY KEY");
                        }
                    }
                }
                else
                {
                    sql.Append(NullDefine);
                }
                sql.Append(",");
            }
            if (_Columns.Count != 0)
            {
                if (isMutiKey)
                {
                    sql.Append("\n\tPRIMARY KEY(").Append(keys.Substring(0, keys.Length-2)).Append(")");
                }
                else
                {
                    sql.Length--;
                }
            }
            sql.Append("\n);\n");
            if (HasOneDbGenKey())
            {
                sql.Append(dd.GetCreateSequenceString(TableName));
            }
            // Create Index
            AddCreateIndexStatement(sql, dd);
            return new SqlStatement(CommandType.Text, sql.ToString());
        }

        private bool IsMutiKey()
        {
            int n = 0;
            foreach (ColumnInfo ci in _Columns)
            {
                if (ci.IsKey) { n++; }
            }
            return n > 1;
        }

        private bool HasOneDbGenKey()
        {
            foreach (ColumnInfo ci in _Columns)
            {
                if (ci.IsKey && ci.IsDbGenerate) { return true; }
            }
            return false;
        }

        private void AddCreateIndexStatement(StringBuilder sb, DbDialect dd)
        {
            string prefix = "IX_" + TableName.Replace('.', '_') + "_";
            foreach (DbIndex i in _Indexes)
            {
                string n = prefix;
                n += (i.IndexName != null) ? i.IndexName : i.Columns[0].Key;
                if (i.UNIQUE)
                {
                    sb.Append("CREATE UNIQUE ");
                }
                else
                {
                    sb.Append("CREATE ");
                }
                if (!dd.SupportDirctionOfEachColumnInIndex)
                {
                    if (i.Columns[0] is DESC)
                    {
                        sb.Append("DESC ");
                    }
                }
                sb.Append("INDEX ");
                sb.Append(dd.QuoteForColumnName(n));
                sb.Append(" ON ");
                sb.Append(dd.QuoteForLimitTableName(TableName));
                sb.Append(" (");
                foreach (ASC c in i.Columns)
                {
                    if (dd.SupportDirctionOfEachColumnInIndex)
                    {
                        sb.Append(c.ToString(dd));
                    }
                    else
                    {
                        sb.Append(dd.QuoteForColumnName(c.Key));
                    }
                    sb.Append(", ");
                }
                if (i.Columns.Length > 0)
                {
                    sb.Length -= 2;
                }
                sb.Append(");\n");
            }
        }

        public List<ColumnInfo> Columns
        {
            get { return _Columns; }
        }

        public List<DbIndex> Indexes
        {
            get { return _Indexes; }
        }
    }
}
