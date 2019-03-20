using Leafing.Core;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Leafing.Data.Model.Handler.Generator {
    public class MemoryAssembly {
        public const string DefaultAssemblyName = "DbEntry_MemoryAssembly";
        public static readonly MemoryAssembly Instance = new MemoryAssembly();
        private static int index;

        private readonly AssemblyBuilder InnerAssembly;
        private ModuleBuilder InnerModule;
        private readonly string AssemblyName;

        public MemoryAssembly()
            : this(DefaultAssemblyName) { }

        public MemoryAssembly(string AssemblyName) {
            this.AssemblyName = AssemblyName;
            var an = new AssemblyName(AssemblyName);
            byte[] snk = ResourceHelper.ReadAll(GetType(), "Model.Handler.Generator.dynamic.snk");
            an.KeyPair = new StrongNameKeyPair(snk);
            InnerAssembly = AssemblyBuilder.DefineDynamicAssembly(
                an, AssemblyBuilderAccess.Run);
            ResetModule();
        }

        public static string GetTypeName() {
            string typeName = "$" + index;
            index++;
            return typeName;
        }

        public TypeBuilder DefineType(TypeAttributes attr, Type InheritsFrom, Type[] Interfaces) {
            return DefineType(attr, InheritsFrom, Interfaces, new CustomAttributeBuilder[] { });
        }

        public TypeBuilder DefineType(TypeAttributes attr, Type InheritsFrom, Type[] Interfaces, CustomAttributeBuilder[] attributes) {
            return DefineType(GetTypeName(), attr, InheritsFrom, Interfaces, attributes);
        }

        public TypeBuilder DefineType(string TypeName, TypeAttributes attr, Type InheritsFrom, Type[] Interfaces, CustomAttributeBuilder[] attributes) {
            TypeBuilder tb = InnerModule.DefineType(TypeName, attr, InheritsFrom, Interfaces);
            if (attributes != null) {
                foreach (CustomAttributeBuilder cb in attributes) {
                    tb.SetCustomAttribute(cb);
                }
            }
            return tb;
        }

        public void ResetModule() {
            InnerModule = InnerAssembly.DefineDynamicModule(AssemblyName);
        }
    }
}