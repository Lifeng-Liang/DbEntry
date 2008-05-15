using System;
using System.Collections.Generic;
using System.Data.Common;
using Lephone.MockSql.Recorder;

namespace Lephone.MockSql
{
    public class RecorderReader : DbDataReader
    {
        public List<string> CurRowNames
        {
            get { return StaticRecorder.CurRowNames; }
            set { StaticRecorder.CurRowNames = value; }
        }

        public List<object> CurRow
        {
            get { return StaticRecorder.CurRow; }
            set { StaticRecorder.CurRow = value; }
        }

        public List<Type> CurRowTypes
        {
            get { return StaticRecorder.CurRowTypes; }
            set { StaticRecorder.CurRowTypes = value; }
        }

        private bool IsClose;

        public override void Close()
        {
            CurRow = null;
            CurRowNames = null;
            CurRowTypes = null;
            IsClose = true;
        }

        public override int Depth
        {
            get { return 1; }
        }

        public override int FieldCount
        {
            get
            {
                if (CurRow != null) return CurRow.Count;
                throw new MockDbException("CurRow is null.");
            }
        }

        public override bool GetBoolean(int ordinal)
        {
            return (bool)GetValue(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return (byte)GetValue(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override char GetChar(int ordinal)
        {
            return (char)GetValue(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return (DateTime)GetValue(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return (decimal)GetValue(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return (double)GetValue(ordinal);
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override Type GetFieldType(int ordinal)
        {
            if (CurRowTypes != null) return CurRowTypes[ordinal];
            throw new MockDbException("CurRowTypes is null.");
        }

        public override float GetFloat(int ordinal)
        {
            return (float)GetValue(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return (Guid)GetValue(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return (short)GetValue(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return (int)GetValue(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return (long)GetValue(ordinal);
        }

        public override string GetName(int ordinal)
        {
            if (CurRowNames != null) return CurRowNames[ordinal];
            throw new MockDbException("CurRowNames is null.");
        }

        public override int GetOrdinal(string name)
        {
            if (CurRowNames != null)
                return CurRowNames.FindIndex(delegate(string s)
                {
                    return (name == s);
                });
            throw new MockDbException("CurRowNames is null.");
        }

        public override System.Data.DataTable GetSchemaTable()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetString(int ordinal)
        {
            return (string)GetValue(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            if (CurRow != null)
            {
                object o = CurRow[ordinal];
                if (o == null)
                {
                    return DBNull.Value;
                }
                return o;
            }
            throw new MockDbException("type error.");
        }

        public override int GetValues(object[] values)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool HasRows
        {
            get { return (CurRow != null); }
        }

        public override bool IsClosed
        {
            get { return IsClose; }
        }

        public override bool IsDBNull(int ordinal)
        {
            return (GetValue(ordinal) == null);
        }

        public override bool NextResult()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private int index;

        public override bool Read()
        {
            if (index++ == 0)
            {
                return (CurRow != null);
            }
            CurRow = null;
            CurRowNames = null;
            CurRowTypes = null;
            return false;
        }

        public override int RecordsAffected
        {
            get { return 1; }
        }

        public override object this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }

        public override object this[int ordinal]
        {
            get { return GetValue(ordinal); }
        }
    }
}
