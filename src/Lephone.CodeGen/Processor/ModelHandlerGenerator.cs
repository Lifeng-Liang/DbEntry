using Lephone.Data;
using Lephone.Data.SqlEntry;
using Lephone.Core;
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

        private readonly ModelInformation _info;

        public ModelHandlerGenerator(TypeDefinition model, KnownTypesHandler handler)
        {
            this._model = model;
            this._handler = handler;
            _index++;
            _result = new TypeDefinition("$Lephone", MemberPrifix + _index,
                ClassTypeAttr, _handler.ModelHandlerBaseType);
            _model.CustomAttributes.Add(_handler.GetModelHandler(_result));

            _info = ModelInformation.GetInstance(model, _handler);
        }

        public TypeDefinition Generate()
        {
            GenerateConstructor();
            GenerateCreateInstance();
            GenerateLoadSimpleValuesByIndex();
            GenerateLoadSimpleValuesByName();
            GenerateLoadRelationValues(true);
            GenerateLoadRelationValues(false);
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
            var processor = new IlBuilder(method.Body);
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
            var processor = new IlBuilder(method.Body);
            processor.NewObj(ctor);
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateLoadSimpleValuesByIndex()
        {
            var method = new MethodDefinition("LoadSimpleValuesByIndex", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.DataReaderInterface));
            var processor = new IlBuilder(method.Body);

            // User u = (User)o;
            processor.DeclareLocal(_model);
            processor.LoadArg(1).Cast(_model).SetLoc(0);
            // set values
            int n = 0;
            foreach (var f in _info.SimpleMembers)
            {
                processor.LoadLoc(0);
                if (f.AllowNull) { processor.LoadArg(0); }
                processor.LoadArg(2).LoadInt(n);
                var mi1 = _handler.GetDataReaderMethod(f.MemberType);
                if (f.AllowNull || mi1 == null)
                {
                    processor.CallVirtual(_handler.GetDataReaderMethodInt());
                    if (f.AllowNull)
                    {
                        SetSecendArgForGetNullable(f, processor);
                        processor.Call(_handler.ModelHandlerBaseTypeGetNullable);
                    }
                    // cast or unbox
                    processor.CastOrUnbox(f.MemberType, _handler);
                }
                else
                {
                    processor.CallVirtual(mi1);
                }
                processor.SetMember(f);
                n++;
            }

            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private static void SetSecendArgForGetNullable(ModelMember f, IlBuilder il)
        {
            if (f.MemberType.IsValueType && f.MemberType.GenericParameters[0].FullName == KnownTypesHandler.Guid)
            {
                il.LoadInt(1);
            }
            else if (f.MemberType.IsValueType && f.MemberType.GenericParameters[0].FullName == KnownTypesHandler.Bool)
            {
                il.LoadInt(2);
            }
            else
            {
                il.LoadInt(0);
            }
        }

        private void GenerateLoadSimpleValuesByName()
        {
            var method = new MethodDefinition("LoadSimpleValuesByName", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.DataReaderInterface));
            var processor = new IlBuilder(method.Body);

            // User u = (User)o;
            processor.DeclareLocal(_model);
            processor.LoadArg(1).Cast(_model).SetLoc(0);
            // set values
            foreach (var f in _info.SimpleMembers)
            {
                // get value
                processor.LoadLoc(0);
                if (f.AllowNull) { processor.LoadArg(0); }
                processor.LoadArg(2).LoadString(f.Name).CallVirtual(_handler.GetDataReaderMethodString());
                if (f.AllowNull)
                {
                    SetSecendArgForGetNullable(f, processor);
                    processor.Call(_handler.ModelHandlerBaseTypeGetNullable);
                }
                // cast or unbox
                processor.CastOrUnbox(f.MemberType, _handler);
                // set field
                processor.SetMember(f);
            }

            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateLoadRelationValues(bool useIndex)
        {
            int index = _info.SimpleMembers.Count;
            string methodName = useIndex ? "LoadRelationValuesByIndex" : "LoadRelationValuesByName";
            var method = new MethodDefinition(methodName, MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.DataReaderInterface));
            var processor = new IlBuilder(method.Body);

            // User u = (User)o;
            processor.DeclareLocal(_model);
            processor.LoadArg(1).Cast(_model).SetLoc(0);
            // set values
            foreach (var f in _info.RelationMembers)
            {
                processor.LoadLoc(0);
                processor.GetMember(f);
                if (f.IsLazyLoad)
                {
                    processor.LoadString(f.Name).CallVirtual(_handler.LazyLoadingInterfaceInit);
                }
                else if (f.IsHasOne || f.IsHasMany)
                {
                    var oi1 = ModelInformation.GetInstance(f.GetFirstGenericArgument(), _handler); //ObjectInfo.GetSimpleInstance(f.FieldType.GetGenericArguments()[0]);
                    var mh = oi1.GetBelongsTo(_model);
                    if (mh == null)
                    {
                        throw new DataException("HasOne/HasMany and BelongsTo must be paired.");
                    }
                    processor.LoadString(mh.Name).CallVirtual(_handler.LazyLoadingInterfaceInit);
                }
                else if (f.IsHasAndBelongsToMany)
                {
                    var oi1 = ModelInformation.GetInstance(f.GetFirstGenericArgument(), _handler); // ObjectInfo.GetSimpleInstance(f.FieldType.GetGenericArguments()[0]);
                    var mh = oi1.GetHasAndBelongsToMany(_model);
                    if (mh == null)
                    {
                        throw new DataException("HasOne/HasMany and BelongsTo must be paired.");
                    }
                    processor.LoadString(mh.Name).CallVirtual(_handler.LazyLoadingInterfaceInit);
                }
                else if (f.IsBelongsTo)
                {
                    processor.LoadArg(2);
                    if (useIndex)
                    {
                        processor.LoadInt(index++).CallVirtual(_handler.GetDataReaderMethodInt());
                    }
                    else
                    {
                        processor.LoadString(f.Name).CallVirtual(_handler.GetDataReaderMethodString());
                    }
                    processor.CallVirtual(_handler.BelongsToInterfaceSetForeignKey);
                }
            }

            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateGetKeyValueDirect()
        {
            //TODO: implements this
            var method = new MethodDefinition("GetKeyValueDirect", MethodAttr, _handler.ObjectType);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            var processor = new IlBuilder(method.Body);

            if (_info.KeyMembers.Count == 1)
            {
                var h = _info.KeyMembers[0];
                processor.LoadArg(1).Cast(_model);
                processor.GetMember(h);
                processor.Box(h.MemberType);
            }
            else
            {
                processor.LoadNull();
            }

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
            var processor = new IlBuilder(method.Body);

            // User u = (User)o;
            processor.DeclareLocal(_model);
            processor.LoadArg(2).Cast(_model).SetLoc(0);
            // set values
            foreach (var f in _info.KeyMembers)
            {
                processor.LoadArg(1).LoadString(f.Name).LoadLoc(0);
                processor.GetMember(f);
                processor.Box(f.MemberType).CallVirtual(_handler.DictionaryStringObjectAdd);
            }

            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateSetValuesForSelectDirect()
        {
            var method = new MethodDefinition("SetValuesForSelectDirect", MethodAttr, _handler.VoidType);
            method.Parameters.Add(new ParameterDefinition("keys", ParameterAttributes.None, _handler.ListKeyValuePairStringStringType));
            var processor = new IlBuilder(method.Body);

            foreach (var f in _info.Members)
            {
                if (!f.IsHasOne && !f.IsHasMany && !f.IsHasAndBelongsToMany && !f.IsLazyLoad)
                {
                    processor.LoadArg(1);

                    processor.LoadString(f.Name);
                    if (f.Name != f.Member.Name)
                    {
                        processor.LoadString(f.Member.Name);
                    }
                    else
                    {
                        processor.LoadNull();
                    }
                    processor.NewObj(_handler.KeyValuePairStringStringCtor);

                    processor.CallVirtual(_handler.ListKeyValuePairStringStringAdd);
                }
            }

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
            var processor = new IlBuilder(method.Body);

            GenerateSetValuesDirect(processor,
                                    m => m.IsUpdatedOn,
                                    m => m.IsCreatedOn || m.IsSavedOn || m.IsCount);

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
            var processor = new IlBuilder(method.Body);

            if (_model.BaseTypeIsDbObjectSmartUpdate())
            {
                GenerateSetValuesForPartialUpdate(processor);
            }
            else
            {
                GenerateSetValuesDirect(processor,
                                        m => m.IsCreatedOn || m.IsDbKey,
                                        m => m.IsUpdatedOn || m.IsSavedOn || m.IsCount);
            }


            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateSetValuesDirect(IlBuilder processor, CallbackReturnHandler<ModelMember, bool> cb1, CallbackReturnHandler<ModelMember, bool> cb2)
        {
            // User u = (User)o;
            processor.DeclareLocal(_model);
            processor.LoadArg(2).Cast(_model).SetLoc(0);
            // set values
            int n = 0;
            foreach (var f in _info.Members)
            {
                if (!f.IsDbGenerate && !f.IsHasOne && !f.IsHasMany && !f.IsHasAndBelongsToMany)
                {
                    if (!cb1(f))
                    {
                        processor.LoadArg(1).LoadArg(0).LoadInt(n);
                        if (cb2(f))
                        {
                            processor.LoadInt((int)(f.IsCount ? AutoValue.Count : AutoValue.DbNow))
                                .Box(_handler.AutoValueType).Call(_handler.ModelHandlerBaseTypeNewKeyValueDirect);
                        }
                        else
                        {
                            processor.LoadLoc(0);
                            processor.GetMember(f);
                            if (f.IsBelongsTo)
                            {
                                processor.CallVirtual(f.MemberType.GetMethod("get_ForeignKey"));
                            }
                            else if (f.IsLazyLoad)
                            {
                                var it = ((GenericInstanceType)f.MemberType).GenericArguments[0];
                                processor.CallVirtual(f.MemberType.GetMethod("get_Value")).Box(it);
                            }
                            else
                            {
                                processor.Box(f.MemberType);
                            }
                            processor.Call(_handler.ModelHandlerBaseTypeNewKeyValue);
                        }
                        processor.CallVirtual(_handler.KeyValueCollectionAdd);
                    }
                    n++;
                }
            }
        }

        private void GenerateSetValuesForPartialUpdate(IlBuilder il)
        {
            // User u = (User)o;
            il.DeclareLocal(_model);
            il.LoadArg(2).Cast(_model).SetLoc(0);
            // set values
            int n = 0;
            foreach (var f in _info.Members)
            {
                if (!f.IsDbGenerate && !f.IsHasOne && !f.IsHasMany && !f.IsHasAndBelongsToMany)
                {
                    if (!f.IsDbKey && (f.IsUpdatedOn || f.IsSavedOn || !f.IsCreatedOn || f.IsCount))
                    {
                        if (f.IsUpdatedOn || f.IsSavedOn || f.IsCount)
                        {
                            il.LoadArg(1).LoadArg(0).LoadInt(n)
                                .LoadInt((int)(f.IsCount ? AutoValue.Count : AutoValue.DbNow)).Box(_handler.AutoValueType)
                                .Call(_handler.ModelHandlerBaseTypeNewKeyValueDirect).CallVirtual(_handler.KeyValueCollectionAdd);
                        }
                        else
                        {
                            il.LoadArg(0).LoadArg(1).LoadLoc(0).LoadString(f.Name).LoadInt(n).LoadLoc(0);
                            il.GetMember(f);
                            if (f.IsBelongsTo)
                            {
                                il.CallVirtual(f.MemberType.GetMethod("get_ForeignKey"));
                            }
                            else if (f.IsLazyLoad)
                            {
                                var it = ((GenericInstanceType)f.MemberType).GenericArguments[0];
                                il.CallVirtual(f.MemberType.GetMethod("get_Value")).Box(it);
                            }
                            else
                            {
                                il.Box(f.MemberType);
                            }
                            il.Call(_handler.ModelHandlerBaseTypeAddKeyValue);
                        }
                    }
                    n++;
                }
            }
        }
    }
}
