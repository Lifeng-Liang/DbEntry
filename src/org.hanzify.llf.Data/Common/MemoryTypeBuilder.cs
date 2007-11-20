
#region usings

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Collections;

using Lephone.Util;
using Lephone.Data.Definition;

#endregion

namespace Lephone.Data.Common
{
    internal enum FieldType
    {
        Normal,
        HasOne,
        HasMany,
        BelongsTo,
        HasAndBelongsToMany,
        LazyLoad
    }

    internal class MemoryTypeBuilder
    {
        public delegate void EmitCode(ILGenerator il);

        private const MethodAttributes OverrideFlag = MethodAttributes.Public |
            MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual;

        public static readonly string FieldPrifix = "$";

        private TypeBuilder InnerType;

        public MemoryTypeBuilder(TypeBuilder builder)
        {
            this.InnerType = builder;
        }

        public void DefineDefaultConstructor(MethodAttributes attr)
        {
            InnerType.DefineDefaultConstructor(attr);
        }

        public void DefineConstructor(MethodAttributes attr, ConstructorInfo ci, MethodInfo minit, List<MemberHandler> impRelations)
        {
            ParameterInfo[] pis = ci.GetParameters();

            ArrayList al = new ArrayList();
            foreach (ParameterInfo pi in pis)
            {
                al.Add(pi.ParameterType);
            }
            Type[] ts = (Type[])al.ToArray(typeof(Type));

            ConstructorBuilder cb = InnerType.DefineConstructor(attr,
                CallingConventions.ExplicitThis | CallingConventions.HasThis, ts);
            ILGenerator il = cb.GetILGenerator();
            // call base consructor
            il.Emit(OpCodes.Ldarg_0);
            for (int i = 1; i <= ts.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_S, i);
            }
            il.Emit(OpCodes.Call, ci);
            // create relation fields.
            foreach (MemberHandler h in impRelations)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_0);
                ConstructorInfo ci1;
                if (h.IsHasOne || h.IsHasMany || h.IsHasAndBelongsToMany)
                {
                    ci1 = h.FieldType.GetConstructor(new Type[] { typeof(object), typeof(string) });
                    if (string.IsNullOrEmpty(h.OrderByString))
                    {
                        il.Emit(OpCodes.Ldnull);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldstr, h.OrderByString);
                    }
                }
                else
                {
                    ci1 = h.FieldType.GetConstructor(new Type[] { typeof(object) });
                }
                il.Emit(OpCodes.Newobj, ci1);
                h.MemberInfo.EmitSet(il);
            }
            // call m_InitUpdateColumns
            if (minit != null)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, minit);
            }
            // return
            il.Emit(OpCodes.Ret);
        }

        public Type CreateType()
        {
            return InnerType.CreateType();
        }

        public FieldType GetFieldType(PropertyInfo pi)
        {
            foreach (Attribute a in pi.GetCustomAttributes(true))
            {
                if (a is HasOneAttribute)
                {
                    return FieldType.HasOne;
                }
                if (a is BelongsToAttribute)
                {
                    return FieldType.BelongsTo;
                }
                if (a is HasManyAttribute)
                {
                    return FieldType.HasMany;
                }
                if (a is HasAndBelongsToManyAttribute)
                {
                    return FieldType.HasAndBelongsToMany;
                }
                if (a is LazyLoadAttribute)
                {
                    return FieldType.LazyLoad;
                }
            }
            return FieldType.Normal;
        }

        private static readonly ConstructorInfo DbColumnAttributeConstructor
            = typeof(DbColumnAttribute).GetConstructor(new Type[] { typeof(string) });

        private static readonly ConstructorInfo AllowNullAttributeConstructor
            = typeof(AllowNullAttribute).GetConstructor(new Type[] { });

        private static readonly ConstructorInfo LengthAttributeConstructor
            = typeof(LengthAttribute).GetConstructor(new Type[] { typeof(int), typeof(int) });

        private CustomAttributeBuilder GetDbColumnBuilder(string Name)
        {
            return new CustomAttributeBuilder(DbColumnAttributeConstructor, new object[] { Name });
        }

        private CustomAttributeBuilder GetAllowNullBuilder()
        {
            return new CustomAttributeBuilder(AllowNullAttributeConstructor, new object[] { });
        }

        private CustomAttributeBuilder GetMaxLengthBuilder(int Min, int Max)
        {
            return new CustomAttributeBuilder(LengthAttributeConstructor, new object[] { Min, Max });
        }

        private CustomAttributeBuilder GetStringColumnBuilder(StringColumnAttribute o)
        {
            Type t = typeof(StringColumnAttribute);
            return new CustomAttributeBuilder(t.GetConstructor(new Type[] { }), new object[] { },
                new FieldInfo[] { t.GetField("IsUnicode"), t.GetField("Regular") },
                new object[] { o.IsUnicode, o.Regular });
        }

        private CustomAttributeBuilder GetIndexBuilder(IndexAttribute o)
        {
            Type t = typeof(IndexAttribute);
            return new CustomAttributeBuilder(t.GetConstructor(new Type[] { }), new object[] { },
                new FieldInfo[] { t.GetField("ASC"), t.GetField("IndexName"), t.GetField("UNIQUE") },
                new object[] { o.ASC, o.IndexName, o.UNIQUE });
        }

        private FieldInfo DefineField(string Name, Type PropertyType, FieldType ft, PropertyInfo pi)
        {
            if (ft == FieldType.Normal)
            {
                return InnerType.DefineField(Name, PropertyType, FieldAttributes.Private);
            }
            Type t = GetRealType(PropertyType, ft);
            FieldBuilder fb = InnerType.DefineField(Name, t, FieldAttributes.FamORAssem);
            DbColumnAttribute[] bs = (DbColumnAttribute[])pi.GetCustomAttributes(typeof(DbColumnAttribute), true);
            if (bs != null && bs.Length > 0)
            {
                fb.SetCustomAttribute(GetDbColumnBuilder(bs[0].Name));
            }
            else if (ft == FieldType.LazyLoad)
            {
                fb.SetCustomAttribute(GetDbColumnBuilder(pi.Name));
            }
            ProcessCustomAttribute<AllowNullAttribute>(pi, true, delegate(AllowNullAttribute o)
            {
                fb.SetCustomAttribute(GetAllowNullBuilder());
            });
            ProcessCustomAttribute<LengthAttribute>(pi, true, delegate(LengthAttribute o)
            {
                fb.SetCustomAttribute(GetMaxLengthBuilder(o.Min, o.Max));
            });
            ProcessCustomAttribute<StringColumnAttribute>(pi, true, delegate(StringColumnAttribute o)
            {
                fb.SetCustomAttribute(GetStringColumnBuilder(o));
            });
            ProcessCustomAttribute<IndexAttribute>(pi, true, delegate(IndexAttribute o)
            {
                fb.SetCustomAttribute(GetIndexBuilder(o));
            });
            return fb;
        }

        private Type GetRealType(Type PropertyType, FieldType ft)
        {
            Type t = null;
            switch (ft)
            {
                case FieldType.HasOne:
                    t = typeof(HasOne<>);
                    t = t.MakeGenericType(PropertyType);
                    break;
                case FieldType.HasMany:
                    t = typeof(HasMany<>);
                    t = t.MakeGenericType(PropertyType.GetGenericArguments()[0]);
                    break;
                case FieldType.BelongsTo:
                    t = typeof(BelongsTo<>);
                    t = t.MakeGenericType(PropertyType);
                    break;
                case FieldType.HasAndBelongsToMany:
                    t = typeof(HasAndBelongsToMany<>);
                    t = t.MakeGenericType(PropertyType.GetGenericArguments()[0]);
                    break;
                case FieldType.LazyLoad:
                    t = typeof(LazyLoadField<>);
                    t = t.MakeGenericType(PropertyType);
                    break;
                default:
                    throw new DataException("Impossible");
            }
            return t;
        }

        private void ProcessCustomAttribute<T>(PropertyInfo pi, bool inhrit, CallbackObjectHandler<T> callback)
        {
            object[] bs = pi.GetCustomAttributes(typeof(T), true);
            if (bs != null && bs.Length > 0)
            {
                callback((T)bs[0]);
            }
        }

        public MemberHandler ImplProperty(string PropertyName, Type PropertyType, Type OriginType, MethodInfo mupdate, PropertyInfo pi)
        {
            string GetPropertyName = "get_" + PropertyName;
            string SetPropertyName = "set_" + PropertyName;

            FieldType ft = GetFieldType(pi);
            FieldInfo fi = DefineField(FieldPrifix + PropertyName, PropertyType, ft, pi);

            OverrideMethod(OverrideFlag, GetPropertyName, OriginType, PropertyType, null, delegate(ILGenerator il)
            {
                il.Emit(OpCodes.Ldfld, fi);
                if (ft == FieldType.BelongsTo || ft == FieldType.HasOne || ft == FieldType.LazyLoad)
                {
                    MethodInfo getValue = fi.FieldType.GetMethod("get_Value", ClassHelper.InstancePublic);
                    il.Emit(OpCodes.Callvirt, getValue);
                }
            });

            OverrideMethod(OverrideFlag, SetPropertyName, OriginType, null, new Type[] { PropertyType }, delegate(ILGenerator il)
            {
                if (ft == FieldType.HasOne || ft == FieldType.BelongsTo || ft == FieldType.LazyLoad)
                {
                    il.Emit(OpCodes.Ldfld, fi);
                    il.Emit(OpCodes.Ldarg_1);
                    MethodInfo setValue = fi.FieldType.GetMethod("set_Value", ClassHelper.InstancePublic);
                    il.Emit(OpCodes.Callvirt, setValue);
                }
                else if (ft == FieldType.Normal)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Stfld, fi);
                }
                if (ft == FieldType.LazyLoad || ft == FieldType.Normal)
                {
                    if (mupdate != null)
                    {
                        string cn = DbObjectHelper.GetColumuName(new MemberAdapter.PropertyAdapter(pi));
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldstr, cn);
                        il.Emit(OpCodes.Call, mupdate);
                    }
                }
            });
            if (ft != FieldType.Normal)
            {
                return MemberHandler.NewObject(fi, ft, pi);
            }
            return null;
        }

        public void OverrideMethod(MethodAttributes flag, string MethodName, Type OriginType, Type returnType, Type[] paramTypes, EmitCode emitCode)
        {
            MethodBuilder mb = DefineMethod(flag, MethodName, returnType, paramTypes, emitCode);
            MethodInfo mi = OriginType.GetMethod(MethodName);
            InnerType.DefineMethodOverride(mb, mi);
        }

        public void OverrideMethodDirect(MethodAttributes flag, string MethodName, Type OriginType, Type returnType, Type[] paramTypes, EmitCode emitCode)
        {
            MethodBuilder mb = DefineMethodDirect(flag, MethodName, returnType, paramTypes, emitCode);
            MethodInfo mi = OriginType.GetMethod(MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            InnerType.DefineMethodOverride(mb, mi);
        }

        public MethodBuilder DefineMethod(MethodAttributes flag, string MethodName, Type returnType, Type[] paramTypes, EmitCode emitCode)
        {
            return DefineMethodDirect(flag, MethodName, returnType, paramTypes, delegate(ILGenerator il)
            {
                il.Emit(OpCodes.Ldarg_0);
                emitCode(il);
            });
        }

        public MethodBuilder DefineMethodDirect(MethodAttributes flag, string MethodName, Type returnType, Type[] paramTypes, EmitCode emitCode)
        {
            MethodBuilder mb = InnerType.DefineMethod(MethodName, flag, returnType, paramTypes);
            ILGenerator il = mb.GetILGenerator();
            emitCode(il);
            il.Emit(OpCodes.Ret);
            return mb;
        }
    }
}
