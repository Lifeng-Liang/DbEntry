using System;
using System.Reflection;
using System.Runtime.Serialization;
using Leafing.Data.Definition;
using Leafing.Core;

namespace Leafing.Data.Common
{
    [Serializable]
    public class DynamicObjectReference : IObjectReference, ISerializable
    {
        public static void SerializeObject(object obj, SerializationInfo info, StreamingContext context)
        {
            Type t = obj.GetType();
            info.SetType(typeof(DynamicObjectReference));
            info.AddValue("#", t);
            foreach (FieldInfo fi in t.GetFields(ClassHelper.InstanceFlag))
            {
                info.AddValue(fi.Name, fi.GetValue(obj));
            }
            if(obj is DbObjectSmartUpdate) // should be DbObjectModel<T, TKey>
            {
                var pi = t.GetProperty("Id");
                info.AddValue(pi.Name, pi.GetValue(obj, new object[]{}));
            }
        }

        protected object RealObj;

        protected DynamicObjectReference(SerializationInfo info, StreamingContext context)
        {
            var t = (Type)info.GetValue("#", typeof(Type));
            object o = ClassHelper.CreateInstance(t);
            foreach (FieldInfo fi in t.GetFields(ClassHelper.InstanceFlag))
            {
                object v = info.GetValue(fi.Name, fi.FieldType);
                fi.SetValue(o, v);
            }
            if (o is DbObjectSmartUpdate) // should be DbObjectModel<T, TKey>
            {
                var pi = t.GetProperty("Id");
                pi.SetValue(o, info.GetValue("Id", pi.PropertyType), new object[] {});
            }
            RealObj = o;
        }

        public object GetRealObject(StreamingContext context)
        {
            return RealObj;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // This class should never being Serialized.
        }
    }
}
