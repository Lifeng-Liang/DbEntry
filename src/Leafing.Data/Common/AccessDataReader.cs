using System;
using System.Data;

namespace Leafing.Data.Common
{
    public class AccessDataReader : StupidDataReader
    {
        public AccessDataReader(IDataReader dr, Type returnType)
            : base(dr, returnType)
        {
        }

        public override bool GetBoolean(int ordinal)
        {
            return DataReader.GetBoolean(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return DataReader.GetByte(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return DataReader.GetInt16(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return DataReader.GetInt32(ordinal);
        }
    }
}
