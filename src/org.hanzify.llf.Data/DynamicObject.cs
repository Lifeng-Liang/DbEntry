
#region usings

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Data.Driver;
using Lephone.Data.SqlEntry;

using Lephone.Util;

#endregion

namespace Lephone.Data
{
    internal class DataReaderEmitHelper
    {
        Dictionary<Type, string> dic;

        public DataReaderEmitHelper()
        {
            // process chars etc.
            dic = new Dictionary<Type, string>();
            dic.Add(typeof(long), "GetInt64");
            dic.Add(typeof(int), "GetInt32");
            dic.Add(typeof(short), "GetInt16");
            dic.Add(typeof(byte), "GetByte");
            dic.Add(typeof(bool), "GetBoolean");
            dic.Add(typeof(DateTime), "GetDateTime");
            dic.Add(typeof(string), "GetString");
            dic.Add(typeof(decimal), "GetDecimal");
            dic.Add(typeof(float), "GetFloat");
            dic.Add(typeof(double), "GetDouble");
            dic.Add(typeof(Guid), "GetGuid");

            dic.Add(typeof(ulong), "GetInt64");
            dic.Add(typeof(uint), "GetInt32");
            dic.Add(typeof(ushort), "GetInt16");
        }

        public MethodInfo GetMethodInfo(Type t)
        {
            Type drt = typeof(IDataRecord);
            if (dic.ContainsKey(t))
            {
                string n = dic[t];
                MethodInfo mi = drt.GetMethod(n);
                return mi;
            }
            if (t.IsEnum)
            {
                return drt.GetMethod("GetInt32");
            }
            return null;
        }

        public MethodInfo GetMethodInfo()
        {
            return GetMethodInfo(false);
        }

