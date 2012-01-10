using Mono.Cecil;

namespace Leafing.Processor
{
    public static class TypeFactory
    {
        private const TypeAttributes ClassTypeAttr = TypeAttributes.Class | TypeAttributes.Public;
        private const string NameSplitter = "$";
        private static int _index;

        public static TypeDefinition CreateType(KnownTypesHandler handler, TypeDefinition model, TypeDefinition interfaceType)
        {
            _index++;
            var result = new TypeDefinition(
                GetNamespace(model),
                GetClassName(model, interfaceType.Name),
                ClassTypeAttr,
                handler.ObjectType);
            result.Interfaces.Add(interfaceType);
            return result;
        }

        public static TypeDefinition CreateType(TypeDefinition model, TypeReference baseType, string middleName = "")
        {
            _index++;
            var result = new TypeDefinition(
                GetNamespace(model),
                GetClassName(model, middleName),
                ClassTypeAttr,
                baseType);
            return result;
        }

        public static PropertyDefinition CreateProperty(string name, MethodAttributes attr, TypeReference type, KnownTypesHandler handler)
        {
            var pd = new PropertyDefinition(name, PropertyAttributes.None, type);
            pd.GetMethod = new MethodDefinition("get_" + name, attr, type);
            pd.SetMethod = new MethodDefinition("set_" + name, attr, handler.VoidType);
            pd.SetMethod.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, type));
            return pd;
        }

        private static string GetNamespace(TypeDefinition model)
        {
            return NameSplitter + model.Namespace;
        }

        private static string GetClassName(TypeDefinition model, string middleName)
        {
            return NameSplitter + model.Name + NameSplitter + middleName + NameSplitter + _index;
        }
    }
}
