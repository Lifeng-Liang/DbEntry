using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Leafing.Core;
using Leafing.Data.Model.Member;

namespace Leafing.Data.Common {
    // for stupid oracle
    public class StupidDataReader : DbDataReader {
        private readonly Dictionary<int, Type> _indexType;
        private readonly Dictionary<string, Type> _nameType;
        protected IDataReader DataReader;

        public StupidDataReader(IDataReader dr, Type returnType) {
            this.DataReader = dr;
            if (returnType != null) {
                _indexType = new Dictionary<int, Type>();
                _nameType = new Dictionary<string, Type>();
                var ctx = ModelContext.GetInstance(returnType);
                int n = 0;
                foreach (MemberHandler mh in ctx.Info.SimpleMembers) {
                    _indexType.Add(n++, mh.MemberType);
                    if (!_nameType.ContainsKey(mh.Name)) {
                        _nameType.Add(mh.Name, mh.MemberType);
                    }
                }
                foreach (MemberHandler mh in ctx.Info.RelationMembers) {
                    if (mh.Is.BelongsTo) {
                        var ctx1 = ModelContext.GetInstance(mh.MemberType.GetGenericArguments()[0]);
                        _indexType.Add(n++, ctx1.Info.KeyMembers[0].MemberType);
                        _nameType.Add(mh.Name, ctx1.Info.KeyMembers[0].MemberType);
                    }
                }
            }
        }

        #region transfer

        public override void Close() {
            DataReader.Close();
        }

        public override int Depth {
            get { return DataReader.Depth; }
        }

        public override int FieldCount {
            get { return DataReader.FieldCount; }
        }

        public override string GetDataTypeName(int ordinal) {
            return DataReader.GetDataTypeName(ordinal);
        }

        public override DateTime GetDateTime(int ordinal) {
            return DataReader.GetDateTime(ordinal);
        }

        public override decimal GetDecimal(int ordinal) {
            return DataReader.GetDecimal(ordinal);
        }

        public override double GetDouble(int ordinal) {
            return DataReader.GetDouble(ordinal);
        }

        public override System.Collections.IEnumerator GetEnumerator() {
            return ((DbDataReader)DataReader).GetEnumerator();
        }

        public override Type GetFieldType(int ordinal) {
            return DataReader.GetFieldType(ordinal);
        }

        public override float GetFloat(int ordinal) {
            return DataReader.GetFloat(ordinal);
        }

        public override Guid GetGuid(int ordinal) {
            return Guid.Parse(DataReader.GetString(ordinal));
        }

        public override string GetName(int ordinal) {
            return DataReader.GetName(ordinal);
        }

        public override int GetOrdinal(string name) {
            return DataReader.GetOrdinal(name);
        }

        public override DataTable GetSchemaTable() {
            return DataReader.GetSchemaTable();
        }

        public override string GetString(int ordinal) {
            return DataReader.GetString(ordinal);
        }

        public override object GetValue(int ordinal) {
            return DataReader.GetValue(ordinal);
        }

        public override int GetValues(object[] values) {
            return DataReader.GetValues(values);
        }

        public override bool HasRows {
            get { return ((DbDataReader)DataReader).HasRows; }
        }

        public override bool IsClosed {
            get { return DataReader.IsClosed; }
        }

        public override bool IsDBNull(int ordinal) {
            return DataReader.IsDBNull(ordinal);
        }

        public override bool NextResult() {
            return DataReader.NextResult();
        }

        public override bool Read() {
            return DataReader.Read();
        }

        public override int RecordsAffected {
            get { return DataReader.RecordsAffected; }
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) {
            return DataReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override char GetChar(int ordinal) {
            return DataReader.GetChar(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) {
            return DataReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        #endregion

        public override object this[string name] {
            get {
                object o = DataReader[name];
                return GetObject(o, _nameType[name]);
            }
        }

        public override object this[int ordinal] {
            get {
                object o = DataReader[ordinal];
                return GetObject(o, _indexType[ordinal]);
            }
        }

        private static object GetObject(object o, Type t) {
            if (o == DBNull.Value || t == o.GetType()) {
                return o;
            }
            if (t.IsGenericType) {
                return ClassHelper.ChangeType(o, t.GetGenericArguments()[0]);
            }
            return ClassHelper.ChangeType(o, t);
        }

        public override bool GetBoolean(int ordinal) {
            decimal o = DataReader.GetDecimal(ordinal);
            return (o == 1);
        }

        public override byte GetByte(int ordinal) {
            return Convert.ToByte(DataReader[ordinal]);
        }

        public override short GetInt16(int ordinal) {
            return Convert.ToInt16(DataReader[ordinal]);
        }

        public override int GetInt32(int ordinal) {
            return Convert.ToInt32(DataReader[ordinal]);
        }

        public override long GetInt64(int ordinal) {
            return Convert.ToInt64(DataReader[ordinal]);
        }
    }
}
