using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Lephone.CodeGen.Processor
{
    public class IlBuilder
    {
        //private static readonly Type[] EmptyTypes = new Type[] { };
        //private static readonly MethodDefinition DateEx = typeof(Date).GetMethod("op_Explicit", new[] { typeof(DateTime) }));
        //private static readonly MethodDefinition TimeEx = typeof(Time).GetMethod("op_Explicit", new[] { typeof(DateTime) }));

        private readonly ILProcessor _il;

        private readonly List<Instruction> _list;

        public IlBuilder(ILProcessor il)
        {
            this._il = il;
            _list = new List<Instruction>();
        }

        public void Append()
        {
            foreach (var instruction in _list)
            {
                _il.Append(instruction);
            }
            _list.Clear();
        }

        public void InsertBefore(Instruction target)
        {
            foreach (var instruction in _list)
            {
                _il.InsertBefore(target, instruction);
            }
            _list.Clear();
        }

        public void InsertAfter(Instruction target)
        {
            foreach (var instruction in _list)
            {
                _il.InsertAfter(target, instruction);
                target = instruction;
            }
            _list.Clear();
        }

        //public IlBuilder DeclareLocal(Type t)
        //{
        //    il.DeclareLocal(t));
        //    return this;
        //}

        public IlBuilder Emit(OpCode opCode)
        {
            _list.Add(_il.Create(opCode));
            return this;
        }

        public IlBuilder LoadInt(int n)
        {
            switch (n)
            {
                case 0:
                    _list.Add(_il.Create(OpCodes.Ldc_I4_0));
                    break;
                case 1:
                    _list.Add(_il.Create(OpCodes.Ldc_I4_1));
                    break;
                case 2:
                    _list.Add(_il.Create(OpCodes.Ldc_I4_2));
                    break;
                case 3:
                    _list.Add(_il.Create(OpCodes.Ldc_I4_3));
                    break;
                case 4:
                    _list.Add(_il.Create(OpCodes.Ldc_I4_4));
                    break;
                case 5:
                    _list.Add(_il.Create(OpCodes.Ldc_I4_5));
                    break;
                case 6:
                    _list.Add(_il.Create(OpCodes.Ldc_I4_6));
                    break;
                case 7:
                    _list.Add(_il.Create(OpCodes.Ldc_I4_7));
                    break;
                case 8:
                    _list.Add(_il.Create(OpCodes.Ldc_I4_8));
                    break;
                default:
                    _list.Add(_il.Create(OpCodes.Ldc_I4, n));
                    break;
            }
            return this;
        }

        public IlBuilder LoadArg(int n)
        {
            switch (n)
            {
                case 0:
                    _list.Add(_il.Create(OpCodes.Ldarg_0));
                    break;
                case 1:
                    _list.Add(_il.Create(OpCodes.Ldarg_1));
                    break;
                case 2:
                    _list.Add(_il.Create(OpCodes.Ldarg_2));
                    break;
                case 3:
                    _list.Add(_il.Create(OpCodes.Ldarg_3));
                    break;
                default:
                    _list.Add(_il.Create(OpCodes.Ldarg, n));
                    break;
            }
            return this;
        }

        public IlBuilder LoadArgShort(int min, int max)
        {
            for (int i = min; i <= max; i++)
            {
                _list.Add(_il.Create(OpCodes.Ldarg_S, i));
            }
            return this;
        }

        public IlBuilder LoadField(FieldDefinition fi)
        {
            _list.Add(_il.Create(OpCodes.Ldfld, fi));
            return this;
        }

        public IlBuilder SetField(FieldDefinition fi)
        {
            _list.Add(_il.Create(OpCodes.Stfld, fi));
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

        public IlBuilder NewObj(MethodReference ci)
        {
            _list.Add(_il.Create(OpCodes.Newobj, ci));
            return this;
        }

        //public IlBuilder NewObj(Type t)
        //{
        //    return NewObj(GetConstructor(t)));
        //}

        public IlBuilder SetLoc(int n)
        {
            switch (n)
            {
                case 0:
                    _list.Add(_il.Create(OpCodes.Stloc_0));
                    break;
                case 1:
                    _list.Add(_il.Create(OpCodes.Stloc_1));
                    break;
                case 2:
                    _list.Add(_il.Create(OpCodes.Stloc_2));
                    break;
                case 3:
                    _list.Add(_il.Create(OpCodes.Stloc_3));
                    break;
                default:
                    _list.Add(_il.Create(OpCodes.Stloc, n));
                    break;
            }
            return this;
        }

        public IlBuilder LoadLoc(int n)
        {
            switch (n)
            {
                case 0:
                    _list.Add(_il.Create(OpCodes.Ldloc_0));
                    break;
                case 1:
                    _list.Add(_il.Create(OpCodes.Ldloc_1));
                    break;
                case 2:
                    _list.Add(_il.Create(OpCodes.Ldloc_2));
                    break;
                case 3:
                    _list.Add(_il.Create(OpCodes.Ldloc_3));
                    break;
                default:
                    _list.Add(_il.Create(OpCodes.Ldloc, n));
                    break;
            }
            return this;
        }

        public IlBuilder CallVirtual(MethodReference mi)
        {
            _list.Add(_il.Create(OpCodes.Callvirt, mi));
            return this;
        }

        public IlBuilder Call(MethodReference mi)
        {
            _list.Add(_il.Create(OpCodes.Call, mi));
            return this;
        }

        //public IlBuilder Call(ConstructorInfo ci)
        //{
        //    il.Emit(OpCodes.Call, ci));
        //    return this;
        //}

        public IlBuilder Return()
        {
            _list.Add(_il.Create(OpCodes.Ret));
            return this;
        }

        public IlBuilder LoadString(string s)
        {
            _list.Add(_il.Create(OpCodes.Ldstr, s));
            return this;
        }

        public IlBuilder LoadNull()
        {
            _list.Add(_il.Create(OpCodes.Ldnull));
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
        //            t = typeof(bool));
        //        }
        //        else if (t == typeof(uint))
        //        {
        //            t = typeof(int));
        //        }
        //        else if (t == typeof(ulong))
        //        {
        //            t = typeof(long));
        //        }
        //        else if (t == typeof(ushort))
        //        {
        //            t = typeof(short));
        //        }
        //        il.Emit(OpCodes.Unbox_Any, t));
        //    }
        //    else
        //    {
        //        il.Emit(OpCodes.Castclass, t));
        //    }
        //    return this;
        //}

        //private bool ProcessDateAndTime(Type inType, Type unboxType)
        //{
        //    if (inType == typeof(Date))
        //    {
        //        il.Emit(OpCodes.Unbox_Any, unboxType));
        //        il.Emit(OpCodes.Call, DateEx));
        //        return true;
        //    }
        //    if (inType == typeof(Time))
        //    {
        //        il.Emit(OpCodes.Unbox_Any, unboxType));
        //        il.Emit(OpCodes.Call, TimeEx));
        //        return true;
        //    }
        //    return false;
        //}

        public IlBuilder Cast(TypeDefinition t)
        {
            _list.Add(_il.Create(OpCodes.Castclass, t));
            return this;
        }

        public IlBuilder Box(TypeDefinition t)
        {
            if (t.IsValueType)
            {
                _list.Add(_il.Create(OpCodes.Box, t));
            }
            return this;
        }

        public IlBuilder Ceq()
        {
            _list.Add(_il.Create(OpCodes.Ceq));
            return this;
        }

        //public IlBuilder Br_S(Label label)
        //{
        //    il.Emit(OpCodes.Br_S, label));
        //    return this;
        //}

        //public IlBuilder BrTrue_S(Label label)
        //{
        //    il.Emit(OpCodes.Brtrue_S, label));
        //    return this;
        //}

        //public IlBuilder BrFalse_S(Label label)
        //{
        //    il.Emit(OpCodes.Brfalse_S, label));
        //    return this;
        //}

        //public Label DefineLabel()
        //{
        //    return il.DefineLabel());
        //}

        //public IlBuilder MarkLabel(Label label)
        //{
        //    il.MarkLabel(label));
        //    return this;
        //}

        public IlBuilder LoadLocala_S(int index)
        {
            _list.Add(_il.Create(OpCodes.Ldloca_S, index));
            return this;
        }

        //public IlBuilder Bne_Un_S(Label label)
        //{
        //    il.Emit(OpCodes.Bne_Un_S, label));
        //    return this;
        //}

        public IlBuilder Conv_R4()
        {
            _list.Add(_il.Create(OpCodes.Conv_R4));
            return this;
        }

        public IlBuilder Conv_R8()
        {
            _list.Add(_il.Create(OpCodes.Conv_R8));
            return this;
        }

        public IlBuilder ConvFloaty(Type type)
        {
            if (type == typeof(float))
            {
                _list.Add(_il.Create(OpCodes.Conv_R4));
            }
            else if (type == typeof(double))
            {
                _list.Add(_il.Create(OpCodes.Conv_R8));
            }
            return this;
        }
    }
}
