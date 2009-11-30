using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;
using Lephone.Util;
using Lephone.Util.Text;

namespace Lephone.Data.Common
{
    public class DynamicObjectBuilder
    {
        public static DynamicObjectBuilder Instance = new DynamicObjectBuilder();

        private const TypeAttributes DynamicObjectTypeAttr =
            TypeAttributes.Class | TypeAttributes.Public;

        private const MethodAttributes ImplFlag = MethodAttributes.Public | MethodAttributes.HideBySig |
            MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;

        private const MethodAttributes OverridePublicFlag = MethodAttributes.Public | MethodAttributes.HideBySig |
            MethodAttributes.Virtual;

        private const MethodAttributes OverrideFlag = MethodAttributes.Family | MethodAttributes.HideBySig |
            MethodAttributes.Virtual;

        private static readonly Type ObjType = typeof(object);

        private static readonly Type[] EmptyTypes = new Type[] { };

        private static readonly Type VhBaseType = typeof(EmitObjectHandlerBase);
        private static readonly DataReaderEmitHelper Helper = new DataReaderEmitHelper();

        protected DynamicObjectBuilder()
        {
        }

        public T NewObject<T>(params object[] os)
        {
            if (os.Length > 0)
            {
                Type implType = AssemblyHandler.Instance.GetImplType(typeof(T));
                return (T)ClassHelper.CreateInstance(implType, os);
            }
            return (T)ObjectInfo.GetInstance(typeof(T)).NewObject();
        }

