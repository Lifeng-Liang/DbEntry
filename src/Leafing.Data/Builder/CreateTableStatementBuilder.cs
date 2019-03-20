using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Leafing.Data.Dialect;
using Leafing.Data.SqlEntry;
using Leafing.Core.Setting;

namespace Leafing.Data.Builder {
    public class CreateTableStatementBuilder : SqlStatementBuilder {
        internal string TableName;
        public readonly List<ColumnInfo> Columns;
        public readonly List<DbIndex> Indexes;
        private readonly StringBuilder _sql;
        private bool _isMutiKey;
        private string _keys;

        public CreateTableStatementBuilder(string tableName) {
            TableName = tableName;
            Columns = new List<ColumnInfo>();
            Indexes = new List<DbIndex>();
            _sql = new StringBuilder();
        }

        protected override SqlStatement ToSqlStatement(DbDialect dd, List<string> queryRequiredFields) {
            CheckIsMutiKey(dd);
            _sql.Append("CREATE TABLE ");
            _sql.Append(dd.QuoteForTableName(TableName));
            _sql.Append(" (");

            ProcessColumns(dd);
            ProcessForeignKeys(dd);
            ProcessPrimaryKey();

            if (Columns.Count > 0) {
                _sql.Length--;
            }

            _sql.Append("\n);\n");

            if (HasOneDbGenKey()) {
                _sql.Append(dd.GetCreateSequenceString(TableName));
            }
            // Create Index
            AddCreateIndexStatement(_sql, dd);
            return new SqlStatement(CommandType.Text, _sql.ToString());
        }

        private void ProcessForeignKeys(DbDialect dd) {
            if (!ConfigReader.Config.Database.UseForeignKey) {
                return;
            }
            foreach (ColumnInfo ci in Columns) {
                if (ci.IsForeignKey) {
                    _sql.Append("\n\tFOREIGN KEY(");
                    _sql.Append(dd.QuoteForColumnName(ci.Key));
                    _sql.Append(") REFERENCES ");
                    _sql.Append(dd.QuoteForTableName(ci.BelongsToTableName));
                    _sql.Append(" ([Id]) ,");
                }
            }
        }

        private void ProcessColumns(DbDialect dd) {
            foreach (ColumnInfo ci in Columns) {
                ProcessColumn(ci, dd);
            }
        }

        private void ProcessColumn(ColumnInfo ci, DbDialect dd) {
            string nullDefine = ci.AllowNull ? dd.NullString : dd.NotNullString;
            _sql.Append("\n\t");
            _sql.Append(dd.QuoteForColumnName(ci.Key));
            _sql.Append(" ");
            if (ci.IsDbGenerate && dd.IdentityTypeString != null) {
                _sql.Append(dd.IdentityTypeString);
            } else {
                _sql.Append(dd.GetTypeName(ci));
            }
            if (ci.IsDbGenerate) {
                _sql.Append(" ").Append(dd.IdentityColumnString);
            }
            if (ci.IsKey) {
                ProcessKeyColumn(ci, dd, nullDefine);
            } else {
                _sql.Append(nullDefine);
            }
            _sql.Append(",");
        }

        private void ProcessKeyColumn(ColumnInfo ci, DbDialect dd, string nullDefine) {
            if (_isMutiKey) {
                _sql.Append(nullDefine);
            } else {
                if (ci.ValueType == typeof(Guid) || !dd.IdentityIncludePKString || !ci.IsDbGenerate) {
                    if (!ci.IsDbGenerate) {
                        _sql.Append(nullDefine);
                    }
                    _sql.Append(" PRIMARY KEY");
                }
            }
        }

        private void ProcessPrimaryKey() {
            if (_isMutiKey) {
                _sql.Append("\n\tPRIMARY KEY(").Append(_keys.Substring(0, _keys.Length - 2)).Append("),");
            }
        }

        private void CheckIsMutiKey(DbDialect dd) {
            int n = 0;
            foreach (ColumnInfo ci in Columns) {
                if (ci.IsKey) {
                    n++;
                    _keys += dd.QuoteForColumnName(ci.Key) + ", ";
                }
            }
            _isMutiKey = n > 1;
        }

        private bool HasOneDbGenKey() {
            foreach (ColumnInfo ci in Columns) {
                if (ci.IsKey && ci.IsDbGenerate) { return true; }
            }
            return false;
        }

        private void AddCreateIndexStatement(StringBuilder sb, DbDialect dd) {
            string prefix = "IX_" + TableName.Replace('.', '_') + "_";
            foreach (DbIndex i in Indexes) {
                string n = prefix;
                n += i.IndexName ?? i.Columns[0].Key;
                string gn = dd.GenIndexName(n);
                if (gn != null) {
                    n = "IX_" + gn;
                }
                sb.Append(i.Unique ? "CREATE UNIQUE " : "CREATE ");
                if (!dd.SupportDirctionOfEachColumnInIndex) {
                    if (i.Columns[0] is DESC) {
                        sb.Append("DESC ");
                    }
                }
                sb.Append("INDEX ");
                sb.Append(dd.QuoteForColumnName(n));
                sb.Append(" ON ");
                sb.Append(dd.QuoteForLimitTableName(TableName));
                sb.Append(" (");
                foreach (ASC c in i.Columns) {
                    sb.Append(dd.SupportDirctionOfEachColumnInIndex ? c.ToString(dd) : dd.QuoteForColumnName(c.Key));
                    sb.Append(", ");
                }
                if (i.Columns.Length > 0) {
                    sb.Length -= 2;
                }
                sb.Append(");\n");
            }
        }
    }
}