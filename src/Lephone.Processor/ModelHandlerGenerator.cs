using System;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.SqlEntry;
using Lephone.Core;
using Mono.Cecil;

namespace Lephone.Processor
{
    public class ModelHandlerGenerator
    {
        private static readonly DataReaderEmitHelper Helper = new DataReaderEmitHelper();

        private const TypeAttributes ClassTypeAttr = TypeAttributes.Class | TypeAttributes.Public;
        private const MethodAttributes CtMethodAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual; //public hidebysig virtual instance
        private const MethodAttributes MethodAttr = MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual; //public hidebysig virtual instance
        private const MethodAttributes CtorAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

        private const string MemberPrifix = "$";
        private readonly TypeDefinition _model;
        private readonly KnownTypesHandler _handler;
        private readonly TypeDefinition _result;
        private static int _index;

        private readonly ObjectInfo _info;

        private readonly Type _type;

        public ModelHandlerGenerator(Type type, TypeDefinition model, KnownTypesHandler handler)
        {
            this._type = type;
            this._model = model;
            this._handler = handler;
            _index++;
            _result = new TypeDefinition(MemberPrifix + type.Namespace, MemberPrifix + _index + "_" + type.Name,
                ClassTypeAttr, _handler.ModelHandlerBaseType);
            _result.Interfaces.Add(_handler.DbObjectHandlerInterface);
            _model.CustomAttributes.Add(_handler.GetModelHandler(_result));

            _info = ObjectInfo.GetInstance(type);
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
            var method = new MethodDefinition("CreateInstance", CtMethodAttr, _handler.ObjectType);
            method.Overrides.Add(_handler.CreateInstance);
            var processor = new IlBuilder(method.Body);
            processor.NewObj(ctor);
            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateLoadSimpleValuesByIndex()
        {
            var method = new MethodDefinition("LoadSimpleValuesByIndex", MethodAttr, _handler.VoidType);
            method.Overrides.Add(_handler.LoadSimpleValuesByIndex);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.DataReaderInterface));
            var processor = new IlBuilder(method.Body);

            // User u = (User)o;
            processor.DeclareLocal(_model);
            processor.LoadArg(1).Cast(_model).SetLoc(0);
            // set values
            int n = 0;
            foreach (var f in _info.SimpleFields)
            {
                processor.LoadLoc(0);
                if (f.AllowNull) { processor.LoadArg(0); }
                processor.LoadArg(2).LoadInt(n);
                var mi1 = Helper.GetMethodInfo(f.FieldType);
                if (f.AllowNull || mi1 == null)
                {
                    processor.CallVirtual(_handler.GetDataReaderMethodInt());
                    if (f.AllowNull)
                    {
                        SetSecendArgForGetNullable(f, processor);
                        processor.Call(_handler.ModelHandlerBaseTypeGetNullable);
                    }
                    // cast or unbox
                    processor.CastOrUnbox(_handler.Import(f.FieldType), _handler);
                }
                else
                {
                    processor.CallVirtual(_handler.Import(mi1));
                }
                processor.SetMember(f, _handler);
                n++;
            }

            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private static void SetSecendArgForGetNullable(MemberHandler f, IlBuilder il)
        {
            if (f.FieldType.IsValueType && f.FieldType == typeof(Guid?))
            {
                il.LoadInt(1);
            }
            else if (f.FieldType.IsValueType && f.FieldType == typeof(bool?))
            {
                il.LoadInt(2);
            }
            else if (f.FieldType.IsValueType && f.FieldType == typeof(Date?))
            {
                il.LoadInt(3);
            }
            else if (f.FieldType.IsValueType && f.FieldType == typeof(Time?))
            {
                il.LoadInt(4);
            }
            else
            {
                il.LoadInt(0);
            }
        }

        private void GenerateLoadSimpleValuesByName()
        {
            var method = new MethodDefinition("LoadSimpleValuesByName", MethodAttr, _handler.VoidType);
            method.Overrides.Add(_handler.LoadSimpleValuesByName);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.DataReaderInterface));
            var processor = new IlBuilder(method.Body);

