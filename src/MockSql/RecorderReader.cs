
#region usings

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

#endregion

namespace org.hanzify.llf.MockSql
{
    public class RecorderReader : DbDataReader
    {
        public override void Close()
        {
        }

        public override int Depth
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override int FieldCount
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool GetBoolean(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override byte GetByte(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override char GetChar(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
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
            throw new Exception("The method or operation is not implemented.");
        }

        public override decimal GetDecimal(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override double GetDouble(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override Type GetFieldType(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override float GetFloat(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override short GetInt16(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int GetInt32(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override long GetInt64(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetName(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int GetOrdinal(string name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataTable GetSchemaTable()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetString(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override object GetValue(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int GetValues(object[] values)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool HasRows
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool IsClosed
        {
            get { return true; }
        }

        public override bool IsDBNull(int ordinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool NextResult()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool Read()
        {
            return false;
        }

        public override int RecordsAffected
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override object this[string name]
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override object this[int ordinal]
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }
    }
}
