using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Lephone.CodeGen.Processor
{
    public class IlBuilder
    {
        //private static readonly Type[] EmptyTypes = new Type[] { };
        //private static readonly MethodDefinition DateEx = typeof(Date).GetMethod("op_Explicit", new[] { typeof(DateTime) });
        //private static readonly MethodDefinition TimeEx = typeof(Time).GetMethod("op_Explicit", new[] { typeof(DateTime) });

        private readonly ILProcessor _il;

        public IlBuilder(ILProcessor il)
        {
            this._il = il;
        }

        //public IlBuilder DeclareLocal(Type t)
        //{
        //    il.DeclareLocal(t);
        //    return this;
        //}

        public IlBuilder Emit(OpCode opCode)
        {
            _il.Emit(opCode);
            return this;
        }

        public IlBuilder LoadInt(int n)
        {
            switch (n)
            {
                case 0:
                    _il.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    _il.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    _il.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    _il.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    _il.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    _il.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    _il.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    _il.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    _il.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    _il.Emit(OpCodes.Ldc_I4, n);
                    break;
            }
            return this;
        }

        public IlBuilder LoadArg(int n)
        {
            switch (n)
            {
                case 0:
                    _il.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    _il.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    _il.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    _il.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    _il.Emit(OpCodes.Ldarg, n);
                    break;
            }
            return this;
        }

        public IlBuilder LoadArgShort(int min, int max)
        {
            for (int i = min; i <= max; i++)
            {
                _il.Emit(OpCodes.Ldarg_S, i);
            }
            return this;
        }

        public IlBuilder LoadField(FieldDefinition fi)
        {
            _il.Emit(OpCodes.Ldfld, fi);
            return this;
        }

        public IlBuilder SetField(FieldDefinition fi)
        {
            _il.Emit(OpCodes.Stfld, fi);
            return this;
        }

        //private static ConstructorInfo GetConstructor(Type sourceType)
        //{
        //    Type t = sourceType;
        //    ConstructorInfo ret;
        //    while ((ret = t.GetConstructor(EmptyTypes)) == null)
        //    {
        //        t = t.BaseType;
        //    }
        //    return ret;
        //}

        //public IlBuilder NewObj(ConstructorInfo ci)
        //{
        //    il.Emit(OpCodes.Newobj, ci);
        //    return this;
        //}

        //public IlBuilder NewObj(Type t)
        //{
        //    return NewObj(GetConstructor(t));
        //}

        public IlBuilder SetLoc(int n)
        {
            switch (n)
            {
                case 0:
                    _il.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    _il.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    _il.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    _il.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    _il.Emit(OpCodes.Stloc, n);
                    break;
            }
            return this;
        }

        public IlBuilder LoadLoc(int n)
        {
            switch (n)
            {
                case 0:
                    _il.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    _il.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    _il.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    _il.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    _il.Emit(OpCodes.Ldloc, n);
                    break;
            }
            return this;
        }

        public IlBuilder CallVirtual(MethodReference mi)
        {
            _il.Emit(OpCodes.Callvirt, mi);
            return this;
        }

        public IlBuilder Call(MethodReference mi)
        {
            _il.Emit(OpCodes.Call, mi);
            return this;
        }

        //public IlBuilder Call(ConstructorInfo ci)
        //{
        //    il.Emit(OpCodes.Call, ci);
        //    return this;
        //}

        public IlBuilder Return()
        {
            _il.Emit(OpCodes.Ret);
            return this;
        }

        public IlBuilder LoadString(string s)
        {
            _il.Emit(OpCodes.Ldstr, s);
            return this;
        }

        public IlBuilder LoadNull()
        {
            _il.Emit(OpCodes.Ldnull);
            return this;
        }

        //public IlBuilder CastOrUnbox(Type t)
        //{
        //    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
        //    {
        //        Type inType = t.GetGenericArguments()[0];
        //        if (ProcessDateAndTime(inType, typeof(DateTime?)))
        //        {
        //            return this;
        //        }
        //    }
        //    if (t.IsValueType)
        //    {
        //        if (ProcessDateAndTime(t, typeof(DateTime)))
        //        {
        //            return this;
        //        }
        //        if (t == typeof(bool))
        //        {
        //            t = typeof(bool);
        //        }
        //        else if (t == typeof(uint))
        //        {
        //            t = typeof(int);
        //        }
        //        else if (t == typeof(ulong))
        //        {
        //            t = typeof(long);
        //        }
        //        else if (t == typeof(ushort))
        //        {
        //            t = typeof(short);
        //        }
        //        il.Emit(OpCodes.Unbox_Any, t);
        //    }
        //    else
        //    {
        //        il.Emit(OpCodes.Castclass, t);
        //    }
        //    return this;
        //}

        //private bool ProcessDateAndTime(Type inType, Type unboxType)
        //{
        //    if (inType == typeof(Date))
        //    {
        //        il.Emit(OpCodes.Unbox_Any, unboxType);
        //        il.Emit(OpCodes.Call, DateEx);
        //        return true;
        //    }
        //    if (inType == typeof(Time))
        //    {
        //        il.Emit(OpCodes.Unbox_Any, unboxType);
        //        il.Emit(OpCodes.Call, TimeEx);
        //        return true;
        //    }
        //    return false;
        //}

        public IlBuilder Cast(TypeDefinition t)
        {
            _il.Emit(OpCodes.Castclass, t);
            return this;
        }

        public IlBuilder Box(TypeDefinition t)
        {
            if (t.IsValueType)
            {
                _il.Emit(OpCodes.Box, t);
            }
            return this;
        }

        public IlBuilder Ceq()
        {
            _il.Emit(OpCodes.Ceq);
            return this;
        }

        //public IlBuilder Br_S(Label label)
        //{
        //    il.Emit(OpCodes.Br_S, label);
        //    return this;
        //}

        //public IlBuilder BrTrue_S(Label label)
        //{
        //    il.Emit(OpCodes.Brtrue_S, label);
        //    return this;
        //}

        //public IlBuilder BrFalse_S(Label label)
        //{
        //    il.Emit(OpCodes.Brfalse_S, label);
        //    return this;
        //}

        //public Label DefineLabel()
        //{
        //    return il.DefineLabel();
        //}

        //public IlBuilder MarkLabel(Label label)
        //{
        //    il.MarkLabel(label);
        //    return this;
        //}

        public IlBuilder LoadLocala_S(int index)
        {
            _il.Emit(OpCodes.Ldloca_S, index);
            return this;
        }

        //public IlBuilder Bne_Un_S(Label label)
        //{
        //    il.Emit(OpCodes.Bne_Un_S, label);
        //    return this;
        //}

        public IlBuilder Conv_R4()
        {
            _il.Emit(OpCodes.Conv_R4);
            return this;
        }

        public IlBuilder Conv_R8()
        {
            _il.Emit(OpCodes.Conv_R8);
            return this;
        }

        public IlBuilder ConvFloaty(Type type)
        {
            if (type == typeof(float))
            {
                _il.Emit(OpCodes.Conv_R4);
            }
            else if (type == typeof(double))
            {
                _il.Emit(OpCodes.Conv_R8);
            }
            return this;
        }
    }
}
