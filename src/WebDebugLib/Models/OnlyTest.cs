using System;
using System.Runtime.Serialization;
using Lephone.Data.Definition;

namespace DebugLib.Models
{
    public interface Haha
    {
        void Tete();
    }

    public class OnlyTest : IDbObject, ISerializable, Haha
    {
        public string Name;
        public int Age;
        public DateTime Test;

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        public void Tete()
        {
        }
    }
}
