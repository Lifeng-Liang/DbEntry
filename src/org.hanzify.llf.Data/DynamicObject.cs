
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
using Lephone.Util.Text;

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

        private static Hashtable types = Hashtable.Synchronized(new Hashtable());
        private static readonly Type[] emptyTypes = new Type[] { };

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
                objType, emptyTypes, delegate(ILBuilder il)
            {
                il.NewObj(ci);
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
                new Type[] { typeof(object), typeof(IDataReader) }, delegate(ILBuilder il)
            {
                // User u = (User)o;
                il.DeclareLocal(srcType).LoadArg(1).Cast(srcType).SetLoc(0);
                // set values
                int n = 0;
                foreach (MemberHandler f in SimpleFields)
                {
                    il.LoadLoc(0);
                    if (f.AllowNull) { il.LoadArg(0); }
                    il.LoadArg(2).LoadInt(n);
                    MethodInfo mi1 = helper.GetMethodInfo(f.FieldType);
                    if(f.AllowNull || mi1 == null)
                    {
                        il.CallVirtual(mi);
                        if (f.AllowNull)
                        {
                            il.Call(miGetNullable);
                        }
                        // cast or unbox
                        il.CastOrUnbox(f.FieldType);
                    }
                    else
                    {
                        il.CallVirtual(mi1);
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
                new Type[] { typeof(object), typeof(IDataReader) }, delegate(ILBuilder il)
            {
                // User u = (User)o;
                il.DeclareLocal(srcType).LoadArg(1).Cast(srcType).SetLoc(0);
                // set values
                foreach (MemberHandler f in SimpleFields)
                {
                    // get value
                    il.LoadLoc(0);
                    if (f.AllowNull) { il.LoadArg(0); }
                    il.LoadArg(2).LoadString(f.Name).CallVirtual(mi);
                    if (f.AllowNull)
                    {
                        il.Call(miGetNullable);
                    }
                    // cast or unbox
                    il.CastOrUnbox(f.FieldType);
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
                new Type[] { typeof(DbContext), typeof(object), typeof(IDataReader) }, delegate(ILBuilder il)
            {
                // User u = (User)o;
                il.DeclareLocal(srcType).LoadArg(2).Cast(srcType).SetLoc(0);
                // set values
                MethodInfo mi = typeof(ILazyLoading).GetMethod("Init");
                foreach (MemberHandler f in RelationFields)
                {
                    il.LoadLoc(0);
                    f.MemberInfo.EmitGet(il);
                    if (f.IsLazyLoad)
                    {
                        il.LoadArg(1).LoadString(f.Name).CallVirtual(mi);
                    }
                    else if (f.IsHasOne || f.IsHasMany)
                    {
                        ObjectInfo oi1 = ObjectInfo.GetSimpleInstance(f.FieldType.GetGenericArguments()[0]);
                        MemberHandler mh = oi1.GetBelongsTo(srcType);
                        if (mh == null)
                        {
                            throw new DataException("HasOne or HasMany and BelongsTo must be paired.");
                        }
                        il.LoadArg(1).LoadString(mh.Name).CallVirtual(mi);
                    }
                    else if (f.IsHasAndBelongsToMany)
                    {
                        ObjectInfo oi1 = ObjectInfo.GetSimpleInstance(f.FieldType.GetGenericArguments()[0]);
                        MemberHandler mh = oi1.GetHasAndBelongsToMany(srcType);
                        if (mh == null)
                        {
                            throw new DataException("HasOne or HasMany and BelongsTo must be paired.");
                        }
                        il.LoadArg(1).LoadString(mh.Name).CallVirtual(mi);
                    }
                    else if (f.IsBelongsTo)
                    {
                        il.LoadArg(3);
                        if (UseIndex)
                        {
                            il.LoadInt(Index++).CallVirtual(helper.GetMethodInfo(true));
                        }
                        else
                        {
                            il.LoadString(f.Name).CallVirtual(helper.GetMethodInfo());
                        }
                        il.CallVirtual(typeof(IBelongsTo).GetMethod("set_ForeignKey"));
                    }
                }
            });
        }

        private static void OverrideGetKeyValues(MemoryTypeBuilder tb, Type srcType, MemberHandler[] KeyFields)
        {
            Type t = typeof(Dictionary<string, object>);
            MethodInfo mi = t.GetMethod("Add", new Type[] { typeof(string), typeof(object) });
            tb.OverrideMethodDirect(OverrideFlag, "GetKeyValuesDirect", vhBaseType, null,
                new Type[] { t, typeof(object) }, delegate(ILBuilder il)
            {
                // User u = (User)o;
                il.DeclareLocal(srcType).LoadArg(2).Cast(srcType).SetLoc(0);
                // set values
                foreach (MemberHandler f in KeyFields)
                {
                    il.LoadArg(1).LoadString(f.Name).LoadLoc(0);
                    f.MemberInfo.EmitGet(il);
                    il.Box(f.FieldType).CallVirtual(mi);
                }
            });
        }

        private static void OverrideGetKeyValue(MemoryTypeBuilder tb, Type srcType, MemberHandler[] KeyFields)
        {
            tb.OverrideMethodDirect(OverrideFlag, "GetKeyValueDirect", vhBaseType, typeof(object),
                new Type[] { typeof(object) }, delegate(ILBuilder il)
            {
                if (KeyFields.Length == 1)
                {
                    MemberHandler h = KeyFields[0];
                    il.LoadArg(1).Cast(srcType);
                    h.MemberInfo.EmitGet(il);
                    il.Box(h.FieldType);
                }
                else
                {
                    il.LoadNull();
                }
            });
        }

        private static void OverrideSetValuesForSelect(MemoryTypeBuilder tb, Type srcType, MemberHandler[] Fields)
        {
            Type t = typeof(List<string>);
            MethodInfo mi = t.GetMethod("Add", new Type[] { typeof(string) });
            tb.OverrideMethodDirect(OverrideFlag, "SetValuesForSelectDirect", vhBaseType, null,
                new Type[] { t }, delegate(ILBuilder il)
            {
                foreach (MemberHandler f in Fields)
                {
                    if (!f.IsHasOne && !f.IsHasMany && !f.IsHasAndBelongsToMany && !f.IsLazyLoad)
                    {
                        il.LoadArg(1).LoadString(f.Name).CallVirtual(mi);
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

            tb.OverrideMethodDirect(OverrideFlag, Name, vhBaseType, null,
                new Type[] { t, typeof(object) }, delegate(ILBuilder il)
            {
                // User u = (User)o;
                il.DeclareLocal(srcType).LoadArg(2).Cast(srcType).SetLoc(0);
                // set values
                int n = 0;
                foreach (MemberHandler f in Fields)
                {
                    if (!f.IsDbGenerate && !f.IsHasOne && !f.IsHasMany && !f.IsHasAndBelongsToMany)
                    {
                        if (!cb1(f))
                        {
                            il.LoadArg(1).LoadArg(0).LoadInt(n);
                            if (cb2(f))
                            {
                                il.LoadInt((int)(f.IsCount ? AutoValue.Count : AutoValue.DbNow))
                                    .Box(typeof(AutoValue)).Call(nkvdmi);
                            }
                            else
                            {
                                il.LoadLoc(0);
                                f.MemberInfo.EmitGet(il);
                                if (f.IsBelongsTo)
                                {
                                    il.CallVirtual(f.FieldType.GetMethod("get_ForeignKey"));
                                }
                                else if (f.IsLazyLoad)
                                {
                                    Type it = f.FieldType.GetGenericArguments()[0];
                                    il.CallVirtual(f.FieldType.GetMethod("get_Value")).Box(it);
                                }
                                else
                                {
                                    il.Box(f.FieldType);
                                }
                                il.Call(nkvmi);
                            }
                            il.CallVirtual(addmi);
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
                delegate(MemberHandler m) { return m.IsCreatedOn || m.IsSavedOn || m.IsCount; });
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
                    delegate(MemberHandler m) { return m.IsUpdatedOn || m.IsSavedOn || m.IsCount; });
            }

        }

        private static void OverrideSetValuesForPartialUpdate(MemoryTypeBuilder tb, Type srcType, MemberHandler[] Fields)
        {
            Type t = typeof(KeyValueCollection);
            MethodInfo akvmi = vhBaseType.GetMethod("AddKeyValue", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo addmi = t.GetMethod("Add", new Type[] { typeof(KeyValue) });
            MethodInfo nkvdmi = vhBaseType.GetMethod("NewKeyValueDirect", BindingFlags.NonPublic | BindingFlags.Instance);

            tb.OverrideMethodDirect(OverrideFlag, "SetValuesForUpdateDirect", vhBaseType, null,
                new Type[] { t, typeof(object) }, delegate(ILBuilder il)
            {
                // User u = (User)o;
                il.DeclareLocal(srcType).LoadArg(2).Cast(srcType).SetLoc(0);
                // set values
                int n = 0;
                foreach (MemberHandler f in Fields)
                {
                    if (!f.IsDbGenerate && !f.IsHasOne && !f.IsHasMany && !f.IsHasAndBelongsToMany)
                    {
                        if (f.IsUpdatedOn || f.IsSavedOn || !f.IsCreatedOn || f.IsCount)
                        {
                            if (f.IsUpdatedOn || f.IsSavedOn || f.IsCount)
                            {
                                il.LoadArg(1).LoadArg(0).LoadInt(n)
                                    .LoadInt((int)(f.IsCount ? AutoValue.Count : AutoValue.DbNow)).Box(typeof(AutoValue))
                                    .Call(nkvdmi).CallVirtual(addmi);
                            }
                            else
                            {
                                il.LoadArg(0).LoadArg(1).LoadLoc(0).LoadString(f.Name).LoadInt(n).LoadLoc(0);
                                f.MemberInfo.EmitGet(il);
                                if (f.IsBelongsTo)
                                {
                                    il.CallVirtual(f.FieldType.GetMethod("get_ForeignKey"));
                                }
                                else if (f.IsLazyLoad)
                                {
                                    Type it = f.FieldType.GetGenericArguments()[0];
                                    il.CallVirtual(f.FieldType.GetMethod("get_Value")).Box(it);
                                }
                                else
                                {
                                    il.Box(f.FieldType);
                                }
                                il.Call(akvmi);
                            }
                        }
                        n++;
                    }
                }
            });
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
                        delegate(ILBuilder il)
                    {
                        il.LoadArg(1).LoadArg(2).Call(mi);
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
                string DefaultName = NameMapper.Instance.MapName(SourceType.Name);
                al.Add(new CustomAttributeBuilder(
                    typeof(DbTableAttribute).GetConstructor(new Type[] { typeof(string) }),
                    new object[] { DefaultName }));
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
