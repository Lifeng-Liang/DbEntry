
#region usings

using System;
using System.Reflection;
using System.Reflection.Emit;

#endregion

namespace org.hanzify.llf.Data.Common
{
    public class MemoryAssembly
    {
        public const string DefaultAssemblyName = "MemoryAssembly";
        public static readonly MemoryAssembly Instance = new MemoryAssembly();

        private AssemblyBuilder InnerAssembly;
        private ModuleBuilder InnerModule;
        private string AssemblyName;

        public MemoryAssembly()
            : this(DefaultAssemblyName) { }

        public MemoryAssembly(string AssemblyName)
        {
            this.AssemblyName = AssemblyName;
            InnerAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(AssemblyName), AssemblyBuilderAccess.Run);
            ResetModule();
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
    }
}
