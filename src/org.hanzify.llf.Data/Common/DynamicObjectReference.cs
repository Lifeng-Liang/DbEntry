
#region usings

using System;
using System.Reflection;
using System.Runtime.Serialization;

using org.hanzify.llf.util;

#endregion

namespace org.hanzify.llf.Data.Common
{
    [Serializable]
    public class DynamicObjectReference : IObjectReference, ISerializable
    {
        public static void SerializeObject(object obj, SerializationInfo info, StreamingContext context)
        {
            Type t = obj.GetType();
            info.SetType(typeof(DynamicObjectReference));
            info.AddValue("#", t.BaseType);
            foreach (FieldInfo fi in t.GetFields(ClassHelper.InstanceFlag))
            {
                info.AddValue(fi.Name, fi.GetValue(obj));
            }
        }

        protected object RealObj;

        protected DynamicObjectReference(SerializationInfo info, StreamingContext context)
        {
            Type BaseType = (Type)info.GetValue("#", typeof(Type));
            Type t = DynamicObject.GetImplType(BaseType);
            object o = ClassHelper.CreateInstance(t);
            foreach (FieldInfo fi in t.GetFields(ClassHelper.InstanceFlag))
            {
                object v = info.GetValue(fi.Name, fi.FieldType);
                fi.SetValue(o, v);
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
