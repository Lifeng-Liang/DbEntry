using Mono.Cecil;

namespace Lephone.Processor
{
    public static class TypeFactory
    {
        private const TypeAttributes ClassTypeAttr = TypeAttributes.Class | TypeAttributes.Public;
        private const string NameSplitter = "$";
        private static int _index;

        public static TypeDefinition CreateType(TypeDefinition model, TypeDefinition interfaceType)
        {
            _index++;
            var result = new TypeDefinition(
                GetNamespace(model),
                GetClassName(model, interfaceType.Name),
                ClassTypeAttr);
            result.Interfaces.Add(interfaceType);
            return result;
        }

        public static TypeDefinition CreateType(TypeDefinition model, TypeReference baseType)
        {
            _index++;
            var result = new TypeDefinition(
                GetNamespace(model),
                GetClassName(model, ""),
                ClassTypeAttr,
                baseType);
            return result;
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
