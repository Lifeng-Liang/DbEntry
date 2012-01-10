using System;
using System.Data;
using System.Data.Common;

namespace Leafing.Data.Common
{
    public class TimeSpanLessDataReader : DbDataReader
    {
        protected IDataReader dr;

        public TimeSpanLessDataReader(IDataReader dr)
        {
            this.dr = dr;
        }

        #region transfer

        public override void Close()
        {
            dr.Close();
        }

        public override int Depth
        {
            get { return dr.Depth; }
        }

        public override int FieldCount
        {
            get { return dr.FieldCount; }
        }

        public override string GetDataTypeName(int ordinal)
        {
            return dr.GetDataTypeName(ordinal);
        }

        public override DateTime GetDateTime(int ordinal)
        {
            object o = dr[ordinal];
            if(o != null)
            {
                if(o.GetType() == typeof(TimeSpan))
                {
                    var x = (TimeSpan)o;
                    return new DateTime(x.Ticks);
                }
                return (DateTime)o;
            }
            return default(DateTime);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return dr.GetDecimal(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return dr.GetDouble(ordinal);
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return ((DbDataReader)dr).GetEnumerator();
        }

        public override Type GetFieldType(int ordinal)
        {
            return dr.GetFieldType(ordinal);
        }

        public override float GetFloat(int ordinal)
        {
            return dr.GetFloat(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return dr.GetGuid(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return dr.GetName(ordinal);
        }

        public override int GetOrdinal(string name)
        {
            return dr.GetOrdinal(name);
        }

        public override DataTable GetSchemaTable()
        {
            return dr.GetSchemaTable();
        }

        public override string GetString(int ordinal)
        {
            return dr.GetString(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            return dr.GetValue(ordinal);
        }

        public override int GetValues(object[] values)
        {
            return dr.GetValues(values);
        }

        public override bool HasRows
        {
            get { return ((DbDataReader)dr).HasRows; }
        }

        public override bool IsClosed
        {
            get { return dr.IsClosed; }
        }

        public override bool IsDBNull(int ordinal)
        {
            return dr.IsDBNull(ordinal);
        }

        public override bool NextResult()
        {
            return dr.NextResult();
        }

        public override bool Read()
        {
            return dr.Read();
        }

        public override int RecordsAffected
        {
            get { return dr.RecordsAffected; }
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return dr.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override char GetChar(int ordinal)
        {
            return dr.GetChar(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return dr.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        #endregion

        public override object this[string name]
        {
            get
            {
                return dr[name];
            }
        }

        public override object this[int ordinal]
        {
            get
            {
                return dr[ordinal];
            }
        }

        public override bool GetBoolean(int ordinal)
        {
            return dr.GetBoolean(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return dr.GetByte(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return dr.GetInt16(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return dr.GetInt32(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return dr.GetInt64(ordinal);
        }
    }
}