        internal Type GetDbObjectHandler(Type srcType, ObjectInfo oi)
        {
            // TODO: process null value, nullable
            ConstructorInfo ci = GetConstructor(srcType);

            MemoryTypeBuilder tb = MemoryAssembly.Instance.DefineType(
                DynamicObjectTypeAttr, VhBaseType, new[] { typeof(IDbObjectHandler) },
                new[]{new CustomAttributeBuilder(typeof(ForTypeAttribute).GetConstructor(new[] { typeof(Type) }),
                            new object[] { srcType }) });

            tb.DefineDefaultConstructor(MethodAttributes.Public);
            // implements CreateInstance
            tb.OverrideMethodDirect(OverridePublicFlag, "CreateInstance", VhBaseType,
                ObjType, EmptyTypes, il => il.NewObj(ci));
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

        private void OverrideLoadSimpleValuesByIndex(MemoryTypeBuilder tb, Type srcType, MemberHandler[] simpleFields)
        {
            MethodInfo mi = Helper.GetMethodInfo(true);
            MethodInfo miGetNullable = VhBaseType.GetMethod("GetNullable", BindingFlags.NonPublic | BindingFlags.Instance);
            tb.OverrideMethodDirect(OverrideFlag, "LoadSimpleValuesByIndex", VhBaseType, null,
                new[] { typeof(object), typeof(IDataReader) }, delegate(ILBuilder il)
                {
                    // User u = (User)o;
                    il.DeclareLocal(srcType).LoadArg(1).Cast(srcType).SetLoc(0);
                    // set values
                    int n = 0;
                    foreach (MemberHandler f in simpleFields)
                    {
                        il.LoadLoc(0);
                        if (f.IsDataReaderInitalize)
                        {
                            il.NewObj(f.FieldType);
                            il.LoadArg(2);
                            il.LoadInt(n);
                            MethodInfo miInit = f.FieldType.GetMethod("Initalize");
                            il.Call(miInit);
                            il.Cast(f.FieldType);
                            f.MemberInfo.EmitSet(il);
                            n += f.DataReaderInitalizeFieldCount;
                        }
                        else
                        {
                            if (f.AllowNull) { il.LoadArg(0); }
                            il.LoadArg(2).LoadInt(n);
                            MethodInfo mi1 = Helper.GetMethodInfo(f.FieldType);
                            if (f.AllowNull || mi1 == null)
                            {
                                il.CallVirtual(mi);
                                if (f.AllowNull)
                                {
                                    Set2ndArgForGetNullable(f, il);
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
                    }
                });
        }

        private void OverrideLoadSimpleValuesByName(MemoryTypeBuilder tb, Type srcType, MemberHandler[] simpleFields)
        {
            MethodInfo mi = Helper.GetMethodInfo();
            MethodInfo miGetNullable = VhBaseType.GetMethod("GetNullable", BindingFlags.NonPublic | BindingFlags.Instance);
            tb.OverrideMethodDirect(OverrideFlag, "LoadSimpleValuesByName", VhBaseType, null,
                new[] { typeof(object), typeof(IDataReader) }, delegate(ILBuilder il)
                {
                    // User u = (User)o;
                    il.DeclareLocal(srcType).LoadArg(1).Cast(srcType).SetLoc(0);
                    // set values
                    foreach (MemberHandler f in simpleFields)
                    {
                        // get value
                        il.LoadLoc(0);
                        if (f.AllowNull) { il.LoadArg(0); }
                        il.LoadArg(2).LoadString(f.Name).CallVirtual(mi);
                        if (f.AllowNull)
                        {
                            Set2ndArgForGetNullable(f, il);
                            il.Call(miGetNullable);
                        }
                        // cast or unbox
                        il.CastOrUnbox(f.FieldType);
                        // set field
                        f.MemberInfo.EmitSet(il);
                    }
                });
        }

        private void Set2ndArgForGetNullable(MemberHandler f, ILBuilder il)
        {
            if (f.MemberInfo.MemberType.IsValueType && f.MemberInfo.MemberType.GetGenericArguments()[0] == typeof(Guid))
            {
                il.LoadInt(1);
            }
            else if (f.MemberInfo.MemberType.IsValueType && f.MemberInfo.MemberType.GetGenericArguments()[0] == typeof(bool))
            {
                il.LoadInt(2);
            }
            else
            {
                il.LoadInt(0);
            }
        }

        private void OverrideLoadRelationValues(MemoryTypeBuilder tb, Type srcType,
            MemberHandler[] relationFields, int index, bool useIndex)
        {
            string methodName = useIndex ? "LoadRelationValuesByIndex" : "LoadRelationValuesByName";
            tb.OverrideMethodDirect(OverrideFlag, methodName, VhBaseType, null,
                new[] { typeof(DbContext), typeof(object), typeof(IDataReader) }, delegate(ILBuilder il)
                {
                    // User u = (User)o;
                    il.DeclareLocal(srcType).LoadArg(2).Cast(srcType).SetLoc(0);
                    // set values
                    MethodInfo mi = typeof(ILazyLoading).GetMethod("Init");
                    foreach (MemberHandler f in relationFields)
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
                            if (useIndex)
                            {
                                il.LoadInt(index++).CallVirtual(Helper.GetMethodInfo(true));
                            }
                            else
                            {
                                il.LoadString(f.Name).CallVirtual(Helper.GetMethodInfo());
                            }
                            il.CallVirtual(typeof(IBelongsTo).GetMethod("set_ForeignKey"));
                        }
                    }
                });
        }

        private void OverrideGetKeyValues(MemoryTypeBuilder tb, Type srcType, MemberHandler[] keyFields)
        {
            Type t = typeof(Dictionary<string, object>);
            MethodInfo mi = t.GetMethod("Add", new[] { typeof(string), typeof(object) });
            tb.OverrideMethodDirect(OverrideFlag, "GetKeyValuesDirect", VhBaseType, null,
                new[] { t, typeof(object) }, delegate(ILBuilder il)
                {
                    // User u = (User)o;
                    il.DeclareLocal(srcType).LoadArg(2).Cast(srcType).SetLoc(0);
                    // set values
                    foreach (MemberHandler f in keyFields)
                    {
                        il.LoadArg(1).LoadString(f.Name).LoadLoc(0);
                        f.MemberInfo.EmitGet(il);
                        il.Box(f.FieldType).CallVirtual(mi);
                    }
                });
        }

        private void OverrideGetKeyValue(MemoryTypeBuilder tb, Type srcType, MemberHandler[] keyFields)
        {
            tb.OverrideMethodDirect(OverrideFlag, "GetKeyValueDirect", VhBaseType, typeof(object),
                new[] { typeof(object) }, delegate(ILBuilder il)
                {
                    if (keyFields.Length == 1)
                    {
                        MemberHandler h = keyFields[0];
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

        private void OverrideSetValuesForSelect(MemoryTypeBuilder tb, Type srcType, MemberHandler[] fields)
        {
            Type t = typeof(List<KeyValuePair<string, string>>);
            MethodInfo mi = t.GetMethod("Add", new[] { typeof(KeyValuePair<string, string>) });
            ConstructorInfo ci = typeof(KeyValuePair<string, string>).GetConstructor(new[] { typeof(string), typeof(string) });
            tb.OverrideMethodDirect(OverrideFlag, "SetValuesForSelectDirect", VhBaseType, null,
                new[] { t }, delegate(ILBuilder il)
                {
                    foreach (MemberHandler f in fields)
                    {
                        if (!f.IsHasOne && !f.IsHasMany && !f.IsHasAndBelongsToMany && !f.IsLazyLoad)
                        {
                            il.LoadArg(1);

                            il.LoadString(f.Name);
                            if (f.Name != f.MemberInfo.Name)
                            {
                                il.LoadString(f.MemberInfo.Name);
                            }
                            else
                            {
                                il.LoadNull();
                            }
                            il.NewObj(ci);

                            il.CallVirtual(mi);
                        }
                    }
                });
        }

        private void OverrideSetValuesDirect(string name, MemoryTypeBuilder tb, Type srcType,
            MemberHandler[] fields, CallbackHandler<MemberHandler, bool> cb1, CallbackHandler<MemberHandler, bool> cb2)
        {
            Type t = typeof(KeyValueCollection);
            MethodInfo addmi = t.GetMethod("Add", new[] { typeof(KeyValue) });
            MethodInfo nkvmi = VhBaseType.GetMethod("NewKeyValue", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo nkvdmi = VhBaseType.GetMethod("NewKeyValueDirect", BindingFlags.NonPublic | BindingFlags.Instance);

            tb.OverrideMethodDirect(OverrideFlag, name, VhBaseType, null,
                new[] { t, typeof(object) }, delegate(ILBuilder il)
                {
                    // User u = (User)o;
                    il.DeclareLocal(srcType).LoadArg(2).Cast(srcType).SetLoc(0);
                    // set values
                    int n = 0;
                    foreach (MemberHandler f in fields)
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

        private void OverrideSetValuesForInsert(MemoryTypeBuilder tb, Type srcType, MemberHandler[] fields)
        {
            OverrideSetValuesDirect("SetValuesForInsertDirect", tb, srcType, fields,
                                    m => m.IsUpdatedOn,
                                    m => m.IsCreatedOn || m.IsSavedOn || m.IsCount);
        }

        private void OverrideSetValuesForUpdate(MemoryTypeBuilder tb, Type srcType, MemberHandler[] fields)
        {
            if (srcType.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                OverrideSetValuesForPartialUpdate(tb, srcType, fields);
            }
            else
            {
                OverrideSetValuesDirect("SetValuesForUpdateDirect", tb, srcType, fields,
                                        m => m.IsCreatedOn || m.IsKey,
                                        m => m.IsUpdatedOn || m.IsSavedOn || m.IsCount);
            }

        }

        private void OverrideSetValuesForPartialUpdate(MemoryTypeBuilder tb, Type srcType, MemberHandler[] fields)
        {
            Type t = typeof(KeyValueCollection);
            MethodInfo akvmi = VhBaseType.GetMethod("AddKeyValue", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo addmi = t.GetMethod("Add", new[] { typeof(KeyValue) });
            MethodInfo nkvdmi = VhBaseType.GetMethod("NewKeyValueDirect", BindingFlags.NonPublic | BindingFlags.Instance);

            tb.OverrideMethodDirect(OverrideFlag, "SetValuesForUpdateDirect", VhBaseType, null,
                new[] { t, typeof(object) }, delegate(ILBuilder il)
                {
                    // User u = (User)o;
                    il.DeclareLocal(srcType).LoadArg(2).Cast(srcType).SetLoc(0);
                    // set values
                    int n = 0;
                    foreach (MemberHandler f in fields)
                    {
                        if (!f.IsDbGenerate && !f.IsHasOne && !f.IsHasMany && !f.IsHasAndBelongsToMany)
                        {
                            if (!f.IsKey && (f.IsUpdatedOn || f.IsSavedOn || !f.IsCreatedOn || f.IsCount))
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

        public virtual Type GenerateType(Type sourceType)
        {
            TypeAttributes ta = DynamicObjectTypeAttr;
            Type[] interfaces = null;
            if (sourceType.IsSerializable)
            {
                ta |= TypeAttributes.Serializable;
                interfaces = new[] { typeof(ISerializable) };
            }

            MethodInfo minit = sourceType.GetMethod("m_InitUpdateColumns", ClassHelper.InstanceFlag);
            MethodInfo mupdate = sourceType.GetMethod("m_ColumnUpdated", ClassHelper.InstanceFlag);

            PropertyInfo[] pis = sourceType.GetProperties();
            var plist = new List<PropertyInfo>();
            var impRelations = new List<MemberHandler>();
            foreach (PropertyInfo pi in pis)
            {
                if (pi.CanRead && pi.CanWrite)
                {
                    if (pi.GetGetMethod().IsAbstract)
                    {
                        if (!pi.PropertyType.IsValueType && !pi.PropertyType.IsArray && pi.PropertyType != typeof(string))
                        {
                            var ft = MemoryTypeBuilder.GetFieldType(pi);
                            if (ft == FieldType.Normal || ft == FieldType.LazyLoad)
                            {
                                throw new DataException("The property '{0}' should define as relation field and can not set lazy load attribute", pi.Name);
                            }
                        }
                        plist.Add(pi);
                    }
                }
            }

            MemoryTypeBuilder tb = MemoryAssembly.Instance.DefineType(
                ta, sourceType, interfaces, GetCustomAttributes(sourceType));

            foreach (PropertyInfo pi in plist)
            {
                MemberHandler h = tb.ImplProperty(sourceType, mupdate, pi);
                if (h != null)
                {
                    impRelations.Add(h);
                }
            }

            foreach (MethodInfo info in sourceType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                if (info.IsAbstract)
                {
                    if (info.Name == "Init" || info.Name == "Initialize")
                    {
                        if (info.ReturnType == sourceType)
                        {
                            tb.ImplInitialize(sourceType, info);
                        }
                    }
                }
            }

            if (sourceType.IsSerializable)
            {
                MethodInfo mi = typeof(DynamicObjectReference).GetMethod("SerializeObject", ClassHelper.StaticFlag);
                tb.OverrideMethod(ImplFlag, "GetObjectData", typeof(ISerializable), null,
                                  new[] { typeof(SerializationInfo), typeof(StreamingContext) },
                                  il => il.LoadArg(1).LoadArg(2).Call(mi));
            }

            ConstructorInfo[] cis = GetConstructorInfos(sourceType);
            foreach (ConstructorInfo ci in cis)
            {
                tb.DefineConstructor(MethodAttributes.Public, ci, minit, impRelations);
            }

            return tb.CreateType();
        }

        private ConstructorInfo GetConstructor(Type sourceType)
        {
            Type t = sourceType;
            ConstructorInfo ret;
            while ((ret = t.GetConstructor(EmptyTypes)) == null)
            {
                t = t.BaseType;
            }
            return ret;
        }

        private ConstructorInfo[] GetConstructorInfos(Type sourceType)
        {
            Type t = sourceType;
            ConstructorInfo[] ret;
            while ((ret = ClassHelper.GetPublicOrProtectedConstructors(t)).Length == 0)
            {
                t = t.BaseType;
            }
            return ret;
        }

        private CustomAttributeBuilder[] GetCustomAttributes(Type sourceType)
        {
            object[] os = sourceType.GetCustomAttributes(false);
            var al = new ArrayList();
            bool hasAttr = false;
            hasAttr |= PopulateDbTableAttribute(al, os);
            hasAttr |= PopulateJoinOnAttribute(al, os);
            if (!hasAttr)
            {
                string defaultName = NameMapper.Instance.MapName(sourceType.Name);
                al.Add(new CustomAttributeBuilder(
                    typeof(DbTableAttribute).GetConstructor(new[] { typeof(string) }),
                    new object[] { defaultName }));
            }
            return (CustomAttributeBuilder[])al.ToArray(typeof(CustomAttributeBuilder));
        }

        private bool PopulateDbTableAttribute(ArrayList al, object[] os)
        {
            foreach (object o in os)
            {
                if (o is DbTableAttribute)
                {
                    var d = o as DbTableAttribute;
                    if (d.TableName != null)
                    {
                        al.Add(new CustomAttributeBuilder(
                            typeof(DbTableAttribute).GetConstructor(new[] { typeof(string) }),
                            new object[] { d.TableName }));
                    }
                    else
                    {
                        al.Add(new CustomAttributeBuilder(
                            typeof(DbTableAttribute).GetConstructor(new[] { typeof(string[]) }),
                            new object[] { d.LinkNames }));
                    }
                    return true;
                }
            }
            return false;
        }

        private bool PopulateJoinOnAttribute(ArrayList al, object[] os)
        {
            bool hasJoinOnAttribute = false;
            foreach (object o in os)
            {
                if (o is JoinOnAttribute)
                {
                    hasJoinOnAttribute = true;
                    var j = o as JoinOnAttribute;
                    var c = new CustomAttributeBuilder(
                        typeof(JoinOnAttribute).GetConstructor(
                            new[] { typeof(int), typeof(string), typeof(string), typeof(CompareOpration), typeof(JoinMode) }),
                        new object[] { j.Index, j.joinner.Key1, j.joinner.Key2, j.joinner.comp, j.joinner.mode });
                    al.Add(c);
                }
            }
            return hasJoinOnAttribute;
        }
    }
}
