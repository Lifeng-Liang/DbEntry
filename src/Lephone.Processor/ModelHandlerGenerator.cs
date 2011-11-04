using System;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Data.Model;
using Lephone.Data.Model.Member;
using Lephone.Data.SqlEntry;
using Lephone.Core;
using Mono.Cecil;

namespace Lephone.Processor
{
    public class ModelHandlerGenerator
    {
        private static readonly DataReaderEmitHelper Helper = new DataReaderEmitHelper();

        private const MethodAttributes CtMethodAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual; //public hidebysig virtual instance
        private const MethodAttributes MethodAttr = MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual; //public hidebysig virtual instance
        private const MethodAttributes CtorAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

        private readonly TypeDefinition _model;
        private readonly KnownTypesHandler _handler;
        private readonly TypeDefinition _result;

        private readonly ObjectInfo _info;

        private readonly Type _type;

        public ModelHandlerGenerator(Type type, TypeDefinition model, KnownTypesHandler handler)
        {
            this._type = type;
            this._model = model;
            this._handler = handler;
            _result = TypeFactory.CreateType(model, _handler.ModelHandlerBaseType);
            _result.Interfaces.Add(_handler.DbObjectHandlerInterface);
            _model.CustomAttributes.Add(_handler.GetModelHandler(_result));

            _info = ObjectInfoFactory.Instance.GetInstance(type);

            CheckType();
        }

        private void CheckType()
        {
            if (_type.IsSubclassOf(typeof(DbObjectSmartUpdate)))
            {
                foreach(var field in _info.SimpleMembers)
                {
                    if(!field.MemberInfo.IsProperty)
                    {
                        throw new ModelException(
                            field.MemberInfo,
                            "The subclass of DbObjectModel can not has any fields, use property instead.");
                    }
                }
            }
            if(_info.From.PartOf != null)
            {
                var oi = ObjectInfoFactory.Instance.GetInstance(_info.From.PartOf);
                var dic = new Dictionary<string, int>();
                foreach(var member in oi.Members)
                {
                    if(member.Is.SimpleField || member.Is.LazyLoad || member.Is.BelongsTo)
                    {
                        dic.Add(member.Name, 1);
                    }
                }
                foreach(var member in _info.SimpleMembers)
                {
                    if(!dic.ContainsKey(member.Name))
                    {
                        throw new ModelException(member.MemberInfo,
                            "The member of PartOf-model can not find in the origin model");
                    }
                }
            }
        }