        public MethodInfo GetMethodInfo(bool IsInt)
        {
            if (IsInt)
            {
                return typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
            }
            else
            {
                return typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(string) });
            }
        }
    }

    public static class DynamicObject
    {
        private const TypeAttributes DynamicObjectTypeAttr = 
            TypeAttributes.Class | TypeAttributes.Public;

        private const MethodAttributes ImplFlag = MethodAttributes.Public | MethodAttributes.HideBySig |
            MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;

        private const MethodAttributes OverridePublicFlag = MethodAttributes.Public | MethodAttributes.HideBySig |
            MethodAttributes.Virtual;

        private const MethodAttributes OverrideFlag = MethodAttributes.Family | MethodAttributes.HideBySig |
            MethodAttributes.Virtual;

        private static readonly Type objType = typeof(object);
        private static readonly Type[] emptyTypes = new Type[] { };

        private static Hashtable types = Hashtable.Synchronized(new Hashtable());

        public static T NewObject<T>(params object[] os)
        {
            if (os.Length > 0)
            {
                Type ImplType = GetImplType(typeof(T));
                return (T)ClassHelper.CreateInstance(ImplType, os);
            }
            else
            {
                return (T)ObjectInfo.GetInstance(typeof(T)).NewObject();
            }
        }

        private static readonly Type vhBaseType = typeof(EmitObjectHandlerBase);
        private static DataReaderEmitHelper helper = new DataReaderEmitHelper();

        internal static IDbObjectHandler CreateDbObjectHandler(Type srcType, ObjectInfo oi)
        {
            Type t = GetDbObjectHandler(srcType, oi);
            EmitObjectHandlerBase o = (EmitObjectHandlerBase)ClassHelper.CreateInstance(t);
            o.Init(oi);
            return o;
        }

        internal static Type GetDbObjectHandler(Type srcType, ObjectInfo oi)
        {
            // TODO: process null value, nullable
            ConstructorInfo ci = GetConstructor(srcType);
            MemoryTypeBuilder tb = MemoryAssembly.Instance.DefineType(
                DynamicObjectTypeAttr, vhBaseType, new Type[] { typeof(IDbObjectHandler) });
            tb.DefineDefaultConstructor(MethodAttributes.Public);
            // implements CreateInstance
            tb.OverrideMethodDirect(OverridePublicFlag, "CreateInstance", vhBaseType,
                objType, emptyTypes, delegate(ILGenerator il)
            {
                il.Emit(OpCodes.Newobj, ci);
            });
            // implements others
            OverrideLoadSimpleValuesByIndex(tb, srcType, oi.SimpleFields);
            OverrideLoadSimpleValuesByName(tb, srcType, oi.SimpleFields);
            OverrideLoadRelationValues(tb, srcType, oi.RelationFields, oi.SimpleFields.Length, true);
            OverrideLoadRelationValues(tb, srcType, oi.RelationFields, oi.SimpleFields.Length, false);
            OverrideGetKeyValues(tb, srcType, oi.KeyFields);
            OverrideGetKeyValue(tb, srcType, oi.KeyFields);
            OverrideSetValuesForSelect(tb, srcType, oi.Fields);
            OverrideSetValuesForInsert(tb, srcType, oi.Fields);
            OverrideSetValuesForUpdate(tb, srcType, oi.Fields);
            Type t = tb.CreateType();
            return t;
        }

        private static void OverrideLoadSimpleValuesByIndex(MemoryTypeBuilder tb, Type srcType, MemberHandler[] SimpleFields)
        {
            MethodInfo mi = helper.GetMethodInfo(true);
            MethodInfo miGetNullable = vhBaseType.GetMethod("GetNullable", BindingFlags.NonPublic | BindingFlags.Instance);
            tb.OverrideMethodDirect(OverrideFlag, "LoadSimpleValuesByIndex", vhBaseType, null,
                new Type[] { typeof(object), typeof(IDataReader) }, delegate(ILGenerator il)
            {
                // User u = (User)o;
                il.DeclareLocal(srcType);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Castclass, srcType);
                il.Emit(OpCodes.Stloc_0);
                // set values
                int n = 0;
                foreach (MemberHandler f in SimpleFields)
                {
                    il.Emit(OpCodes.Ldloc_0);
                    if (f.AllowNull) { il.Emit(OpCodes.Ldarg_0); }
                    il.Emit(OpCodes.Ldarg_2);
                    EmitldcI4(il, n);
                    MethodInfo mi1 = helper.GetMethodInfo(f.FieldType);
                    if(f.AllowNull || mi1 == null)
                    {
                        il.Emit(OpCodes.Callvirt, mi);
                        if (f.AllowNull)
                        {
                            il.Emit(OpCodes.Call, miGetNullable);
                        }
                        // cast or unbox
                        if (f.FieldType.IsValueType)
                        {
                            il.Emit(OpCodes.Unbox_Any, f.FieldType);
                        }
                        else
                        {
                            il.Emit(OpCodes.Castclass, f.FieldType);
                        }
                    }
                    else
                    {
                        il.Emit(OpCodes.Callvirt, mi1);
                    }
                    f.MemberInfo.EmitSet(il);
                    n++;
                }
            });
        }

        private static void OverrideLoadSimpleValuesByName(MemoryTypeBuilder tb, Type srcType, MemberHandler[] SimpleFields)
        {
            MethodInfo mi = helper.GetMethodInfo();
            MethodInfo miGetNullable = vhBaseType.GetMethod("GetNullable", BindingFlags.NonPublic | BindingFlags.Instance);
            tb.OverrideMethodDirect(OverrideFlag, "LoadSimpleValuesByName", vhBaseType, null,
                new Type[] { typeof(object), typeof(IDataReader) }, delegate(ILGenerator il)
            {
                // User u = (User)o;
                il.DeclareLocal(srcType);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Castclass, srcType);
                il.Emit(OpCodes.Stloc_0);
                // set values
                foreach (MemberHandler f in SimpleFields)
                {
                    // get value
                    il.Emit(OpCodes.Ldloc_0);
                    if (f.AllowNull) { il.Emit(OpCodes.Ldarg_0); }
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Ldstr, f.Name);
                    il.Emit(OpCodes.Callvirt, mi);
                    if (f.AllowNull)
                    {
                        il.Emit(OpCodes.Call, miGetNullable);
                    }
                    // cast or unbox
                    if (f.FieldType.IsValueType)
                    {
                        Type t = f.FieldType;
                        if (f.FieldType == typeof(uint))
                        {
                            t = typeof(int);
                        }
                        else if (f.FieldType == typeof(ulong))
                        {
                            t = typeof(long);
                        }
                        else if (f.FieldType == typeof(ushort))
                        {
                            t = typeof(short);
                        }
                        il.Emit(OpCodes.Unbox_Any, t);
                    }
                    else
                    {
                        il.Emit(OpCodes.Castclass, f.FieldType);
                    }
                    // set field
                    f.MemberInfo.EmitSet(il);
                }
            });
        }

        private static void OverrideLoadRelationValues(MemoryTypeBuilder tb, Type srcType,
            MemberHandler[] RelationFields, int Index, bool UseIndex)
        {
            string MethodName = UseIndex ? "LoadRelationValuesByIndex" : "LoadRelationValuesByName";
            tb.OverrideMethodDirect(OverrideFlag, MethodName, vhBaseType, null,
                new Type[] { typeof(DbContext), typeof(object), typeof(IDataReader) }, delegate(ILGenerator il)
            {
                // User u = (User)o;
                il.DeclareLocal(srcType);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Castclass, srcType);
                il.Emit(OpCodes.Stloc_0);
                // set values
                MethodInfo mi = typeof(ILazyLoading).GetMethod("Init");
                foreach (MemberHandler f in RelationFields)
                {
                    il.Emit(OpCodes.Ldloc_0);
                    f.MemberInfo.EmitGet(il);
                    if (f.IsLazyLoad)
                    {
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldstr, f.Name);
                        il.Emit(OpCodes.Callvirt, mi);
                    }
                    else if (f.IsHasOne || f.IsHasMany)
                    {
                        ObjectInfo oi1 = ObjectInfo.GetSimpleInstance(f.FieldType.GetGenericArguments()[0]);
                        MemberHandler mh = oi1.GetBelongsTo(srcType);
                        if (mh == null)
                        {
                            throw new DataException("HasOne or HasMany and BelongsTo must be paired.");
                        }
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldstr, mh.Name);
                        il.Emit(OpCodes.Callvirt, mi);
                    }
                    else if (f.IsHasAndBelongsToMany)
                    {
                        ObjectInfo oi1 = ObjectInfo.GetSimpleInstance(f.FieldType.GetGenericArguments()[0]);
                        MemberHandler mh = oi1.GetHasAndBelongsToMany(srcType);
                        if (mh == null)
                        {
                            throw new DataException("HasOne or HasMany and BelongsTo must be paired.");
                        }
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldstr, mh.Name);
                        il.Emit(OpCodes.Callvirt, mi);
                    }
                    else if (f.IsBelongsTo)
                    {
                        il.Emit(OpCodes.Ldarg_3);
                        if (UseIndex)
                        {
                            EmitldcI4(il, Index++);
                            il.Emit(OpCodes.Callvirt, helper.GetMethodInfo(true));
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldstr, f.Name);
                            il.Emit(OpCodes.Callvirt, helper.GetMethodInfo());
                        }
                        il.Emit(OpCodes.Callvirt, typeof(IBelongsTo).GetMethod("set_ForeignKey"));
                    }
                }
            });
        }

        private static void OverrideGetKeyValues(MemoryTypeBuilder tb, Type srcType, MemberHandler[] KeyFields)
        {
            Type t = typeof(Dictionary<string, object>);
            MethodInfo mi = t.GetMethod("Add", new Type[] { typeof(string), typeof(object) });
            tb.OverrideMethodDirect(OverrideFlag, "GetKeyValuesDirect", vhBaseType, null,
                new Type[] { t, typeof(object) }, delegate(ILGenerator il)
            {
                // User u = (User)o;
                il.DeclareLocal(srcType);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Castclass, srcType);
                il.Emit(OpCodes.Stloc_0);
                // set values
                foreach (MemberHandler f in KeyFields)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldstr, f.Name);
                    il.Emit(OpCodes.Ldloc_0);
                    f.MemberInfo.EmitGet(il);
                    if (f.FieldType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, f.FieldType);
                    }
                    il.Emit(OpCodes.Callvirt, mi);
                }
            });
        }

        private static void OverrideGetKeyValue(MemoryTypeBuilder tb, Type srcType, MemberHandler[] KeyFields)
        {
            tb.OverrideMethodDirect(OverrideFlag, "GetKeyValueDirect", vhBaseType, typeof(object),
                new Type[] { typeof(object) }, delegate(ILGenerator il)
            {
                if (KeyFields.Length == 1)
                {
                    MemberHandler h = KeyFields[0];
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Castclass, srcType);
                    h.MemberInfo.EmitGet(il);
                    if (h.FieldType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, h.FieldType);
                    }
                }
                else
                {
                    il.Emit(OpCodes.Ldnull);
                }
            });
        }

        private static void OverrideSetValuesForSelect(MemoryTypeBuilder tb, Type srcType, MemberHandler[] Fields)
        {
            Type t = typeof(List<string>);
            MethodInfo mi = t.GetMethod("Add", new Type[] { typeof(string) });
            tb.OverrideMethodDirect(OverrideFlag, "SetValuesForSelectDirect", vhBaseType, null,
                new Type[] { t }, delegate(ILGenerator il)
            {
                foreach (MemberHandler f in Fields)
                {
                    if (!f.IsHasOne && !f.IsHasMany && !f.IsHasAndBelongsToMany && !f.IsLazyLoad)
                    {
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldstr, f.Name);
                        il.Emit(OpCodes.Callvirt, mi);
                    }
                }
            });
        }

        private static void OverrideSetValuesDirect(string Name, MemoryTypeBuilder tb, Type srcType,
            MemberHandler[] Fields, CallbackHandler<MemberHandler, bool> cb1, CallbackHandler<MemberHandler, bool> cb2)
        {
            Type t = typeof(KeyValueCollection);
            MethodInfo addmi = t.GetMethod("Add", new Type[] { typeof(KeyValue) });
            MethodInfo nkvmi = vhBaseType.GetMethod("NewKeyValue", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo nkvdmi = vhBaseType.GetMethod("NewKeyValueDirect", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo dbnowmi = typeof(DbNow).GetField("Value", ClassHelper.AllFlag);

            tb.OverrideMethodDirect(OverrideFlag, Name, vhBaseType, null,
                new Type[] { t, typeof(object) }, delegate(ILGenerator il)
            {
                // User u = (User)o;
                il.DeclareLocal(srcType);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Castclass, srcType);
                il.Emit(OpCodes.Stloc_0);
                // set values
                int n = 0;
                foreach (MemberHandler f in Fields)
                {
                    if (!f.IsDbGenerate && !f.IsHasOne && !f.IsHasMany && !f.IsHasAndBelongsToMany)
                    {
                        if (!cb1(f))
                        {
                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Ldarg_0);
                            EmitldcI4(il, n);
                            if (cb2(f))
                            {
                                il.Emit(OpCodes.Ldsfld, dbnowmi);
                                il.Emit(OpCodes.Call, nkvdmi);
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldloc_0);
                                f.MemberInfo.EmitGet(il);
                                if (f.IsBelongsTo)
                                {
                                    il.Emit(OpCodes.Callvirt, f.FieldType.GetMethod("get_ForeignKey"));
                                }
                                else if (f.IsLazyLoad)
                                {
                                    il.Emit(OpCodes.Callvirt, f.FieldType.GetMethod("get_Value"));
                                    Type it = f.FieldType.GetGenericArguments()[0];
                                    if (it.IsValueType)
                                    {
                                        il.Emit(OpCodes.Box, it);
                                    }
                                }
                                else
                                {
                                    if (f.FieldType.IsValueType)
                                    {
                                        il.Emit(OpCodes.Box, f.FieldType);
                                    }
                                }
                                il.Emit(OpCodes.Call, nkvmi);
                            }
                            il.Emit(OpCodes.Callvirt, addmi);
                        }
                        n++;
                    }
                }
            });
        }

        private static void OverrideSetValuesForInsert(MemoryTypeBuilder tb, Type srcType, MemberHandler[] Fields)
        {
            OverrideSetValuesDirect("SetValuesForInsertDirect", tb, srcType, Fields,
                delegate(MemberHandler m) { return m.IsUpdatedOn; },
                delegate(MemberHandler m) { return m.IsCreatedOn; });
        }

        private static void OverrideSetValuesForUpdate(MemoryTypeBuilder tb, Type srcType, MemberHandler[] Fields)
        {
            if (srcType.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                OverrideSetValuesForPartialUpdate(tb, srcType, Fields);
            }
            else
            {
                OverrideSetValuesDirect("SetValuesForUpdateDirect", tb, srcType, Fields,
                    delegate(MemberHandler m) { return m.IsCreatedOn; },
                    delegate(MemberHandler m) { return m.IsUpdatedOn; });
            }

        }

        private static void OverrideSetValuesForPartialUpdate(MemoryTypeBuilder tb, Type srcType, MemberHandler[] Fields)
        {
            Type t = typeof(KeyValueCollection);
            MethodInfo akvmi = vhBaseType.GetMethod("AddKeyValue", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo addmi = t.GetMethod("Add", new Type[] { typeof(KeyValue) });
            MethodInfo nkvdmi = vhBaseType.GetMethod("NewKeyValueDirect", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo dbnowmi = typeof(DbNow).GetField("Value", ClassHelper.AllFlag);

            tb.OverrideMethodDirect(OverrideFlag, "SetValuesForUpdateDirect", vhBaseType, null,
                new Type[] { t, typeof(object) }, delegate(ILGenerator il)
            {
                // User u = (User)o;
                il.DeclareLocal(srcType);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Castclass, srcType);
                il.Emit(OpCodes.Stloc_0);
                // set values
                int n = 0;
                foreach (MemberHandler f in Fields)
                {
                    if (!f.IsDbGenerate && !f.IsHasOne && !f.IsHasMany && !f.IsHasAndBelongsToMany)
                    {
                        if (f.IsUpdatedOn || !f.IsCreatedOn)
                        {
                            if (f.IsUpdatedOn)
                            {
                                il.Emit(OpCodes.Ldarg_1);
                                il.Emit(OpCodes.Ldarg_0);
                                EmitldcI4(il, n);
                                il.Emit(OpCodes.Ldsfld, dbnowmi);
                                il.Emit(OpCodes.Call, nkvdmi);
                                il.Emit(OpCodes.Callvirt, addmi);
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Ldarg_1);
                                il.Emit(OpCodes.Ldloc_0);
                                //il.Emit(OpCodes.Ldstr, f.IsBelongsTo ? "$" : f.Name);
                                il.Emit(OpCodes.Ldstr, f.Name);
                                EmitldcI4(il, n);
                                il.Emit(OpCodes.Ldloc_0);
                                f.MemberInfo.EmitGet(il);
                                if (f.IsBelongsTo)
                                {
                                    il.Emit(OpCodes.Callvirt, f.FieldType.GetMethod("get_ForeignKey"));
                                }
                                else if (f.IsLazyLoad)
                                {
                                    il.Emit(OpCodes.Callvirt, f.FieldType.GetMethod("get_Value"));
                                    Type it = f.FieldType.GetGenericArguments()[0];
                                    if (it.IsValueType)
                                    {
                                        il.Emit(OpCodes.Box, it);
                                    }
                                }
                                else
                                {
                                    if (f.FieldType.IsValueType)
                                    {
                                        il.Emit(OpCodes.Box, f.FieldType);
                                    }
                                }
                                il.Emit(OpCodes.Call, akvmi);
                            }
                        }
                        n++;
                    }
                }
            });
        }

        private static void EmitldcI4(ILGenerator il, int n)
        {
            switch (n)
            {
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    il.Emit(OpCodes.Ldc_I4, n);
                    break;
            }
        }

        public static Type GetImplType(Type SourceType)
        {
            if (types.Contains(SourceType))
            {
                return (Type)types[SourceType];
            }
            else
            {
                TypeAttributes ta = DynamicObjectTypeAttr;
                Type[] interfaces = null;
                if ( SourceType.IsSerializable )
                {
                    ta |= TypeAttributes.Serializable;
                    interfaces = new Type[] { typeof(ISerializable) };
                }

                MemoryTypeBuilder tb = MemoryAssembly.Instance.DefineType(
                    ta, SourceType, interfaces, GetCustomAttributes(SourceType));

                MethodInfo minit = SourceType.GetMethod("m_InitUpdateColumns", ClassHelper.InstanceFlag);
                MethodInfo mupdate = SourceType.GetMethod("m_ColumnUpdated", ClassHelper.InstanceFlag);

                PropertyInfo[] pis = SourceType.GetProperties();
                List<MemberHandler> impRelations = new List<MemberHandler>();
                foreach (PropertyInfo pi in pis)
                {
                    if (pi.CanRead && pi.CanWrite)
                    {
                        if (pi.GetGetMethod().IsAbstract)
                        {
                            MemberHandler h = tb.ImplProperty(pi.Name, pi.PropertyType, SourceType, mupdate, pi);
                            if (h != null)
                            {
                                impRelations.Add(h);
                            }
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
                    tb.DefineConstructor(MethodAttributes.Public, ci, minit, impRelations);
                }

                Type t = tb.CreateType();
                types.Add(SourceType, t);
                return t;
            }
        }

        private static ConstructorInfo GetConstructor(Type SourceType)
        {
            Type t = SourceType;
            ConstructorInfo ret;
            while ((ret = t.GetConstructor(emptyTypes)) == null)
            {
                t = t.BaseType;
            }
            return ret;
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
