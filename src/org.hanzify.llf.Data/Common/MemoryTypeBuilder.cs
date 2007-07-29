
#region usings

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;

using org.hanzify.llf.util;
using org.hanzify.llf.Data.Definition;

#endregion

namespace org.hanzify.llf.Data.Common
{
    public class MemoryTypeBuilder
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

        public void DefineConstructor(MethodAttributes attr, ConstructorInfo ci, MethodInfo minit)
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

        public enum FieldType
        {
            Normal,
            HasOne,
            HasMany,
            BelongsTo,
            HasManyAndBelongsTo
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
                if (a is HasManyAndBelongsToAttribute)
                {
                    return FieldType.HasManyAndBelongsTo;
                }

            }
            return FieldType.Normal;
        }

        private static readonly ConstructorInfo DbColumnAttributeConstructor
            = typeof(DbColumnAttribute).GetConstructor(new Type[] { typeof(string) });

        private static readonly ConstructorInfo OrderByAttributeConstructor
            = typeof(OrderByAttribute).GetConstructor(new Type[] { typeof(string) });

        private CustomAttributeBuilder GetDbColumnBuilder(string Name)
        {
            return new CustomAttributeBuilder(DbColumnAttributeConstructor, new object[] { Name });
        }

        private CustomAttributeBuilder GetOrderByBuilder(string Name)
        {
            return new CustomAttributeBuilder(OrderByAttributeConstructor, new object[] { Name });
        }

        private FieldInfo DefineField(string Name, Type PropertyType, FieldType ft, PropertyInfo pi)
        {
            Type t = null;
            switch (ft)
            {
                case FieldType.Normal:
                    return InnerType.DefineField(Name, PropertyType, FieldAttributes.Private);
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
                case FieldType.HasManyAndBelongsTo:
                    t = typeof(HasManyAndBelongsTo<>);
                    t = t.MakeGenericType(PropertyType.GetGenericArguments()[0]);
                    break;
                default:
                    throw new DbEntryException("Impossible");
            }
            FieldBuilder fb = InnerType.DefineField(Name, t, FieldAttributes.Family);
            DbColumnAttribute[] bs = (DbColumnAttribute[])pi.GetCustomAttributes(typeof(DbColumnAttribute), true);
            if (bs != null && bs.Length > 0)
            {
                fb.SetCustomAttribute(GetDbColumnBuilder(bs[0].Name));
            }
            OrderByAttribute[] os = (OrderByAttribute[])pi.GetCustomAttributes(typeof(OrderByAttribute), true);
            if (os != null && os.Length > 0)
            {
                fb.SetCustomAttribute(GetOrderByBuilder(os[0].OrderBy));
            }
            return fb;
        }

        public void ImplProperty(string PropertyName, Type PropertyType, Type OriginType, MethodInfo mupdate, PropertyInfo pi)
        {
            string GetPropertyName = "get_" + PropertyName;
            string SetPropertyName = "set_" + PropertyName;

            FieldType ft = GetFieldType(pi);
            FieldInfo fi = DefineField(FieldPrifix + PropertyName, PropertyType, ft, pi);

            OverrideMethod(OverrideFlag, GetPropertyName, OriginType, PropertyType, null, delegate(ILGenerator il)
            {
                il.Emit(OpCodes.Ldfld, fi);
                if (ft == FieldType.BelongsTo || ft == FieldType.HasOne)
                {
                    MethodInfo getValue = fi.FieldType.GetMethod("get_Value", ClassHelper.InstancePublic);
                    il.Emit(OpCodes.Callvirt, getValue);
                }
            });

            OverrideMethod(OverrideFlag, SetPropertyName, OriginType, null, new Type[] { PropertyType }, delegate(ILGenerator il)
            {
                if (ft == FieldType.HasOne || ft == FieldType.BelongsTo)
                {
                    il.Emit(OpCodes.Ldfld, fi);
                    il.Emit(OpCodes.Ldarg_1);
                    MethodInfo setValue = fi.FieldType.GetMethod("set_Value", ClassHelper.InstancePublic);
                    il.Emit(OpCodes.Callvirt, setValue);
                }
                else
                {
                    if (ft == FieldType.Normal)
                    {
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Stfld, fi);
                        if (mupdate != null)
                        {
                            string cn = DbObjectHelper.GetColumuName(new MemberAdapter.PropertyAdapter(pi));
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldstr, cn);
                            il.Emit(OpCodes.Call, mupdate);
                        }
                    }
                }
            });
        }

        public void OverrideMethod(MethodAttributes flag, string MethodName, Type OriginType, Type returnType, Type[] paramTypes, EmitCode emitCode)
        {
            MethodBuilder mb = DefineMethod(flag, MethodName, returnType, paramTypes, emitCode);
            MethodInfo mi = OriginType.GetMethod(MethodName);
            InnerType.DefineMethodOverride(mb, mi);
        }

        public MethodBuilder DefineMethod(MethodAttributes flag, string MethodName, Type returnType, Type[] paramTypes, EmitCode emitCode)
        {
            MethodBuilder mb = InnerType.DefineMethod(MethodName, flag, returnType, paramTypes);
            ILGenerator il = mb.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            emitCode(il);
            il.Emit(OpCodes.Ret);
            return mb;
        }
    }
}
