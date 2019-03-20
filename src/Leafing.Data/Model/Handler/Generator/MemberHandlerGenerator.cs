using System;
using System.Collections.Generic;
using Leafing.Data.Model;
using Leafing.Data.Model.Member;
using System.Reflection;
using Leafing.Data.Definition;
using System.Reflection.Emit;

namespace Leafing.Data.Model.Handler.Generator {
    public class TinyMember {
        public FieldInfo Field;
        public PropertyInfo Property;

        public bool IsProperty {
            get {
                return Property != null;
            }
        }

        public TinyMember(FieldInfo info) {
            this.Field = info;
        }

        public TinyMember(PropertyInfo info) {
            this.Property = info;
        }
    }

    public class MemberHandlerGenerator {
        private const MethodAttributes CtorAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        private const MethodAttributes CtMethodAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;
        private const TypeAttributes DynamicObjectTypeAttr = TypeAttributes.Class | TypeAttributes.Public;

        private static readonly Type objectType = typeof(object);
        private static readonly Type memberHandlerInterface = typeof(IMemberHandler);

        public MemberHandlerGenerator() {
        }

        public static Type Generate(Type model, FieldInfo info) {
            var type = MemoryAssembly.Instance.DefineType(DynamicObjectTypeAttr,
                objectType, new[] { memberHandlerInterface });
            GenerateConstructor(type);
            GenerateSetValue(model, type, new TinyMember(info), info.FieldType);
            GenerateGetValue(model, type, new TinyMember(info), info.FieldType);
            return type.CreateType();
        }

        public static Type Generate(Type model, PropertyInfo info) {
            var type = MemoryAssembly.Instance.DefineType(DynamicObjectTypeAttr,
                objectType, new[] { memberHandlerInterface });
            GenerateConstructor(type);
            GenerateSetValue(model, type, new TinyMember(info), info.PropertyType);
            GenerateGetValue(model, type, new TinyMember(info), info.PropertyType);
            return type.CreateType();
        }

        private static void GenerateConstructor(TypeBuilder type) {
            type.DefineDefaultConstructor(MethodAttributes.Public);
        }

        private static void GenerateSetValue(Type model, TypeBuilder builder, TinyMember info, Type memberType) {
            var method = builder.DefineMethod("SetValue", CtMethodAttr,
                null, new Type[] { objectType, objectType });
            var processor = new ILBuilder(method.GetILGenerator());

            processor.LoadArg(1).Cast(model).LoadArg(2).CastOrUnbox(memberType);
            processor.SetMember(info);
            processor.Return();
        }

        private static void GenerateGetValue(Type model, TypeBuilder builder, TinyMember info, Type memberType) {
            var method = builder.DefineMethod("GetValue", CtMethodAttr,
                objectType, new Type[] { objectType });
            var processor = new ILBuilder(method.GetILGenerator());

            processor.DeclareLocal(objectType);
            processor.LoadArg(1).Cast(model);
            processor.GetMember(info);
            if (memberType.IsValueType) {
                processor.Box(memberType);
            }
            processor.SetLoc(0);
            processor.LoadLoc(0);
            processor.Return();
        }
    }
}