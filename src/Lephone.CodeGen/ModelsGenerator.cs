using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Data;
using Lephone.Data.SqlEntry;

namespace Lephone.CodeGen
{
    public class ModelsGenerator
    {
        public class ModelBuilder
        {
            private readonly Dictionary<Type, string> _types;
            protected StringBuilder Result;

            public ModelBuilder()
            {
                Result = new StringBuilder();
                _types = new Dictionary<Type, string>
                             {
                                 {typeof (string), "string"},
                                 {typeof (int), "int"},
                                 {typeof (short), "short"},
                                 {typeof (long), "long"},
                                 {typeof (float), "float"},
                                 {typeof (double), "double"},
                                 {typeof (DateTime), "DateTime"},
                                 {typeof (bool), "bool"},
                             };
            }

            protected string GetTypeName(Type t)
            {
                if (_types.ContainsKey(t))
                {
                    return _types[t];
                }
                return t.ToString();
            }

            public string Build(string tableName)
            {
                var list = DbEntry.Context.GetDbColumnInfoList(tableName);
                return Build(tableName, list);
            }

            protected virtual string Build(string tableName, IEnumerable<DbColumnInfo> list)
            {
                Result.Append("public").Append(GetAbstract()).Append("class ").Append(tableName);
                AppendBaseType(tableName);
                var sb = new StringBuilder();
                foreach (var info in list)
                {
                    if (IsDbModel() && info.IsKey && info.IsAutoIncrement && info.ColumnName.ToLower() != "id")
                    {
                        return GetObjectModelBuilderResult(tableName, list);
                    }
                    if(info.IsKey)
                    {
                        BuildKeyColomn(info);
                    }
                    else
                    {
                        BuildColumn(info);
                        sb.Append(GetTypeName(info.DataType));
                        sb.Append(" ");
                        sb.Append(info.ColumnName);
                        sb.Append(", ");
                    }
                }
                if(sb.Length > 2)
                {
                    sb.Length -= 2;
                    Result.Append("\tpublic").Append(GetAbstract()).Append(tableName).Append(" ");
                    Result.Append("Initialize(");
                    Result.Append(sb);
                    Result.Append(")");
                    AppendInitMethodBody();

                }
                Result.Append("}\r\n");
                return Result.ToString();
            }

            protected virtual bool IsDbModel()
            {
                return true;
            }

            protected virtual string GetObjectModelBuilderResult(string tableName, IEnumerable<DbColumnInfo> list)
            {
                return new ObjectModelBuilder().Build(tableName, list);
            }

            protected virtual string GetAbstract()
            {
                return " abstract ";
            }

            protected virtual void AppendInitMethodBody()
            {
                Result.Append(";\r\n");
            }

            protected virtual void AppendBaseType(string tableName)
            {
                Result.Append(" : LinqObjectModel<").Append(tableName).Append(">\r\n{\r\n");
            }

            protected virtual void BuildKeyColomn(DbColumnInfo info)
            {
            }

            protected virtual void BuildColumn(DbColumnInfo info)
            {
                Result.Append("\tpublic").Append(GetAbstract());
                Result.Append(GetTypeName(info.DataType));
                Result.Append(" ");
                Result.Append(info.ColumnName);
                Result.Append(GetProperty());
                Result.Append("\r\n");
            }

            protected virtual string GetProperty()
            {
                return " { get; set; }";
            }
        }

        public class ObjectModelBuilder : ModelBuilder
        {
            protected StringBuilder InitMethodBody = new StringBuilder();

            protected override string GetAbstract()
            {
                return " ";
            }

            protected override bool IsDbModel()
            {
                return false;
            }

            protected override void AppendInitMethodBody()
            {
                Result.Append("{\r\n");
                if(InitMethodBody.Length > 3)
                {
                    Result.Append(InitMethodBody);
                    Result.Append("\t\treturn this;\r\n");
                }
                Result.Append("\t}\r\n");
            }

            protected override void AppendBaseType(string tableName)
            {
                Result.Append(" : IDbObject\r\n");
            }

            protected override void BuildKeyColomn(DbColumnInfo info)
            {
                if (info.IsAutoIncrement)
                {
                    Result.Append("\t[DbKey]\r\n");
                }
                else
                {
                    Result.Append("\t[DbKey(IsDbGenerate = false)]\r\n");
                }
                BuildColumn(info);
            }

            protected override void BuildColumn(DbColumnInfo info)
            {
                base.BuildColumn(info);
                if(!info.IsKey)
                {
                    InitMethodBody.Append("\t\tthis.");
                    InitMethodBody.Append(info.ColumnName);
                    InitMethodBody.Append(" = ");
                    InitMethodBody.Append(info.ColumnName);
                    InitMethodBody.Append(";\r\n");
                }
            }

            protected override string GetProperty()
            {
                return ";";
            }

            protected override string GetObjectModelBuilderResult(string tableName, IEnumerable<DbColumnInfo> list)
            {
                return "";
            }
        }

        public List<string> GetTableList()
        {
            return DbEntry.Context.GetTableNames();
        }

        public string GenerateModelFromDatabase(string tableName)
        {
            if(tableName.ToLower() == "*")
            {
                var sb = new StringBuilder();
                foreach (var table in GetTableList())
                {
                    string s = new ModelBuilder().Build(table);
                    sb.Append(s);
                    sb.Append("\r\n");
                }
                return sb.ToString();
            }
            return new ModelBuilder().Build(tableName);
        }
    }
}
