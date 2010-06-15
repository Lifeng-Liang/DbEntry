using System;
using System.Collections.Generic;
using Lephone.Data.Common;
using Mono.Cecil;

namespace Lephone.CodeGen.Processor
{
    public class ModelInformation
    {
        private static readonly Dictionary<string, ModelInformation> Jar = new Dictionary<string, ModelInformation>(); 

        public static ModelInformation GetInstance(TypeDefinition model, KnownTypesHandler handler)
        {
            if(Jar.ContainsKey(model.FullName))
            {
                return Jar[model.FullName];
            }
            var result = new ModelInformation(model, handler);
            Jar[model.FullName] = result;
            return result;
        }

        public readonly List<ModelMember> KeyMembers = new List<ModelMember>();
        public readonly List<ModelMember> SimpleMembers = new List<ModelMember>();
        public readonly List<ModelMember> RelationMembers = new List<ModelMember>();
        public readonly List<ModelMember> Members = new List<ModelMember>();

        private readonly Dictionary<string, string> _genericTypes = new Dictionary<string, string>();
        private readonly KnownTypesHandler _handler;

        private ModelInformation(TypeDefinition model, KnownTypesHandler handler)
        {
            _handler = handler;
            SearchMember(model);
            SimpleMembers.InsertRange(0, KeyMembers);
            Members.AddRange(SimpleMembers);
            Members.AddRange(RelationMembers);
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

        //for Cecil's bug
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
            if((field.IsPublic || field.IsFamilyAndAssembly) && !field.IsExclude())
            {
                var fieldType = KnownTypesHandler.GetFieldType(field);
                AddMember(field, field.FieldType, fieldType);
            }
        }

        private void ProcessMember(PropertyDefinition property)
        {
            if (property.SetMethod != null && property.GetMethod != null
                && property.SetMethod.IsPublic && property.GetMethod.IsPublic
                && !property.IsHandlerExclude())
            {
                AddMember(property, property.GetMethod.ReturnType, FieldType.Normal);
            }
        }

        private void AddMember(IMemberDefinition member, TypeReference mt, FieldType fieldType)
        {
            TypeReference type = mt.IsGenericParameter 
                ? _handler.Import(_genericTypes[mt.Name]) 
                : mt;
            ModelMember mm;
            if (member.DeclaringType.HasGenericParameters)
            {
                var dt = new GenericInstanceType(member.DeclaringType);
                foreach(var parameter in member.DeclaringType.GenericParameters)
                {
                    dt.GenericArguments.Add(_handler.Import(_genericTypes[parameter.Name]));
                }
                mm = new ModelMember(member, type, fieldType, dt);
            }
            else
            {
                mm = new ModelMember(member, type, fieldType, null);
            }
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

        public ModelMember GetBelongsTo(TypeReference type)
        {
            foreach (var member in RelationMembers)
            {
                if(member.IsBelongsTo && ((GenericInstanceType)member.MemberType).GenericArguments[0].FullName == type.FullName)
                {
                    return member;
                }
            }
            throw new ApplicationException();
        }

        public ModelMember GetHasAndBelongsToMany(TypeReference type)
        {
            foreach (var member in RelationMembers)
            {
                if (member.IsHasAndBelongsToMany && member.MemberType.GenericParameters[0].FullName == type.FullName)
                {
                    return member;
                }
            }
            throw new ApplicationException();
        }
    }
}
