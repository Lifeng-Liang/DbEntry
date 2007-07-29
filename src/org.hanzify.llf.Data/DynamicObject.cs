
#region usings

using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.Definition;

using org.hanzify.llf.util;

#endregion

namespace org.hanzify.llf.Data
{
    public static class DynamicObject
    {
        private const TypeAttributes DynamicObjectTypeAttr = 
            TypeAttributes.Class | TypeAttributes.Public;

        private const MethodAttributes ImplFlag = MethodAttributes.Public | MethodAttributes.HideBySig |
            MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;

        private static Hashtable types = Hashtable.Synchronized(new Hashtable());
        private static int index = 0;

        public static T NewObject<T>(params object[] os)
        {
            Type ImplType = GetImplType(typeof(T));
            return (T)ClassHelper.CreateInstance(ImplType, os);
        }

        public static Type GetImplType(Type SourceType)
        {
            if (types.Contains(SourceType))
            {
                return (Type)types[SourceType];
            }
            else
            {
                string TypeName = "T" + index.ToString();
                index++;

                TypeAttributes ta = DynamicObjectTypeAttr;
                Type[] interfaces = null;
                if ( SourceType.IsSerializable )
                {
                    ta |= TypeAttributes.Serializable;
                    interfaces = new Type[] { typeof(ISerializable) };
                }

                MemoryTypeBuilder tb = MemoryAssembly.Instance.DefineType(
                    TypeName, ta, SourceType, interfaces, GetCustomAttributes(SourceType));

                MethodInfo minit = SourceType.GetMethod("m_InitUpdateColumns", ClassHelper.InstanceFlag);
                MethodInfo mupdate = SourceType.GetMethod("m_ColumnUpdated", ClassHelper.InstanceFlag);

                PropertyInfo[] pis = SourceType.GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    if (pi.CanRead && pi.CanWrite)
                    {
                        if (pi.GetGetMethod().IsAbstract)
                        {
                            tb.ImplProperty(pi.Name, pi.PropertyType, SourceType, mupdate, pi);
                        }
                    }
                }

                if (SourceType.IsSerializable)
                {
                    MethodInfo mi = typeof(DynamicObjectReference).GetMethod("SerializeObject", ClassHelper.StaticFlag);
                    tb.OverrideMethod(ImplFlag, "GetObjectData", typeof(ISerializable), null,
                        new Type[] { typeof(SerializationInfo), typeof(StreamingContext) },
                        delegate(ILGenerator il)
                    {
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldarg_2);
                        il.Emit(OpCodes.Call, mi);
                    });
                }

                ConstructorInfo[] cis = GetConstructorInfos(SourceType);
                foreach (ConstructorInfo ci in cis)
                {
                    tb.DefineConstructor(MethodAttributes.Public, ci, minit);
                }

                Type t = tb.CreateType();
                types.Add(SourceType, t);
                return t;
            }
        }

        private static ConstructorInfo[] GetConstructorInfos(Type SourceType)
        {
            Type t = SourceType;
            ConstructorInfo[] ret;
            while((ret = t.GetConstructors()).Length == 0)
            {
                t = t.BaseType;
            }
            return ret;
        }

        private static CustomAttributeBuilder[] GetCustomAttributes(Type SourceType)
        {
            object[] os = SourceType.GetCustomAttributes(false);
            ArrayList al = new ArrayList();
            bool hasAttr = false;
            hasAttr |= PopulateDbTableAttribute(al, os);
            hasAttr |= PopulateJoinOnAttribute(al, os);
            if (!hasAttr)
            {
                al.Add(new CustomAttributeBuilder(
                    typeof(DbTableAttribute).GetConstructor(new Type[] { typeof(string) }),
                    new object[] { SourceType.Name }));
            }
            return (CustomAttributeBuilder[])al.ToArray(typeof(CustomAttributeBuilder));
        }

        private static bool PopulateDbTableAttribute(ArrayList al, object[] os)
        {
            foreach (object o in os)
            {
                if (o is DbTableAttribute)
                {
                    DbTableAttribute d = o as DbTableAttribute;
                    if (d.TableName != null)
                    {
                        al.Add(new CustomAttributeBuilder(
                            typeof(DbTableAttribute).GetConstructor(new Type[] { typeof(string) }),
                            new object[] { d.TableName }));
                    }
                    else
                    {
                        al.Add(new CustomAttributeBuilder(
                            typeof(DbTableAttribute).GetConstructor(new Type[] { typeof(string[]) }),
                            new object[] { d.LinkNames }));
                    }
                    return true;
                }
            }
            return false;
        }

        private static bool PopulateJoinOnAttribute(ArrayList al, object[] os)
        {
            bool hasJoinOnAttribute = false;
            foreach (object o in os)
            {
                if (o is JoinOnAttribute)
                {
                    hasJoinOnAttribute = true;
                    JoinOnAttribute j = o as JoinOnAttribute;
                    CustomAttributeBuilder c = new CustomAttributeBuilder(
                        typeof(JoinOnAttribute).GetConstructor(
                            new Type[] { typeof(int), typeof(string), typeof(string), typeof(CompareOpration), typeof(JoinMode) }),
                        new object[] { j.Index, j.joinner.Key1, j.joinner.Key2, j.joinner.comp, j.joinner.mode });
                    al.Add(c);
                }
            }
            return hasJoinOnAttribute;
        }
    }
}
