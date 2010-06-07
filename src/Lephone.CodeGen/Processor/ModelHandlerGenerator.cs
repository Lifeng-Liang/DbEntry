using Mono.Cecil;

namespace Lephone.CodeGen.Processor
{
    //public class Test : EmitObjectHandlerBase
    //{
    //    protected override void GetKeyValuesDirect(Dictionary<string, object> dic, object o) { }
    //    protected override void SetValuesForSelectDirect(List<KeyValuePair<string, string>> keys) { }
    //    protected override void SetValuesForInsertDirect(KeyValueCollection values, object obj) { }
    //    protected override void SetValuesForUpdateDirect(KeyValueCollection values, object obj) { }
    //}

    public class ModelHandlerGenerator
    {
        private const TypeAttributes DynamicObjectTypeAttr = TypeAttributes.Class | TypeAttributes.Public;
        private const MethodAttributes MethodAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual; //public hidebysig virtual instance

        private const string MemberPrifix = "$";
        private readonly TypeDefinition _model;
        private readonly KnownTypesHandler _handler;
        private static int _index;
        private readonly TypeDefinition _result;

        public ModelHandlerGenerator(TypeDefinition model, KnownTypesHandler handler)
        {
            this._model = model;
            this._handler = handler;
            _index++;
            _result = new TypeDefinition("$Lephone", MemberPrifix + _index,
                DynamicObjectTypeAttr, _handler.ModelHandlerBaseType);
            _result.CustomAttributes.Add(_handler.GetForType(_model));
        }

        public TypeDefinition Generate()
        {
            GenerateConstructor();
            GenerateCreateInstance();
            GenerateLoadSimpleValuesByIndex();
            GenerateLoadSimpleValuesByName();
            GenerateLoadRelationValuesByIndex();
            GenerateLoadRelationValuesByName();
            //OverrideGetKeyValues(tb, srcType, oi.KeyFields);
            //OverrideGetKeyValue(tb, srcType, oi.KeyFields);
            //OverrideSetValuesForSelect(tb, srcType, oi.Fields);
            //OverrideSetValuesForInsert(tb, srcType, oi.Fields);
            //OverrideSetValuesForUpdate(tb, srcType, oi.Fields);
            return _result;
        }

        private static void GenerateConstructor()
        {
            //NOTE: need this?
        }

        private void GenerateCreateInstance()
        {
            var ctor = _model.GetConstructor();
            var createInstance = new MethodDefinition("CreateInstance", MethodAttr, _handler.ObjectType);
            var processor = new IlBuilder(createInstance.Body.GetILProcessor());
            processor.NewObj(ctor);
            processor.Return();
            processor.Append();
            _result.Methods.Add(createInstance);
        }

        private void GenerateLoadSimpleValuesByIndex()
        {
            var method = new MethodDefinition("LoadSimpleValuesByIndex", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.IDataReaderType));
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateLoadSimpleValuesByName()
        {
            var method = new MethodDefinition("LoadSimpleValuesByName", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.IDataReaderType));
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateLoadRelationValuesByIndex()
        {
            var method = new MethodDefinition("LoadRelationValuesByIndex", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.IDataReaderType));
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateLoadRelationValuesByName()
        {
            var method = new MethodDefinition("LoadRelationValuesByName", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.IDataReaderType));
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }
    }
}
