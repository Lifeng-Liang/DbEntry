using Mono.Cecil;

namespace Lephone.CodeGen.Processor
{
    public class ModelHandlerGenerator
    {
        private const TypeAttributes ClassTypeAttr = TypeAttributes.Class | TypeAttributes.Public;
        private const MethodAttributes MethodAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual; //public hidebysig virtual instance
        private const MethodAttributes CtorAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

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
                ClassTypeAttr, _handler.ModelHandlerBaseType);
            _model.CustomAttributes.Add(_handler.GetModelHandler(_result));
        }

        public TypeDefinition Generate()
        {
            GenerateConstructor();
            GenerateCreateInstance();
            GenerateLoadSimpleValuesByIndex();
            GenerateLoadSimpleValuesByName();
            GenerateLoadRelationValuesByIndex();
            GenerateLoadRelationValuesByName();
            GenerateGetKeyValueDirect();
            GenerateGetKeyValuesDirect();
            GenerateSetValuesForSelectDirect();
            GenerateSetValuesForInsertDirect();
            GenerateSetValuesForUpdateDirect();
            return _result;
        }

        private void GenerateConstructor()
        {
            var method = new MethodDefinition(".ctor", CtorAttr, _handler.VoidType);
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.LoadArg(0);
            processor.Call(_handler.ModelHandlerBaseTypeCtor);
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateCreateInstance()
        {
            var ctor = _model.GetConstructor();
            var method = new MethodDefinition("CreateInstance", MethodAttr, _handler.ObjectType);
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.NewObj(ctor);
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateLoadSimpleValuesByIndex()
        {
            //TODO: implements this
            var method = new MethodDefinition("LoadSimpleValuesByIndex", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.DataReaderInterface));
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateLoadSimpleValuesByName()
        {
            //TODO: implements this
            var method = new MethodDefinition("LoadSimpleValuesByName", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.DataReaderInterface));
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateLoadRelationValuesByIndex()
        {
            //TODO: implements this
            var method = new MethodDefinition("LoadRelationValuesByIndex", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.DataReaderInterface));
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateLoadRelationValuesByName()
        {
            //TODO: implements this
            var method = new MethodDefinition("LoadRelationValuesByName", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.DataReaderInterface));
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateGetKeyValueDirect()
        {
            //TODO: implements this
            var method = new MethodDefinition("GetKeyValueDirect", MethodAttr, _handler.ObjectType);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.LoadArg(1);
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateGetKeyValuesDirect()
        {
            //TODO: implements this
            var method = new MethodDefinition("GetKeyValuesDirect", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("dic", ParameterAttributes.None, _handler.DictionaryStringObjectType));
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateSetValuesForSelectDirect()
        {
            //TODO: implements this
            var method = new MethodDefinition("SetValuesForSelectDirect", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("keys", ParameterAttributes.None, _handler.ListKeyValuePairStringStringType));
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateSetValuesForInsertDirect()
        {
            //TODO: implements this
            var method = new MethodDefinition("SetValuesForInsertDirect", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("values", ParameterAttributes.None, _handler.KeyValueCollectionType));
            method.Parameters.Add(new ParameterDefinition("obj", ParameterAttributes.None, _handler.ObjectType));
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateSetValuesForUpdateDirect()
        {
            //TODO: implements this
            var method = new MethodDefinition("SetValuesForUpdateDirect", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("values", ParameterAttributes.None, _handler.KeyValueCollectionType));
            method.Parameters.Add(new ParameterDefinition("obj", ParameterAttributes.None, _handler.ObjectType));
            var processor = new IlBuilder(method.Body.GetILProcessor());
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }
    }
}
