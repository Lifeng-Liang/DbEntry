using System.Collections.Generic;
using Lephone.Data.Common;
using Mono.Cecil;

namespace Lephone.CodeGen.Processor
{
    public class ModelInformation
    {
        public readonly List<ModelMember> KeyMembers = new List<ModelMember>();
        public readonly List<ModelMember> SimpleMembers = new List<ModelMember>();
        public readonly List<ModelMember> RelationMembers = new List<ModelMember>();

        private readonly Dictionary<string, string> _genericTypes = new Dictionary<string, string>();
        private readonly KnownTypesHandler _handler;

        public ModelInformation(TypeDefinition model, KnownTypesHandler handler)
        {
            _handler = handler;
            SearchMember(model);
        }

        private void SearchMember(TypeDefinition model)
        {
            if (model.FullName == KnownTypesHandler.Object)
            {
                return;
            }
            var baseType = ProcessGeneric(model.BaseType);
            foreach (var field in model.Fields)
            {
                ProcessMember(field);
            }
            foreach (var property in model.Properties)
            {
                ProcessMember(property);
            }
            SearchMember(baseType);
        }

        private TypeDefinition ProcessGeneric(TypeReference baseType)
        {
            var result = baseType.Resolve();
            if(result.HasGenericParameters)
            {
                var diff = baseType.FullName.Substring(result.FullName.Length + 1);
                diff = diff.Substring(0, diff.Length - 1);
                var ss = diff.Split(',');
                for (int i = 0; i < result.GenericParameters.Count; i++)
                {
                    var parameter = result.GenericParameters[i];
                    if (!_genericTypes.ContainsKey(parameter.Name))
                    {
                        _genericTypes.Add(parameter.Name, ss[i]);
                    }
                }
            }
            return result;
        }

        private void ProcessMember(FieldDefinition field)
        {
            if((field.IsPublic || field.IsFamily) && !field.IsExclude())
            {
                AddMember(field, field.FieldType);
            }
        }

        private void ProcessMember(PropertyDefinition property)
        {
            if ((property.SetMethod != null && property.GetMethod != null) 
                && (property.SetMethod.IsPublic || property.SetMethod.IsFamily)
                && (property.GetMethod.IsPublic || property.GetMethod.IsFamily) 
                && !property.IsExclude())
            {
                AddMember(property, property.GetMethod.ReturnType);
            }
        }

        private void AddMember(IMemberDefinition member, TypeReference mt)
        {
            TypeReference type = mt.IsGenericParameter 
                ? _handler.Import(_genericTypes[mt.Name]) 
                : mt;
            var mm = new ModelMember(member, type);
            var fieldType = KnownTypesHandler.GetFieldType(member);
            if (mm.IsDbKey)
            {
                KeyMembers.Add(mm);
            }
            else if (fieldType == FieldType.Normal)
            {
                SimpleMembers.Add(mm);
            }
            else
            {
                RelationMembers.Add(mm);
            }
        }
    }
}
