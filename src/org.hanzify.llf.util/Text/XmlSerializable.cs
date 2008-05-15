namespace Lephone.Util.Text
{
    public abstract class XmlSerializable<T> where T : XmlSerializable<T>
    {
        public string ToXml()
        {
            return XmlSerializer<T>.Xml.Serialize((T)this);
        }

        public static T FromXml(string xml)
        {
            return XmlSerializer<T>.Xml.Deserialize(xml);
        }
    }
}
