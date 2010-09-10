using Lephone.Data;
using Mono.Cecil;

namespace Lephone.Processor
{
    public class ComposedOfClassGenerator
    {
        private readonly TypeDefinition _model;
        private readonly TypeDefinition _composedOfType;
        private readonly KnownTypesHandler _handler;
        private readonly TypeDefinition _result;

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
            _result = TypeFactory.CreateType(model, composedOfType);
            _model.CustomAttributes.Add(_handler.GetModelHandler(_result));
        }

        public void Generate()
        {
            foreach(var property in _composedOfType.Properties)
            {
                var name = "$" + _composedOfType.Name + "$" + property.Name;
                var dbn = _composedOfType.Name + property.Name;
                var field = new FieldDefinition(name, FieldAttributes.FamORAssem, property.PropertyType);
                field.CustomAttributes.Add(_handler.GetDbColumn(dbn));
                _model.Fields.Add(field);
            }
        }
    }
}
