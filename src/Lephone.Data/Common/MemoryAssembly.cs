using System;
using System.Reflection;
using System.Reflection.Emit;
using Lephone.Util;

namespace Lephone.Data.Common
{
    public class MemoryAssembly
    {
        public const string DefaultAssemblyName = "DbEntry_MemoryAssembly";
        public static readonly MemoryAssembly Instance = new MemoryAssembly();
        private static int index;

        private readonly AssemblyBuilder InnerAssembly;
        private ModuleBuilder InnerModule;
        private readonly string AssemblyName;

        public MemoryAssembly()
            : this(DefaultAssemblyName) { }

        public MemoryAssembly(string AssemblyName)
        {
            this.AssemblyName = AssemblyName;
            var an = new AssemblyName(AssemblyName);
            byte[] bs = ResourceHelper.ReadAll(GetType(), "dynamic.snk");
            an.KeyPair = new StrongNameKeyPair(bs);
            InnerAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                an, AssemblyBuilderAccess.RunAndSave);
            ResetModule();
        }

        public MemoryTypeBuilder DefineType(TypeAttributes attr, Type InheritsFrom, Type[] Interfaces)
        {
            return DefineType(attr, InheritsFrom, Interfaces, new CustomAttributeBuilder[] { });
        }

        public MemoryTypeBuilder DefineType(TypeAttributes attr, Type InheritsFrom, Type[] Interfaces, CustomAttributeBuilder[] attributes)
        {
            string TypeName = MemoryTypeBuilder.MemberPrifix + index;
            index++;
            return DefineType(TypeName, attr, InheritsFrom, Interfaces, attributes);
        }

        public MemoryTypeBuilder DefineType(string TypeName, TypeAttributes attr, Type InheritsFrom, Type[] Interfaces, CustomAttributeBuilder[] attributes)
        {
            TypeBuilder tb = InnerModule.DefineType(TypeName, attr, InheritsFrom, Interfaces);
            if (attributes != null)
            {
                foreach (CustomAttributeBuilder cb in attributes)
                {
                    tb.SetCustomAttribute(cb);
                }
            }
            return new MemoryTypeBuilder(tb);
        }

        public void ResetModule()
        {
            InnerModule = InnerAssembly.DefineDynamicModule(AssemblyName, @"DbEntry_MemoryAssembly.dll");
        }

        public void Save()
        {
            InnerAssembly.Save(@"DbEntry_MemoryAssembly.dll");
        }
    }
}
