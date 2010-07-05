using System;
using System.Collections.Generic;
using System.Reflection;
using Lephone.Data.Common;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MethodBody = Mono.Cecil.Cil.MethodBody;

namespace Lephone.Processor
{
    public class IlBuilder
    {
        private readonly MethodBody _body;
        private readonly ILProcessor _il;

        private readonly List<Instruction> _list;

        public IlBuilder(MethodBody body)
        {
            _body = body;
            this._il = body.GetILProcessor();
            _list = new List<Instruction>();
        }

        public List<Instruction> Instructions
        {
            get
            {
                return _list;
            }
        }

        public ILProcessor Processor
        {
            get
            {
                return _il;
            }
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

        public VariableDefinition DeclareLocal(TypeReference t)
        {
            var variable = new VariableDefinition(t);
            _body.Variables.Add(variable);
            _body.InitLocals = true;
            return variable;
        }

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

        public IlBuilder LoadField(FieldReference fi)
        {
            if(fi.DeclaringType.HasGenericParameters)
            {
                var test = TypeHelper.MakeGenericType(fi.DeclaringType, fi.DeclaringType.GenericParameters.ToArray());
                fi = new FieldReference(fi.Name, fi.FieldType) {DeclaringType = test};
            }
            _list.Add(_il.Create(OpCodes.Ldfld, fi));
            return this;
        }

        public IlBuilder SetField(FieldReference fi)
        {
            if (fi.DeclaringType.HasGenericParameters)
            {
                var test = TypeHelper.MakeGenericType(fi.DeclaringType, fi.DeclaringType.GenericParameters.ToArray());
                fi = new FieldReference(fi.Name, fi.FieldType) { DeclaringType = test };
            }
            _list.Add(_il.Create(OpCodes.Stfld, fi));
            return this;
        }

        public IlBuilder SetMember(MemberHandler mm, KnownTypesHandler handler)
        {
            if(mm.MemberInfo.IsProperty)
            {
                var method = ((PropertyInfo)mm.MemberInfo.GetMemberInfo()).GetSetMethod();
                return CallVirtual(handler.Import(method));
            }
            return SetField(handler.Import((FieldInfo)mm.MemberInfo.GetMemberInfo()));
        }

        public IlBuilder GetField(PropertyDefinition pi)
        {
            return CallVirtual(pi.SetMethod);
        }

        public IlBuilder GetMember(MemberHandler mm, KnownTypesHandler handler)
        {
            if (mm.MemberInfo.IsProperty)
            {
                var method = ((PropertyInfo)mm.MemberInfo.GetMemberInfo()).GetGetMethod();
                return CallVirtual(handler.Import(method));
            }
            return LoadField(handler.Import((FieldInfo)mm.MemberInfo.GetMemberInfo()));
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

        public IlBuilder CastOrUnbox(TypeReference t, KnownTypesHandler handler)
        {
            //TODO: refactor the types to KnownTypesHandler
            if (t.IsGenericInstance && t.Name == "Nullable`1")
            {
                var inType = ((GenericInstanceType)t).GenericArguments[0];
                if (ProcessDateAndTime(inType, handler.Import(typeof(Date?)), handler.Import(typeof(Time?))))
                {
                    return this;
                }
            }
            if (t.IsValueType)
            {
                if (ProcessDateAndTime(t, handler.Import(typeof(Date)), handler.Import(typeof(Time))))
                {
                    return this;
                }
                if (t.FullName == typeof(bool).FullName)
                {
                    t = handler.Import(typeof(bool));
                }
                else if (t.FullName == typeof(uint).FullName)
                {
                    t = handler.Import(typeof(int));
                }
                else if (t.FullName == typeof(ulong).FullName)
                {
                    t = handler.Import(typeof(long));
                }
                else if (t.FullName == typeof(ushort).FullName)
                {
                    t = handler.Import(typeof(short));
                }
                _list.Add(_il.Create(OpCodes.Unbox_Any, t));
            }
            else
            {
                _list.Add(_il.Create(OpCodes.Castclass, t));
            }
            return this;
        }

        private bool ProcessDateAndTime(TypeReference inType, TypeReference unboxDateType, TypeReference unboxTimeType)
        {
            //TODO: refactor the types to KnownTypesHandler
            if (inType.FullName == typeof(Date).FullName)
            {
                _list.Add(_il.Create(OpCodes.Unbox_Any, unboxDateType));
                return true;
            }
            if (inType.FullName == typeof(Time).FullName)
            {
                _list.Add(_il.Create(OpCodes.Unbox_Any, unboxTimeType));
                return true;
            }
            return false;
        }

        public IlBuilder Cast(TypeReference t)
        {
            _list.Add(_il.Create(OpCodes.Castclass, t));
            return this;
        }

        public IlBuilder Box(TypeReference t)
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

        public IlBuilder Br_S(Instruction instruction)
        {
            _list.Add(_il.Create(OpCodes.Br_S, instruction));
            return this;
        }

        public IlBuilder BrTrue_S(Instruction instruction)
        {
            _list.Add(_il.Create(OpCodes.Brtrue_S, instruction));
            return this;
        }

        public IlBuilder BrFalse_S(Instruction instruction)
        {
            _list.Add(_il.Create(OpCodes.Brfalse_S, instruction));
            return this;
        }

        //public Label DefineLabel()
        //{
        //    return il.DefineLabel());
        //}

        //public IlBuilder MarkLabel(Label label)
        //{
        //    il.MarkLabel(label));
        //    return this;
        //}

        public IlBuilder LoadLocala_S(VariableDefinition variable)
        {
            _list.Add(_il.Create(OpCodes.Ldloca_S, variable));
            return this;
        }

        public IlBuilder Bne_Un_S(Instruction instruction)
        {
            _list.Add(_il.Create(OpCodes.Bne_Un_S, instruction));
            return this;
        }

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

        public IlBuilder ConvFloaty(string typeName)
        {
            if (typeName == typeof(float).FullName)
            {
                _list.Add(_il.Create(OpCodes.Conv_R4));
            }
            else if (typeName == typeof(double).FullName)
            {
                _list.Add(_il.Create(OpCodes.Conv_R8));
            }
            return this;
        }
    }
}
