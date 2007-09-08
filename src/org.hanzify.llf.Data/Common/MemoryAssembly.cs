
#region usings

using System;
using System.Reflection;
using System.Reflection.Emit;
using Lephone.Util;

#endregion

namespace Lephone.Data.Common
{
    internal class MemoryAssembly
    {
        public const string DefaultAssemblyName = "DbEntry_MemoryAssembly";
        public static readonly MemoryAssembly Instance = new MemoryAssembly();
        private static int index = 0;

        private AssemblyBuilder InnerAssembly;
        private ModuleBuilder InnerModule;
        private string AssemblyName;

        public MemoryAssembly()
            : this(DefaultAssemblyName) { }

        public MemoryAssembly(string AssemblyName)
        {
            this.AssemblyName = AssemblyName;
            AssemblyName an = new AssemblyName(AssemblyName);
            byte[] bs = ResourceHelper.ReadAll(this.GetType(), "dynamic.snk");
            an.KeyPair = new StrongNameKeyPair(bs);
            //InnerAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
            //    an, AssemblyBuilderAccess.RunAndSave, @"c:\x.dll");
            InnerAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                an, AssemblyBuilderAccess.Run);
            ResetModule();
        }

        public MemoryTypeBuilder DefineType(TypeAttributes attr, Type InheritsFrom, Type[] Interfaces)
        {
            return DefineType(attr, InheritsFrom, Interfaces, new CustomAttributeBuilder[] { });
        }

        public MemoryTypeBuilder DefineType(TypeAttributes attr, Type InheritsFrom, Type[] Interfaces, CustomAttributeBuilder[] attributes)
        {
            string TypeName = "T" + index.ToString();
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
            InnerModule = InnerAssembly.DefineDynamicModule(this.AssemblyName);
        }

        public void Save()
        {
            InnerAssembly.Save(@"c:\x.dll");
        }
    }
}
