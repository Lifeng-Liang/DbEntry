
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Lephone.Data.Common
{
    // for stupid oracle
    public class StupidDataReader : DbDataReader
    {
        private Dictionary<int, Type> indexType = null;
        private Dictionary<string, Type> nameType = null;
        private IDataReader dr;

        public StupidDataReader(IDataReader dr, Type ReturnType)
        {
            this.dr = dr;
            if (ReturnType != null)
            {
                indexType = new Dictionary<int, Type>();
                nameType = new Dictionary<string, Type>();
                ObjectInfo oi = DbObjectHelper.GetObjectInfo(ReturnType);
                int n = 0;
                foreach (MemberHandler mh in oi.SimpleFields)
                {
                    indexType.Add(n++, mh.FieldType);
                    nameType.Add(mh.Name, mh.FieldType);
                }
                foreach (MemberHandler mh in oi.RelationFields)
                {
                    if (mh.IsBelongsTo)
                    {
                        ObjectInfo oi1 = DbObjectHelper.GetObjectInfo(mh.FieldType.GetGenericArguments()[0]);
                        indexType.Add(n++, oi1.KeyFields[0].FieldType);
                        nameType.Add(mh.Name, oi1.KeyFields[0].FieldType);
                    }
                }
            }
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
            return dr.GetDateTime(ordinal);
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

        public override System.Data.DataTable GetSchemaTable()
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
                object o = dr[name];
                return GetObject(o, nameType[name]);
            }
        }

        public override object this[int ordinal]
        {
            get
            {
                object o = dr[ordinal];
                return GetObject(o, indexType[ordinal]);
            }
        }

        private object GetObject(object o, Type t)
        {
            if (o == DBNull.Value || t == o.GetType())
            {
                return o;
            }
            else
            {
                if (t.IsGenericType)
                {
                    return Convert.ChangeType(o, t.GetGenericArguments()[0]);
                }
                else
                {
                    return Convert.ChangeType(o, t);
                }
            }
        }

        public override bool GetBoolean(int ordinal)
        {
            decimal o = dr.GetDecimal(ordinal);
            return (o == 1);
        }

        public override byte GetByte(int ordinal)
        {
            return Convert.ToByte(dr[ordinal]);
        }

        public override short GetInt16(int ordinal)
        {
            return Convert.ToInt16(dr[ordinal]);
        }

        public override int GetInt32(int ordinal)
        {
            return Convert.ToInt32(dr[ordinal]);
        }

        public override long GetInt64(int ordinal)
        {
            return Convert.ToInt64(dr[ordinal]);
        }
    }
}
