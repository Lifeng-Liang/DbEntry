using System;
using System.Data;

namespace Lephone.Data.Common
{
    public class AccessDataReader : StupidDataReader
    {
        public AccessDataReader(IDataReader dr, Type ReturnType)
            : base(dr, ReturnType)
        {
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
    }
}
