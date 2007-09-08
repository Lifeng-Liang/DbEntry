
#region usings

using System;

#endregion

namespace Lephone.Util.Text
{
    public abstract class StringSerializer<T>
    {
        public static StringSerializer<T> Xml = new XmlSerializer<T>();

        public abstract string Serialize(T obj);
        public abstract T Deserialize(string Source);
    }
}