            // User u = (User)o;
            processor.DeclareLocal(_model);
            processor.LoadArg(1).Cast(_model).SetLoc(0);
            // set values
            foreach (var f in _info.SimpleFields)
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
                processor.CastOrUnbox(_handler.Import(f.FieldType), _handler);
                // set field
                processor.SetMember(f, _handler);
            }

            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateLoadRelationValues(bool useIndex)
        {
            int index = _info.SimpleFields.Length;
            string methodName = useIndex ? "LoadRelationValuesByIndex" : "LoadRelationValuesByName";
            var method = new MethodDefinition(methodName, MethodAttr, _handler.VoidType);
            method.Overrides.Add(useIndex ? _handler.LoadRelationValuesByIndex : _handler.LoadRelationValuesByName);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.DataReaderInterface));
            var processor = new IlBuilder(method.Body);

            if(_info.RelationFields.Length > 0)
            {
                // User u = (User)o;
                processor.DeclareLocal(_model);
                processor.LoadArg(1).Cast(_model).SetLoc(0);
                // set values
                foreach (var f in _info.RelationFields)
                {
                    processor.LoadLoc(0);
                    processor.GetMember(f, _handler);
                    if (f.IsLazyLoad)
                    {
                        processor.LoadString(f.Name).CallVirtual(_handler.LazyLoadingInterfaceInit);
                    }
                    else if (f.IsHasOne || f.IsHasMany)
                    {
                        var oi1 = ObjectInfo.GetSimpleInstance(f.FieldType.GetGenericArguments()[0]);
                        var mh = oi1.GetBelongsTo(_type);
                        if (mh == null)
                        {
                            throw new DataException("HasOne/HasMany and BelongsTo must be paired.");
                        }
                        processor.LoadString(mh.Name).CallVirtual(_handler.LazyLoadingInterfaceInit);
                    }
                    else if (f.IsHasAndBelongsToMany)
                    {
                        var oi1 = ObjectInfo.GetSimpleInstance(f.FieldType.GetGenericArguments()[0]);
                        var mh = oi1.GetHasAndBelongsToMany(_type);
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
            }

            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateGetKeyValueDirect()
        {
            //TODO: implements this
            var method = new MethodDefinition("GetKeyValueDirect", MethodAttr, _handler.ObjectType);
            method.Overrides.Add(_handler.GetKeyValueDirect);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            var processor = new IlBuilder(method.Body);

            if (_info.KeyFields.Length == 1)
            {
                var h = _info.KeyFields[0];
                processor.LoadArg(1).Cast(_model);
                processor.GetMember(h, _handler);
                processor.Box(_handler.Import(h.FieldType));
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
            method.Overrides.Add(_handler.GetKeyValuesDirect);
            method.Parameters.Add(new ParameterDefinition("dic", ParameterAttributes.None, _handler.DictionaryStringObjectType));
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            var processor = new IlBuilder(method.Body);

            // User u = (User)o;
            processor.DeclareLocal(_model);
            processor.LoadArg(2).Cast(_model).SetLoc(0);
            // set values
            foreach (var f in _info.KeyFields)
            {
                processor.LoadArg(1).LoadString(f.Name).LoadLoc(0);
                processor.GetMember(f, _handler);
                processor.Box(_handler.Import(f.FieldType)).CallVirtual(_handler.DictionaryStringObjectAdd);
            }

            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateSetValuesForSelectDirect()
        {
            var method = new MethodDefinition("SetValuesForSelectDirect", MethodAttr, _handler.VoidType);
            method.Overrides.Add(_handler.SetValuesForSelectDirect);
            method.Parameters.Add(new ParameterDefinition("keys", ParameterAttributes.None, _handler.ListKeyValuePairStringStringType));
            var processor = new IlBuilder(method.Body);

            foreach (var f in _info.Fields)
            {
                if (!f.IsHasOne && !f.IsHasMany && !f.IsHasAndBelongsToMany && !f.IsLazyLoad)
                {
                    processor.LoadArg(1);

                    processor.LoadString(f.Name);
                    if (f.Name != f.MemberInfo.Name)
                    {
                        processor.LoadString(f.MemberInfo.Name);
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
            method.Overrides.Add(_handler.SetValuesForInsertDirect);
            method.Parameters.Add(new ParameterDefinition("values", ParameterAttributes.None, _handler.KeyValueCollectionType));
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
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
            method.Overrides.Add(_handler.SetValuesForUpdateDirect);
            method.Parameters.Add(new ParameterDefinition("values", ParameterAttributes.None, _handler.KeyValueCollectionType));
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            var processor = new IlBuilder(method.Body);

            if (_model.BaseTypeIsDbObjectSmartUpdate())
            {
                GenerateSetValuesForPartialUpdate(processor);
            }
            else
            {
                GenerateSetValuesDirect(processor,
                                        m => m.IsCreatedOn || m.IsKey,
                                        m => m.IsUpdatedOn || m.IsSavedOn || m.IsCount);
            }


            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateSetValuesDirect(IlBuilder processor, CallbackReturnHandler<MemberHandler, bool> cb1, CallbackReturnHandler<MemberHandler, bool> cb2)
        {
            // User u = (User)o;
            processor.DeclareLocal(_model);
            processor.LoadArg(2).Cast(_model).SetLoc(0);
            // set values
            int n = 0;
            foreach (var f in _info.Fields)
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
                            processor.GetMember(f, _handler);
                            if (f.IsBelongsTo)
                            {
                                processor.CallVirtual(_handler.Import(f.FieldType.GetMethod("get_ForeignKey")));
                            }
                            else if (f.IsLazyLoad)
                            {
                                var it = f.FieldType.GetGenericArguments()[0];
                                processor.CallVirtual(_handler.Import(f.FieldType.GetMethod("get_Value")));
                                processor.Box(_handler.Import(it));
                            }
                            else
                            {
                                processor.Box(_handler.Import(f.FieldType));
                            }
                            processor.Call(_handler.ModelHandlerBaseTypeNewKeyValue);
                        }
                        processor.CallVirtual(_handler.KeyValueCollectionAdd);
                    }
                    n++;
                }
            }
        }

        private void GenerateSetValuesForPartialUpdate(IlBuilder processor)
        {
            // User u = (User)o;
            processor.DeclareLocal(_model);
            processor.LoadArg(2).Cast(_model).SetLoc(0);
            // set values
            int n = 0;
            foreach (var f in _info.Fields)
            {
                if (!f.IsDbGenerate && !f.IsHasOne && !f.IsHasMany && !f.IsHasAndBelongsToMany)
                {
                    if (!f.IsKey && (f.IsUpdatedOn || f.IsSavedOn || !f.IsCreatedOn || f.IsCount))
                    {
                        if (f.IsUpdatedOn || f.IsSavedOn || f.IsCount)
                        {
                            processor.LoadArg(1).LoadArg(0).LoadInt(n)
                                .LoadInt((int)(f.IsCount ? AutoValue.Count : AutoValue.DbNow)).Box(_handler.AutoValueType)
                                .Call(_handler.ModelHandlerBaseTypeNewKeyValueDirect).CallVirtual(_handler.KeyValueCollectionAdd);
                        }
                        else
                        {
                            processor.LoadArg(0).LoadArg(1).LoadLoc(0).LoadString(f.Name).LoadInt(n).LoadLoc(0);
                            processor.GetMember(f, _handler);
                            if (f.IsBelongsTo)
                            {
                                processor.CallVirtual(_handler.Import(f.FieldType.GetMethod("get_ForeignKey")));
                            }
                            else if (f.IsLazyLoad)
                            {
                                var it = f.FieldType.GetGenericArguments()[0];
                                processor.CallVirtual(_handler.Import(f.FieldType.GetMethod("get_Value")));
                                processor.Box(_handler.Import(it));
                            }
                            else
                            {
                                processor.Box(_handler.Import(f.FieldType));
                            }
                            processor.Call(_handler.ModelHandlerBaseTypeAddKeyValue);
                        }
                    }
                    n++;
                }
            }
        }
    }
}
