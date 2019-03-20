using System;
using System.Reflection;
using System.Reflection.Emit;
using Leafing.Data.Model.Member;
using Leafing.Data.Model.Member.Adapter;

namespace Leafing.Data.Model.Handler.Generator {
    public class ILBuilder {
        private static readonly Type[] EmptyTypes = new Type[] { };
        private static readonly MethodInfo DateEx = typeof(Date).GetMethod("op_Explicit", new[] { typeof(DateTime) });
        private static readonly MethodInfo TimeEx = typeof(Time).GetMethod("op_Explicit", new[] { typeof(DateTime) });

        public readonly ILGenerator il;

        public ILBuilder(ILGenerator il) {
            this.il = il;
        }

        public ILBuilder DeclareLocal(Type t) {
            il.DeclareLocal(t);
            return this;
        }

        public ILBuilder Nop() {
            il.Emit(OpCodes.Nop);
            return this;
        }

        public ILBuilder LoadToken(Type t) {
            il.Emit(OpCodes.Ldtoken, t);
            return this;
        }

        public ILBuilder LoadInt(int n) {
            switch (n) {
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

        public ILBuilder LoadArg(int n) {
            switch (n) {
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

        public ILBuilder LoadArgShort(int min, int max) {
            for (int i = min; i <= max; i++) {
                il.Emit(OpCodes.Ldarg_S, i);
            }
            return this;
        }

        public ILBuilder LoadField(FieldInfo fi) {
            il.Emit(OpCodes.Ldfld, fi);
            return this;
        }

        public ILBuilder SetField(FieldInfo fi) {
            il.Emit(OpCodes.Stfld, fi);
            return this;
        }

        private static ConstructorInfo GetConstructor(Type sourceType) {
            Type t = sourceType;
            ConstructorInfo ret;
            while ((ret = t.GetConstructor(EmptyTypes)) == null) {
                t = t.BaseType;
            }
            return ret;
        }

        //TODO: why left this function?
        //private static ConstructorInfo[] GetConstructorList(Type SourceType)
        //{
        //    Type t = SourceType;
        //    ConstructorInfo[] ret;
        //    while ((ret = t.GetConstructors()).Length == 0)
        //    {
        //        t = t.BaseType;
        //    }
        //    return ret;
        //}

        public ILBuilder NewObj(ConstructorInfo ci) {
            il.Emit(OpCodes.Newobj, ci);
            return this;
        }

        public ILBuilder NewObj(Type t) {
            return NewObj(GetConstructor(t));
        }

        public ILBuilder SetLoc(int n) {
            switch (n) {
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
            return this;
        }

        public ILBuilder LoadLoc(int n) {
            switch (n) {
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

        public ILBuilder CallVirtual(MethodInfo mi) {
            il.Emit(OpCodes.Callvirt, mi);
            return this;
        }

        public ILBuilder Call(MethodInfo mi) {
            il.Emit(OpCodes.Call, mi);
            return this;
        }

        public ILBuilder Call(ConstructorInfo ci) {
            il.Emit(OpCodes.Call, ci);
            return this;
        }

        public ILBuilder Return() {
            il.Emit(OpCodes.Ret);
            return this;
        }

        public ILBuilder LoadString(string s) {
            il.Emit(OpCodes.Ldstr, s);
            return this;
        }

        public ILBuilder LoadNull() {
            il.Emit(OpCodes.Ldnull);
            return this;
        }

        public ILBuilder CastOrUnbox(Type t) {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                Type inType = t.GetGenericArguments()[0];
                if (ProcessDateAndTime(inType, typeof(Date?), typeof(Time?))) {
                    return this;
                }
            }
            if (t.IsValueType) {
                if (ProcessDateAndTime(t, typeof(Date), typeof(Time))) {
                    return this;
                }
                if (t == typeof(bool)) {
                    t = typeof(bool);
                } else if (t == typeof(uint)) {
                    t = typeof(int);
                } else if (t == typeof(ulong)) {
                    t = typeof(long);
                } else if (t == typeof(ushort)) {
                    t = typeof(short);
                }
                il.Emit(OpCodes.Unbox_Any, t);
            } else {
                il.Emit(OpCodes.Castclass, t);
            }
            return this;
        }

        private bool ProcessDateAndTime(Type inType, Type unboxDateType, Type unboxTimeType) {
            if (inType == typeof(Date)) {
                il.Emit(OpCodes.Unbox_Any, unboxDateType);
                il.Emit(OpCodes.Call, DateEx);
                return true;
            }
            if (inType == typeof(Time)) {
                il.Emit(OpCodes.Unbox_Any, unboxTimeType);
                il.Emit(OpCodes.Call, TimeEx);
                return true;
            }
            return false;
        }

        public ILBuilder Cast(Type t) {
            il.Emit(OpCodes.Castclass, t);
            return this;
        }

        public ILBuilder Box(Type t) {
            if (t.IsValueType) {
                il.Emit(OpCodes.Box, t);
            }
            return this;
        }

        public ILBuilder Unbox(Type t) {
            if (t.IsValueType) {
                il.Emit(OpCodes.Unbox_Any, t);
            }
            return this;
        }

        public ILBuilder Ceq() {
            il.Emit(OpCodes.Ceq);
            return this;
        }

        public ILBuilder Br_S(Label label) {
            il.Emit(OpCodes.Br_S, label);
            return this;
        }

        public ILBuilder BrTrue_S(Label label) {
            il.Emit(OpCodes.Brtrue_S, label);
            return this;
        }

        public ILBuilder BrFalse_S(Label label) {
            il.Emit(OpCodes.Brfalse_S, label);
            return this;
        }

        public Label DefineLabel() {
            return il.DefineLabel();
        }

        public ILBuilder MarkLabel(Label label) {
            il.MarkLabel(label);
            return this;
        }

        public ILBuilder LoadLocala_S(int index) {
            il.Emit(OpCodes.Ldloca_S, index);
            return this;
        }

        public ILBuilder Bne_Un_S(Label label) {
            il.Emit(OpCodes.Bne_Un_S, label);
            return this;
        }

        public ILBuilder Conv_R4() {
            il.Emit(OpCodes.Conv_R4);
            return this;
        }

        public ILBuilder Conv_R8() {
            il.Emit(OpCodes.Conv_R8);
            return this;
        }

        public ILBuilder ConvFloaty(Type type) {
            if (type == typeof(float)) {
                il.Emit(OpCodes.Conv_R4);
            } else if (type == typeof(double)) {
                il.Emit(OpCodes.Conv_R8);
            }
            return this;
        }

        public ILBuilder SetMember(MemberHandler mm) {
            if (mm.MemberInfo.IsProperty) {
                var method = ((PropertyInfo)mm.MemberInfo.GetMemberInfo()).GetSetMethod(true);
                return CallVirtual(method);
            }
            return SetField((FieldInfo)mm.MemberInfo.GetMemberInfo());
        }

        public ILBuilder GetMember(MemberHandler mm) {
            if (mm.MemberInfo.IsProperty) {
                var method = ((PropertyInfo)mm.MemberInfo.GetMemberInfo()).GetGetMethod(true);
                return CallVirtual(method);
            }
            return LoadField((FieldInfo)mm.MemberInfo.GetMemberInfo());
        }

        public ILBuilder SetMember(TinyMember mm) {
            if (mm.IsProperty) {
                var method = mm.Property.GetSetMethod(true);
                return CallVirtual(method);
            }
            return SetField(mm.Field);
        }

        public ILBuilder GetMember(TinyMember mm) {
            if (mm.IsProperty) {
                var method = (mm.Property).GetGetMethod(true);
                return CallVirtual(method);
            }
            return LoadField((FieldInfo)mm.Field);
        }
    }
}