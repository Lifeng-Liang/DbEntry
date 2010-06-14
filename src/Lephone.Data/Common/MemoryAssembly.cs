using System;
using System.Reflection;
using System.Reflection.Emit;
using Lephone.Core;

namespace Lephone.Data.Common
{
    public class MemoryAssembly
    {
        public const string DefaultAssemblyName = "DbEntry_MemoryAssembly";
        public static readonly MemoryAssembly Instance = new MemoryAssembly();
        private static int _index;

        private readonly AssemblyBuilder _innerAssembly;
        private ModuleBuilder _innerModule;
        private readonly string _assemblyName;

        public MemoryAssembly()
            : this(DefaultAssemblyName) { }

        public MemoryAssembly(string assemblyName)
        {
            this._assemblyName = assemblyName;
            var an = new AssemblyName(assemblyName);
            byte[] bs = ResourceHelper.ReadAll(GetType(), "dynamic.snk");
            an.KeyPair = new StrongNameKeyPair(bs);
            _innerAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                an, AssemblyBuilderAccess.RunAndSave);
            ResetModule();
        }

        public MemoryTypeBuilder DefineType(TypeAttributes attr, Type inheritsFrom, Type[] interfaces)
        {
            return DefineType(attr, inheritsFrom, interfaces, new CustomAttributeBuilder[] { });
        }

        public MemoryTypeBuilder DefineType(TypeAttributes attr, Type inheritsFrom, Type[] interfaces, CustomAttributeBuilder[] attributes)
        {
            string typeName = MemoryTypeBuilder.MemberPrifix + _index;
            _index++;
            return DefineType(typeName, attr, inheritsFrom, interfaces, attributes);
        }

        public MemoryTypeBuilder DefineType(string typeName, TypeAttributes attr, Type inheritsFrom, Type[] interfaces, CustomAttributeBuilder[] attributes)
        {
            TypeBuilder tb = _innerModule.DefineType(typeName, attr, inheritsFrom, interfaces);
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
            _innerModule = _innerAssembly.DefineDynamicModule(_assemblyName, @"DbEntry_MemoryAssembly.dll");
        }

        public void Save()
        {
            _innerAssembly.Save(@"DbEntry_MemoryAssembly.dll");
        }
    }
}
