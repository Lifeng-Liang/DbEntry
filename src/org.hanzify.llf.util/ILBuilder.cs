
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace Lephone.Util
{
    public class ILBuilder
    {
        private static readonly Type[] emptyTypes = new Type[] { };
        private static readonly MethodInfo dateEx = typeof(Date).GetMethod("op_Explicit", new Type[] { typeof(DateTime) });
        private static readonly MethodInfo timeEx = typeof(Time).GetMethod("op_Explicit", new Type[] { typeof(DateTime) });

        public readonly ILGenerator il;

        public ILBuilder(ILGenerator il)
        {
            this.il = il;
        }

        public ILBuilder DeclareLocal(Type t)
        {
            il.DeclareLocal(t);
            return this;
        }

        public ILBuilder LoadInt(int n)
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
            return this;
        }

        public ILBuilder LoadArg(int n)
        {
            switch (n)
            {
                case 0:
                    il.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    il.Emit(OpCodes.Ldarg, n);
                    break;
            }
            return this;
        }

        public ILBuilder LoadArgShort(int Min, int Max)
        {
            for (int i = Min; i <= Max; i++)
            {
                il.Emit(OpCodes.Ldarg_S, i);
            }
            return this;
        }

        public ILBuilder LoadField(FieldInfo fi)
        {
            il.Emit(OpCodes.Ldfld, fi);
            return this;
        }

        public ILBuilder SetField(FieldInfo fi)
        {
            il.Emit(OpCodes.Stfld, fi);
            return this;
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

        private static ConstructorInfo[] GetConstructorList(Type SourceType)
        {
            Type t = SourceType;
            ConstructorInfo[] ret;
            while ((ret = t.GetConstructors()).Length == 0)
            {
                t = t.BaseType;
            }
            return ret;
        }

        public ILBuilder NewObj(ConstructorInfo ci)
        {
            il.Emit(OpCodes.Newobj, ci);
            return this;
        }

        public ILBuilder NewObj(Type t)
        {
            return NewObj(GetConstructor(t));
        }

        public ILBuilder SetLoc(int n)
        {
            switch (n)
            {
                case 0:
                    il.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    il.Emit(OpCodes.Stloc, n);
                    break;
            }
            return null;
        }

        public ILBuilder LoadLoc(int n)
        {
            switch (n)
            {
                case 0:
                    il.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    il.Emit(OpCodes.Ldloc, n);
                    break;
            }
            return this;
        }

        public ILBuilder CallVirtual(MethodInfo mi)
        {
            il.Emit(OpCodes.Callvirt, mi);
            return this;
        }

        public ILBuilder Call(MethodInfo mi)
        {
            il.Emit(OpCodes.Call, mi);
            return this;
        }

        public ILBuilder Call(ConstructorInfo ci)
        {
            il.Emit(OpCodes.Call, ci);
            return this;
        }

        public ILBuilder Return()
        {
            il.Emit(OpCodes.Ret);
            return this;
        }

        public ILBuilder LoadString(string s)
        {
            il.Emit(OpCodes.Ldstr, s);
            return this;
        }

        public ILBuilder LoadNull()
        {
            il.Emit(OpCodes.Ldnull);
            return this;
        }

        public ILBuilder CastOrUnbox(Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type inType = t.GetGenericArguments()[0];
                if (inType == typeof(Date))
                {
                    il.Emit(OpCodes.Unbox_Any, typeof(DateTime?));
                    il.Emit(OpCodes.Call, dateEx);
                    return this;
                }
                else if (inType == typeof(Time))
                {
                    il.Emit(OpCodes.Unbox_Any, typeof(DateTime?));
                    il.Emit(OpCodes.Call, timeEx);
                    return this;
                }
            }
            if (t.IsValueType)
            {
                if (t == typeof(uint))
                {
                    t = typeof(int);
                }
                else if (t == typeof(ulong))
                {
                    t = typeof(long);
                }
                else if (t == typeof(ushort))
                {
                    t = typeof(short);
                }
                il.Emit(OpCodes.Unbox_Any, t);
            }
            else
            {
                il.Emit(OpCodes.Castclass, t);
            }
            return this;
        }

        public ILBuilder Cast(Type t)
        {
            il.Emit(OpCodes.Castclass, t);
            return this;
        }

        public ILBuilder Box(Type t)
        {
            if (t.IsValueType)
            {
                il.Emit(OpCodes.Box, t);
            }
            return this;
        }
    }
}
