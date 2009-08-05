using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Data;
using Lephone.Data.SqlEntry;

namespace Lephone.CodeGen
{
    public class ModelsGenerator
    {
        private readonly Dictionary<Type, string> _types;

        public ModelsGenerator()
        {
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

        public List<string> GetTableList()
        {
            return DbEntry.Context.GetTableNames();
        }

        public string GenerateModelFromDatabase(string tableName)
        {
            if(tableName.ToLower() == "*all*")
            {
                var sb = new StringBuilder();
                foreach (var table in GetTableList())
                {
                    string s = GenerateSingleModel(table);
                    sb.Append(s);
                    sb.Append("\r\n");
                }
                return sb.ToString();
            }
            return GenerateSingleModel(tableName);
        }

        private string GenerateSingleModel(string tableName)
        {
            var list = DbEntry.Context.GetDbColumnInfoList(tableName);
            var sb = new StringBuilder();
            sb.Append("public class ").Append(tableName).Append(" : LinqObjectModel<").Append(tableName).Append(">\r\n{\r\n");
            foreach (var info in list)
            {
                if(info.IsKey && info.IsAutoIncrement && info.ColumnName.ToLower() != "id")
                {
                    return GenerateSingleModel2(tableName, list);
                }
                if(!info.IsKey)
                {
                    sb.Append("\tpublic abstract ");
                    sb.Append(GetTypeName(info.DataType));
                    sb.Append(" ");
                    sb.Append(info.ColumnName);
                    sb.Append(" { get; set; }\r\n");
                }
            }
            sb.Append("}\r\n");
            return sb.ToString();
        }

        private string GenerateSingleModel2(string tableName, IEnumerable<DbColumnInfo> list)
        {
            var sb = new StringBuilder();
            sb.Append("public class ").Append(tableName).Append(" : IDbObject\r\n");
            foreach (var info in list)
            {
                if(info.IsKey)
                {
                    if (info.IsAutoIncrement)
                    {
                        sb.Append("\t[DbKey]\r\n");
                    }
                    else
                    {
                        sb.Append("\t[DbKey(IsDbGenerate = false)]\r\n");
                    }
                }
                sb.Append("\tpublic ");
                sb.Append(GetTypeName(info.DataType));
                sb.Append(" ");
                sb.Append(info.ColumnName);
                sb.Append(";\r\n");
            }
            sb.Append("}\r\n");
            return sb.ToString();
        }

        private string GetTypeName(Type t)
        {
            if(_types.ContainsKey(t))
            {
                return _types[t];
            }
            return t.ToString();
        }
    }
}
