using Lephone.Data;
using Lephone.Data.Common;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Lephone.Processor
{
    public class ComposedOfClassGenerator
    {
        private const MethodAttributes CtorAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        private const MethodAttributes PropAttr = MethodAttributes.Assem | MethodAttributes.HideBySig | MethodAttributes.SpecialName;

        private readonly TypeDefinition _model;
        private readonly TypeDefinition _composedOfType;
        private readonly KnownTypesHandler _handler;
        private readonly TypeDefinition _result;
        private int _startIndex;

        public ComposedOfClassGenerator(TypeDefinition model, TypeDefinition composedOfType, KnownTypesHandler handler)
        {
            if (!composedOfType.IsInterface)
            {
                throw new DataException("ComposedOf type must be interface.");
            }
            if(!composedOfType.HasProperties)
            {
                throw new DataException("ComposedOf type must has properties.");
            }

            this._model = model;
            this._composedOfType = composedOfType;
            this._handler = handler;
            _result = TypeFactory.CreateType(handler, model, composedOfType);
        }

        public TypeDefinition Generate()
        {
            _startIndex = _model.Properties.Count;
            foreach(var property in _composedOfType.Properties)
            {
                var iname = GetInterfaceName(_composedOfType.Name);
                var name = "$" + iname + "$" + property.Name;
                var dbn = iname + property.Name;
                var pd = TypeFactory.CreateProperty(name, PropAttr, property.PropertyType, _handler);
                foreach (var attribute in property.CustomAttributes)
                {
                    pd.CustomAttributes.Add(attribute);
                }
                if(pd.GetCustomAttribute(KnownTypesHandler.DbColumnAttribute) == null)
                {
                    pd.CustomAttributes.Add(_handler.GetDbColumn(dbn));
                }
                _model.Properties.Add(pd);
                _model.Methods.Add(pd.GetMethod);
                _model.Methods.Add(pd.SetMethod);

                var pi = new PropertyInformation { PropertyDefinition = pd, FieldType = FieldType.Normal };
                var pp = new PropertyProcessor(pi, _model, _handler);
                pp.Process();
            }
            GenerateClass();
            return _result;
        }

        private void GenerateClass()
        {
            _result.Fields.Add(new FieldDefinition("_owner", FieldAttributes.Private | FieldAttributes.InitOnly, _model));
            GenerateConstructor();
            foreach(var property in _composedOfType.Properties)
            {
                var p = GenerateProperty(property);
                _result.Properties.Add(p);
                _result.Methods.Add(p.GetMethod);
                _result.Methods.Add(p.SetMethod);
            }
            _model.Module.Types.Add(_result);
        }

        private void GenerateConstructor()
        {
            var method = new MethodDefinition(".ctor", CtorAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("owner", ParameterAttributes.None, _model));
            var processor = new IlBuilder(method.Body);
            processor.LoadArg(0);
            processor.Call(_handler.ObjectTypeCtor);
            processor.LoadArg(0);
            processor.LoadArg(1);
            processor.SetField(_result.Fields[0]);
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private PropertyDefinition GenerateProperty(PropertyDefinition property)
        {
            const MethodAttributes attr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
                                          MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;

            var p = TypeFactory.CreateProperty(property.Name, attr, property.PropertyType, _handler);

            var getter = new IlBuilder(p.GetMethod.Body);
            getter.DeclareLocal(property.PropertyType);
            getter.LoadArg(0);
            getter.LoadField(_result.Fields[0]);
            getter.Call(_model.Properties[_startIndex].GetMethod);
            getter.SetLoc(0);
            var i = getter.Processor.Create(OpCodes.Ldloc_0);
            getter.Br_S(i);
            getter.Instructions.Add(i);
            getter.Return();
            getter.Append();

            var setter = new IlBuilder(p.SetMethod.Body);
            setter.LoadArg(0);
            setter.LoadField(_result.Fields[0]);
            setter.LoadArg(1);
            setter.Call(_model.Properties[_startIndex].SetMethod);
            setter.Return();
            setter.Append();

            _startIndex++;
            return p;
        }

        private static string GetInterfaceName(string name)
        {
            if(name.Length > 1)
            {
                if(name[0] == 'I' && name[1] >= 'A' && name[1] <= 'Z')
                {
                    return name.Substring(1);
                }
            }
            return name;
        }
    }
}