        public TypeDefinition Generate()
        {
            GenerateConstructor();
            GenerateCreateInstance();
            GenerateLoadSimpleValuesByIndex();
            GenerateLoadSimpleValuesByName();
            GenerateLoadRelationValues(true, false);
            GenerateLoadRelationValues(false, false);
            GenerateLoadRelationValues(true, true);
            GenerateLoadRelationValues(false, true);
            GenerateGetKeyValueDirect();
            GenerateGetKeyValuesDirect();
            GenerateSetKeyValueDirect();
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
            foreach (var f in _info.SimpleMembers)
            {
                processor.LoadLoc(0);
                if (f.Is.AllowNull) { processor.LoadArg(0); }
                processor.LoadArg(2).LoadInt(n);
                var mi1 = Helper.GetMethodInfo(f.MemberType);
                if (f.Is.AllowNull || mi1 == null)
                {
                    processor.CallVirtual(_handler.GetDataReaderMethodInt());
                    if (f.Is.AllowNull)
                    {
                        SetSecendArgForGetNullable(f, processor);
                        processor.Call(_handler.ModelHandlerBaseTypeGetNullable);
                    }
                    // cast or unbox
                    processor.CastOrUnbox(_handler.Import(f.MemberType), _handler);
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
            if (f.MemberType.IsValueType && f.MemberType == typeof(Guid?))
            {
                il.LoadInt(1);
            }
            else if (f.MemberType.IsValueType && f.MemberType == typeof(bool?))
            {
                il.LoadInt(2);
            }
            else if (f.MemberType.IsValueType && f.MemberType == typeof(Date?))
            {
                il.LoadInt(3);
            }
            else if (f.MemberType.IsValueType && f.MemberType == typeof(Time?))
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
            foreach (var f in _info.SimpleMembers)
            {
                // get value
                processor.LoadLoc(0);
                if (f.Is.AllowNull) { processor.LoadArg(0); }
                processor.LoadArg(2).LoadString(f.Name).CallVirtual(_handler.GetDataReaderMethodString());
                if (f.Is.AllowNull)
                {
                    SetSecendArgForGetNullable(f, processor);
                    processor.Call(_handler.ModelHandlerBaseTypeGetNullable);
                }
                // cast or unbox
                processor.CastOrUnbox(_handler.Import(f.MemberType), _handler);
                // set field
                processor.SetMember(f, _handler);
            }

            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateLoadRelationValues(bool useIndex, bool noLazy)
        {
            int index = _info.SimpleMembers.Length;
            string methodName = useIndex ? "LoadRelationValuesByIndex" : "LoadRelationValuesByName";
            if (noLazy)
            {
                methodName = methodName + "NoLazy";
            }
            var method = new MethodDefinition(methodName, MethodAttr, _handler.VoidType);
            method.Overrides.Add(useIndex
                ? (noLazy ? _handler.LoadRelationValuesByIndexNoLazy : _handler.LoadRelationValuesByIndex)
                : (noLazy ? _handler.LoadRelationValuesByNameNoLazy : _handler.LoadRelationValuesByName));
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("dr", ParameterAttributes.None, _handler.DataReaderInterface));
            var processor = new IlBuilder(method.Body);

            if(_info.RelationMembers.Length > 0)
            {
                // User u = (User)o;
                processor.DeclareLocal(_model);
                processor.LoadArg(1).Cast(_model).SetLoc(0);
                // set values
                foreach (var f in _info.RelationMembers)
                {
                    if (f.Is.LazyLoad)
                    {
                        if (noLazy)
                        {
                            processor.LoadLoc(0);
                            processor.GetMember(f, _handler);
                            processor.LoadArg(2);
                            if (useIndex)
                            {
                                processor.LoadInt(index++).CallVirtual(_handler.GetDataReaderMethodInt());
                            }
                            else
                            {
                                processor.LoadString(f.Name).CallVirtual(_handler.GetDataReaderMethodString());
                            }
                            processor.LoadInt(0);
                            processor.CallVirtual(_handler.LazyLoadingInterfaceWrite);
                        }
                    }
                    else if (f.Is.BelongsTo)
                    {
                        processor.LoadLoc(0);
                        processor.GetMember(f, _handler);
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
            var method = new MethodDefinition("GetKeyValueDirect", MethodAttr, _handler.ObjectType);
            method.Overrides.Add(_handler.GetKeyValueDirect);
            method.Parameters.Add(new ParameterDefinition("o", ParameterAttributes.None, _handler.ObjectType));
            var processor = new IlBuilder(method.Body);

            if (_info.KeyMembers.Length == 1)
            {
                var h = _info.KeyMembers[0];
                processor.LoadArg(1).Cast(_model);
                processor.GetMember(h, _handler);
                processor.Box(_handler.Import(h.MemberType));
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
            var method = new MethodDefinition("GetKeyValuesDirect", MethodAttr, _handler.VoidType);
            method.Overrides.Add(_handler.GetKeyValuesDirect);
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
                processor.GetMember(f, _handler);
                processor.Box(_handler.Import(f.MemberType)).CallVirtual(_handler.DictionaryStringObjectAdd);
            }

            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateSetKeyValueDirect()
        {
            var method = new MethodDefinition("SetKeyValueDirect", MethodAttr, _handler.VoidType);
            method.Overrides.Add(_handler.SetKeyValueDirect);
            method.Parameters.Add(new ParameterDefinition("obj", ParameterAttributes.None, _handler.ObjectType));
            method.Parameters.Add(new ParameterDefinition("key", ParameterAttributes.None, _handler.ObjectType));
            var processor = new IlBuilder(method.Body);

            if (_info.KeyMembers.Length == 1)
            {
                var h = _info.KeyMembers[0];
                processor.LoadArg(1).Cast(_model);
                processor.LoadArg(2);
                var fh = _info.KeyMembers[0];
                if (fh.MemberType == typeof(long))
                {
                    processor.Call(_handler.ConvertToInt64);
                }
                else if (fh.MemberType == typeof(int))
                {
                    processor.Call(_handler.ConvertToInt32);
                }
                else if (fh.MemberType == typeof(Guid))
                {
                    processor.Unbox(_handler.Import(h.MemberType));
                }
                else
                {
                    processor.Cast(_handler.Import(h.MemberType));
                }
                processor.SetMember(h, _handler);
            }

            processor.Return();
            processor.Append();
            _result.Methods.Add(method);
        }

        private void GenerateSetValuesForSelectDirect()
        {
            GenerateSetValuesForSelectDirectDirect("SetValuesForSelectDirect", false);
            GenerateSetValuesForSelectDirectDirect("SetValuesForSelectDirectNoLazy", true);
        }

        private void GenerateSetValuesForSelectDirectDirect(string methodName, bool noLazy)
        {
            var method = new MethodDefinition(methodName, MethodAttr, _handler.VoidType);
            method.Overrides.Add(noLazy ? _handler.SetValuesForSelectDirectNoLazy : _handler.SetValuesForSelectDirect);
            method.Parameters.Add(new ParameterDefinition("keys", ParameterAttributes.None, _handler.ListKeyValuePairStringStringType));
            var processor = new IlBuilder(method.Body);

            foreach (var f in _info.Members)
            {
                if (!f.Is.HasOne && !f.Is.HasMany && !f.Is.HasAndBelongsToMany)
                {
                    if (noLazy || !f.Is.LazyLoad)
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
                                    m => m.Is.UpdatedOn,
                                    m => m.Is.CreatedOn || m.Is.SavedOn || m.Is.Count);

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
                                        m => m.Is.CreatedOn || m.Is.Key,
                                        m => m.Is.UpdatedOn || m.Is.SavedOn || m.Is.Count);
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
            foreach (var f in _info.Members)
            {
                if (!f.Is.DbGenerate && !f.Is.HasOne && !f.Is.HasMany && !f.Is.HasAndBelongsToMany)
                {
                    if (!cb1(f))
                    {
                        processor.LoadArg(1).LoadArg(0).LoadInt(n);
                        if (cb2(f))
                        {
                            processor.LoadInt((int)(f.Is.Count ? AutoValue.Count : AutoValue.DbNow))
                                .Box(_handler.AutoValueType).Call(_handler.ModelHandlerBaseTypeNewKeyValueDirect);
                        }
                        else
                        {
                            processor.LoadLoc(0);
                            processor.GetMember(f, _handler);
                            if (f.Is.BelongsTo)
                            {
                                processor.CallVirtual(_handler.Import(f.MemberType.GetMethod("get_ForeignKey")));
                            }
                            else if (f.Is.LazyLoad)
                            {
                                var it = f.MemberType.GetGenericArguments()[0];
                                processor.CallVirtual(_handler.Import(f.MemberType.GetMethod("get_Value")));
                                processor.Box(_handler.Import(it));
                            }
                            else
                            {
                                processor.Box(_handler.Import(f.MemberType));
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
            foreach (var f in _info.Members)
            {
                if (!f.Is.DbGenerate && !f.Is.HasOne && !f.Is.HasMany && !f.Is.HasAndBelongsToMany)
                {
                    if (!f.Is.Key && (f.Is.UpdatedOn || f.Is.SavedOn || !f.Is.CreatedOn || f.Is.Count))
                    {
                        if (f.Is.UpdatedOn || f.Is.SavedOn || f.Is.Count)
                        {
                            processor.LoadArg(1).LoadArg(0).LoadInt(n)
                                .LoadInt((int)(f.Is.Count ? AutoValue.Count : AutoValue.DbNow)).Box(_handler.AutoValueType)
                                .Call(_handler.ModelHandlerBaseTypeNewKeyValueDirect).CallVirtual(_handler.KeyValueCollectionAdd);
                        }
                        else
                        {
                            processor.LoadArg(0).LoadArg(1).LoadLoc(0).LoadString(f.Name).LoadInt(n).LoadLoc(0);
                            processor.GetMember(f, _handler);
                            if (f.Is.BelongsTo)
                            {
                                processor.CallVirtual(_handler.Import(f.MemberType.GetMethod("get_ForeignKey")));
                            }
                            else if (f.Is.LazyLoad)
                            {
                                var it = f.MemberType.GetGenericArguments()[0];
                                processor.CallVirtual(_handler.Import(f.MemberType.GetMethod("get_Value")));
                                processor.Box(_handler.Import(it));
                            }
                            else
                            {
                                processor.Box(_handler.Import(f.MemberType));
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
