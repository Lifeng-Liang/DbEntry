using System;
using System.Collections.Generic;
using Leafing.Data.Model;
using Leafing.Data.Model.Member;
using Mono.Cecil;

namespace Leafing.Processor
{
    public class MemberHandlerGenerator
    {
        private const MethodAttributes CtorAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        private const MethodAttributes CtMethodAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;

        private readonly TypeDefinition _model;
        private readonly KnownTypesHandler _handler;
        private readonly ObjectInfo _info;
        private readonly Dictionary<string, PropertyDefinition> _properties;
        private readonly Dictionary<string, FieldDefinition> _fields;

        public MemberHandlerGenerator(Type type, TypeDefinition model, KnownTypesHandler handler)
        {
            this._model = model;
            this._handler = handler;
            this._properties = new Dictionary<string, PropertyDefinition>();
            this._fields = new Dictionary<string, FieldDefinition>();
            GetMembers(model);
            _info = ObjectInfoFactory.Instance.GetInstance(type);
        }

        private void GetMembers(TypeDefinition model)
        {
            foreach (var member in model.Properties)
            {
                if (!_properties.ContainsKey(member.Name))
                {
                    _properties.Add(member.Name, member);
                }
            }
            foreach (var member in model.Fields)
            {
                if (!_fields.ContainsKey(member.Name))
                {
                    _fields.Add(member.Name, member);
                }
            }
            if (model.BaseType != null && model.BaseType != _handler.ObjectType 
                && model.BaseType.FullName != KnownTypesHandler.DbObjectSmartUpdate)
            {
                var bt = model.BaseType.Resolve();
                GetMembers(bt);
            }
        }

        public void Generate(ModuleDefinition module)
        {
            foreach(var member in _info.Members)
            {
                var key = member.MemberInfo.Name;
                var type = TypeFactory.CreateType(_model, _handler.ObjectType, key);
                type.Interfaces.Add(_handler.MemberHandlerInterface);
                try
                {
                    if (member.MemberInfo.IsProperty)
                    {
                        _properties[key].CustomAttributes.Add(_handler.GetModelHandler(type));
                    }
                    else
                    {
                        _fields[key].CustomAttributes.Add(_handler.GetModelHandler(type));
                    }
                }
                catch(Exception ex)
                {
                    throw new ApplicationException(ex.Message + ":" + key);
                }
                GenerateConstructor(type);
                var mt = _handler.Import(member.MemberInfo.MemberType);
                GenerateSetValue(type, member, mt);
                GenerateGetValue(type, member, mt);

                module.Types.Add(type);
            }
        }

        private void GenerateConstructor(TypeDefinition type)
        {
            var method = new MethodDefinition(".ctor", CtorAttr, _handler.VoidType);
            var processor = new IlBuilder(method.Body);
            processor.LoadArg(0);
            processor.Call(_handler.ObjectTypeCtor);
            processor.Return();
            processor.Append();
            type.Methods.Add(method);
        }

        private void GenerateSetValue(TypeDefinition type, MemberHandler member, TypeReference memberType)
        {
            var method = new MethodDefinition("SetValue", CtMethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("obj", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, _handler.ObjectType));
            var processor = new IlBuilder(method.Body);
            processor.LoadArg(1).Cast(_model).LoadArg(2).CastOrUnbox(memberType, _handler);
            processor.SetMember(member, _handler);
            processor.Return();
            processor.Append();
            type.Methods.Add(method);
        }

        private void GenerateGetValue(TypeDefinition type, MemberHandler member, TypeReference memberType)
        {
            var method = new MethodDefinition("GetValue", CtMethodAttr, _handler.ObjectType);
            method.Parameters.Add(new ParameterDefinition("obj", ParameterAttributes.None, _handler.ObjectType));
            var processor = new IlBuilder(method.Body);
            processor.DeclareLocal(_handler.ObjectType);
            processor.LoadArg(1).Cast(_model);
            processor.GetMember(member, _handler);
            if (member.MemberInfo.MemberType.IsValueType)
            {
                processor.Box(memberType);
            }
            processor.SetLoc(0);
            processor.LoadLoc(0);
            processor.Return();
            processor.Append();
            type.Methods.Add(method);
        }
    }
}
