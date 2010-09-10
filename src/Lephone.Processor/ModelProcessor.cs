using System;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Common;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Lephone.Processor
{
    public class ModelProcessor
    {
        private const MethodAttributes FlagGetObjectData
            = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot
              | MethodAttributes.Virtual | MethodAttributes.Final;
        private const string MemberPrifix = "$";
        private readonly TypeDefinition _model;
        private readonly KnownTypesHandler _handler;
        private readonly List<PropertyInformation> _properties;

        public ModelProcessor(TypeDefinition model, KnownTypesHandler handler)
        {
            this._model = model;
            this._handler = handler;
            _properties = GetProperties();
        }

        public void Process()
        {
            if(_model.BaseTypeIsDbObjectSmartUpdate())
            {
                ProcessSerializable();
                ProcessProperties();
                ProcessConstructors();
                RemoveCompilerGeneratedFields();
            }
        }

        private void RemoveCompilerGeneratedFields()
        {
            for (int i = _model.Fields.Count - 1; i >= 0; i--)
            {
                var field = _model.Fields[i];
                if (field.IsCompilerGenerated())
                {
                    _model.Fields.RemoveAt(i);
                }
            }
        }

        private void ProcessSerializable()
        {
            if(_model.IsSerializable && !_model.IsAbstract)
            {
                _model.Interfaces.Add(_handler.SerializableInterface);
                var method = new MethodDefinition("System.Runtime.Serialization.ISerializable.GetObjectData", FlagGetObjectData, _handler.VoidType);
                method.Overrides.Add(_handler.SerializableGetObjectData);
                method.Parameters.Add(new ParameterDefinition("info", ParameterAttributes.None, _handler.SerializationInfoType));
                method.Parameters.Add(new ParameterDefinition("context", ParameterAttributes.None, _handler.StreamingContextType));
                var processor = new IlBuilder(method.Body);
                processor.LoadArg(0).LoadArg(1).LoadArg(2).Call(_handler.DynamicObjectReferenceSerializeObject);
                processor.Return();
                processor.Append();
                _model.Methods.Add(method);
            }
        }

        private void ProcessProperties()
        {
            foreach (var pi in _properties)
            {
                var t = pi.PropertyDefinition.PropertyType;
                if (!t.IsValueType && !t.IsArray && t.FullName != KnownTypesHandler.String)
                {
                    if(pi.IsComposedOf && pi.FieldType != FieldType.Normal)
                    {
                        throw new DataException("ComposedOf field could not be relation field.");
                    }
                    if (!pi.IsComposedOf && (pi.FieldType == FieldType.Normal || pi.FieldType == FieldType.LazyLoad))
                    {
                        throw new DataException("The property '{0}' should define as relation field and can not set lazy load attribute", pi.PropertyDefinition.Name);
                    }
                }
                ProcessProperty(pi);
            }
        }

        private void ProcessConstructors()
        {
            foreach (var c in _model.Methods)
            {
                if (c.IsConstructor)
                {
                    ProcessConstructor(c);
                }
            }
        }

        private void ProcessConstructor(MethodDefinition constructor)
        {
            var processor = new IlBuilder(constructor.Body);
            foreach (var pi in _properties)
            {
                if(pi.FieldType != FieldType.Normal)
                {
                    ProcessGenericPropertyInConstructor(pi, processor);
                }
            }
            if (!_model.IsAbstract)
            {
                processor.LoadArg(0).Call(_handler.InitUpdateColumns);
            }
            var target = GetCallBaseCtor(constructor);
            processor.InsertAfter(target);
        }

        private void ProcessGenericPropertyInConstructor(PropertyInformation pi, IlBuilder processor)
        {
            processor.LoadArg(0).LoadArg(0);
            MethodReference ci1;
            var ft = (GenericInstanceType)pi.FieldDefinition.FieldType;
            if (pi.IsHasOne || pi.IsHasMany || pi.IsHasAndBelongsToMany)
            {
                ci1 = ft.GetConstructor(typeof(object), typeof(string));
                var ob = GetOrderByString(pi);
                if (string.IsNullOrEmpty(ob))
                {
                    processor.LoadNull();
                }
                else
                {
                    processor.LoadString(ob);
                }
            }
            else
            {
                ci1 = ft.GetConstructor(typeof(object));
            }
            var ctor = _handler.Import(ci1);
            ctor.DeclaringType = ft; //NOTE: might be a bug of Cecil
            processor.NewObj(ctor);
            processor.SetField(pi.FieldDefinition);
        }

        private static string GetOrderByString(PropertyInformation pi)
        {
            var pd = pi.PropertyDefinition;
            CustomAttribute ca;
            switch (pi.FieldType)
            {
                case FieldType.HasOne:
                    ca = pd.GetCustomAttribute(KnownTypesHandler.HasOneAttribute);
                    break;
                case FieldType.HasMany:
                    ca = pd.GetCustomAttribute(KnownTypesHandler.HasManyAttribute);
                    break;
                case FieldType.HasAndBelongsToMany:
                    ca = pd.GetCustomAttribute(KnownTypesHandler.HasAndBelongsToManyAttribute);
                    break;
                default:
                    throw new ApplicationException("Impossiable");
            }
            var value = (string)ca.GetField("OrderBy");
            return value;
        }

        private static Instruction GetCallBaseCtor(MethodDefinition constructor)
        {
            foreach (var instruction in constructor.Body.Instructions)
            {
                if(instruction.OpCode.Code == Code.Call)
                {
                    var ctor = (MethodReference)instruction.Operand;
                    if(ctor.Name == ".ctor")
                    {
                        return instruction;
                    }
                }
            }
            throw new ApplicationException("Can not find base ctor call");
        }

        private List<PropertyInformation> GetProperties()
        {
            var result = new List<PropertyInformation>();
            foreach (PropertyDefinition pi in _model.Properties)
            {
                if(pi.SetMethod != null && pi.GetMethod != null 
                    && pi.SetMethod.IsCompilerGenerated() 
                    && pi.GetMethod.IsCompilerGenerated())
                {
                    var ft = KnownTypesHandler.GetFieldType(pi);
                    var ico = KnownTypesHandler.IsComposedOf(pi);
                    result.Add(new PropertyInformation {PropertyDefinition = pi, FieldType = ft, IsComposedOf = ico});
                }
            }
            return result;
        }

        private void ProcessProperty(PropertyInformation pi)
        {
            DefineField(pi);
            _model.Fields.Add(pi.FieldDefinition);

            if(pi.IsComposedOf)
            {
                ProcessComposedOfAttribute(pi);
            }

            ProcessPropertyGet(pi);
            ProcessPropertySet(pi);
        }

        private void ProcessComposedOfAttribute(PropertyInformation pi)
        {
            var composedOf = pi.PropertyDefinition.PropertyType.Resolve();
            var gen = new ComposedOfClassGenerator(_model, composedOf, _handler);
            gen.Generate();

            foreach(var attribute in pi.PropertyDefinition.CustomAttributes)
            {
                if(attribute.AttributeType.FullName == KnownTypesHandler.ComposedOfAttribute)
                {
                    pi.PropertyDefinition.CustomAttributes.Remove(attribute);
                    break;
                }
            }
            pi.PropertyDefinition.CustomAttributes.Add(_handler.GetExclude());
        }

        private static void RemovePropertyCompilerGeneratedAttribute(MethodDefinition method)
        {
            foreach (var attribute in method.CustomAttributes)
            {
                if (attribute.Constructor.DeclaringType.FullName == KnownTypesHandler.CompilerGeneratedAttribute)
                {
                    method.CustomAttributes.Remove(attribute);
                    return;
                }
            }
        }

        private static IlBuilder PreProcessPropertyMethod(MethodDefinition method)
        {
            RemovePropertyCompilerGeneratedAttribute(method);
            method.Body.Instructions.Clear();
            return new IlBuilder(method.Body);
        }

        private void ProcessPropertyGet(PropertyInformation pi)
        {
            var processor = PreProcessPropertyMethod(pi.PropertyDefinition.GetMethod);
            processor.LoadArg(0);
            processor.LoadField(pi.FieldDefinition);
            if (pi.FieldType == FieldType.BelongsTo || pi.FieldType == FieldType.HasOne || pi.FieldType == FieldType.LazyLoad)
            {
                var getValue = _handler.Import(pi.FieldDefinition.FieldType.GetMethod("get_Value"));
                processor.CallVirtual(getValue);
            }
            processor.Return();
            processor.Append();
        }

        private void ProcessPropertySet(PropertyInformation pi)
        {
            var processor = PreProcessPropertyMethod(pi.PropertyDefinition.SetMethod);
            var exclude = pi.PropertyDefinition.GetCustomAttribute(KnownTypesHandler.ExcludeAttribute);
            if(exclude != null)
            {
                ProcessPropertySetExclude(pi, processor);
                return;
            }
            switch (pi.FieldType)
            {
                case FieldType.Normal:
                    ProcessPropertySetNormal(pi, processor);
                    ProcessPropertySetCallUpdateColumn(pi, processor);
                    break;
                case FieldType.HasOne:
                    ProcessPropertySetHasOneBelongsToLazyLoad(pi, processor);
                    break;
                case FieldType.BelongsTo:
                    ProcessPropertySetHasOneBelongsToLazyLoad(pi, processor);
                    break;
                case FieldType.LazyLoad:
                    ProcessPropertySetHasOneBelongsToLazyLoad(pi, processor);
                    ProcessPropertySetCallUpdateColumn(pi, processor);
                    break;
                default:
                    ProcessPropertySetElse(processor);
                    break;
            }
        }

        private static void ProcessPropertySetExclude(PropertyInformation pi, IlBuilder processor)
        {
            processor.LoadArg(0);
            processor.LoadArg(1);
            processor.SetField(pi.FieldDefinition);
            processor.Return();
            processor.Append();
        }

        private void ProcessPropertySetNormal(PropertyInformation pi, IlBuilder processor)
        {
            processor.Return();
            processor.Append();
            ProcessPropertySetNormalCompare(pi, processor);
            processor.LoadArg(0);
            processor.LoadArg(1);
            processor.SetField(pi.FieldDefinition);
            processor.InsertBefore(pi.PropertyDefinition.SetMethod.Body.Instructions.Last());
        }

        private void ProcessPropertySetHasOneBelongsToLazyLoad(PropertyInformation pi, IlBuilder processor)
        {
            processor.LoadArg(0);
            processor.LoadField(pi.FieldDefinition);
            processor.LoadArg(1);
            var sv = pi.FieldDefinition.FieldType.GetMethod("set_Value");
            var setValue = _handler.Import(sv);
            processor.CallVirtual(setValue);
            processor.Return();
            processor.Append();
        }

        private static void ProcessPropertySetElse(IlBuilder processor)
        {
            processor.Return();
            processor.Append();
        }

        private void ProcessPropertySetCallUpdateColumn(PropertyInformation pi, IlBuilder processor)
        {
            processor.LoadArg(0);
            processor.LoadString(pi.ColumnName);
            processor.Call(_handler.ColumnUpdated);
            processor.InsertBefore(pi.PropertyDefinition.SetMethod.Body.Instructions.Last());
        }

        private void ProcessPropertySetNormalCompare(PropertyInformation pi, IlBuilder processor)
        {
            if(pi.PropertyDefinition.PropertyType.IsValueType)
            {
                if(pi.PropertyDefinition.PropertyType.IsGenericInstance)
                {
                    ProcessPropertySetNormalCompareGeneric(pi, processor);
                }
                else
                {
                    ProcessPropertySetNormalCompareValueType(pi, processor);
                }
            }
            else if(pi.PropertyDefinition.PropertyType.FullName == KnownTypesHandler.String)
            {
                ProcessPropertySetNormalCompareString(pi, processor);
            }
        }

        private void ProcessPropertySetNormalCompareValueType(PropertyInformation pi, IlBuilder processor)
        {
            processor.DeclareLocal(_handler.BoolType);
            processor.LoadArg(1).LoadArg(0).LoadField(pi.FieldDefinition);
            if (_handler.TypeDict.ContainsKey(pi.PropertyDefinition.PropertyType.FullName))
            {
                var mi = _handler.TypeDict[pi.PropertyDefinition.PropertyType.FullName];
                processor.Call(mi).LoadInt(0);
            }
            processor.Ceq().SetLoc(0).LoadLoc(0);
            processor.BrTrue_S(pi.PropertyDefinition.SetMethod.Body.Instructions.Last());
        }

        private void ProcessPropertySetNormalCompareString(PropertyInformation pi, IlBuilder processor)
        {
            processor.DeclareLocal(_handler.BoolType);
            processor.LoadArg(1);
            processor.LoadArg(0);
            processor.LoadField(pi.FieldDefinition);
            var mi = _handler.TypeDict[pi.PropertyDefinition.PropertyType.FullName];
            processor.Call(mi);
            processor.LoadInt(0);
            processor.Ceq();
            processor.SetLoc(0);
            processor.LoadLoc(0);
            processor.BrTrue_S(pi.PropertyDefinition.SetMethod.Body.Instructions.Last());
        }

        private void ProcessPropertySetNormalCompareGeneric(PropertyInformation pi, IlBuilder processor)
        {
            processor.LoadInt(1);
            processor.LoadInt(0);
            var l1 = processor.Instructions[0];
            var l2 = processor.Instructions[1];
            processor.Instructions.Clear();

            var getValueOrDefault = _handler.Import(pi.PropertyDefinition.PropertyType.GetMethod("GetValueOrDefault"));
            var getHasValue = _handler.Import(pi.PropertyDefinition.PropertyType.GetMethod("get_HasValue"));
            var type = (GenericInstanceType) pi.PropertyDefinition.PropertyType;
            var realType = type.GenericArguments[0].FullName;

            var v0 = processor.DeclareLocal(pi.PropertyDefinition.PropertyType);
            var v1 = processor.DeclareLocal(pi.PropertyDefinition.PropertyType);
            processor.DeclareLocal(_handler.BoolType);
            processor.LoadArg(1).SetLoc(0).LoadArg(0).LoadField(pi.FieldDefinition).SetLoc(1).LoadLocala_S(v0);

            if (realType == KnownTypesHandler.Decimal)
            {
                processor.Call(getValueOrDefault).LoadLocala_S(v1).Call(getValueOrDefault)
                    .Call(_handler.TypeDict[realType]);
                processor.BrTrue_S(l1).LoadLocala_S(v0).Call(getHasValue).LoadLocala_S(v1).Call(getHasValue);
                processor.Ceq().LoadInt(0).Ceq().Br_S(l2);
            }
            else if (_handler.TypeDict.ContainsKey(realType))
            {
                processor.LoadInt(0).Br_S(l2);
                var l4 = processor.Instructions[processor.Instructions.Count - 2];
                var l3 = processor.Instructions[processor.Instructions.Count - 1];
                processor.Instructions.Remove(l3);
                processor.Instructions.Remove(l4);
                processor.Call(getHasValue).LoadLocala_S(v1).Call(getHasValue).Bne_Un_S(l1);
                processor.LoadLocala_S(v0).Call(getHasValue).BrFalse_S(l4).LoadLocala_S(v0);
                processor.Call(getValueOrDefault).LoadLocala_S(v1).Call(getValueOrDefault)
                    .Call(_handler.TypeDict[realType]).Br_S(l3);
                processor.Instructions.Add(l4);
                processor.Instructions.Add(l3);
            }
            else
            {
                processor.Call(getValueOrDefault);
                processor.ConvFloaty(realType);
                processor.LoadLocala_S(v1).Call(getValueOrDefault);
                processor.ConvFloaty(realType);
                processor.Bne_Un_S(l1).LoadLocala_S(v0).Call(getHasValue).LoadLocala_S(v1).Call(getHasValue);
                processor.Ceq().LoadInt(0).Ceq().Br_S(l2);
            }

            processor.Instructions.Add(l1);
            processor.Instructions.Add(l2);
            processor.Ceq().SetLoc(2).LoadLoc(2);
            processor.BrTrue_S(pi.PropertyDefinition.SetMethod.Body.Instructions.Last());
        }

        private void DefineField(PropertyInformation pi)
        {
            var pd = pi.PropertyDefinition;
            var name = MemberPrifix + pd.Name;
            var propertyType = pd.PropertyType;

            if (pi.FieldType == FieldType.Normal)
            {
                pi.FieldDefinition = new FieldDefinition(name, FieldAttributes.Private, propertyType);
                return;
            }

            var t = _handler.GetRealType(pi);
            pi.FieldDefinition = new FieldDefinition(name, FieldAttributes.FamORAssem, t);
            PopulateDbColumn(pi);
            PopulateIndex(pi);
            GenerateCrossTableForHasManyAndBelongsTo(pi);
            PopulateCustomAttributeForLazyLoadColumn(pi);
        }

        private void PopulateDbColumn(PropertyInformation pi)
        {
            var pd = pi.PropertyDefinition;
            var bc = pd.GetCustomAttribute(KnownTypesHandler.DbColumnAttribute);
            if (bc != null)
            {
                pd.CustomAttributes.Remove(bc);
                pi.FieldDefinition.CustomAttributes.Add(bc);
            }
            else if (pi.FieldType == FieldType.LazyLoad)
            {
                var c = _handler.GetDbColumn(pi.PropertyDefinition.Name);
                pi.FieldDefinition.CustomAttributes.Add(c);
            }
        }

        private static void PopulateIndex(PropertyInformation pi)
        {
            var pd = pi.PropertyDefinition;
            var bcs = pd.GetCustomAttributes(KnownTypesHandler.IndexAttribute);
            if (bcs != null && bcs.Count > 0)
            {
                foreach(var bc in bcs)
                {
                    pd.CustomAttributes.Remove(bc);
                    pi.FieldDefinition.CustomAttributes.Add(bc);
                }
            }
        }

        private void GenerateCrossTableForHasManyAndBelongsTo(PropertyInformation pi)
        {
            var pd = pi.PropertyDefinition;
            if (pi.FieldType == FieldType.HasAndBelongsToMany)
            {
                var mm = pd.GetCustomAttribute(KnownTypesHandler.HasAndBelongsToManyAttribute);
                var ctName = (string)mm.GetField("CrossTableName");
                if (!string.IsNullOrEmpty(ctName))
                {
                    var c = _handler.GetCrossTable(ctName);
                    pi.FieldDefinition.CustomAttributes.Add(c);
                }
            }
        }

        private static void PopulateCustomAttributeForLazyLoadColumn(PropertyInformation pi)
        {
            if (pi.FieldType == FieldType.LazyLoad)
            {
                PopulateCustomAttribute(pi, KnownTypesHandler.AllowNullAttribute);
                PopulateCustomAttribute(pi, KnownTypesHandler.LengthAttribute);
                PopulateCustomAttribute(pi, KnownTypesHandler.StringColumnAttribute);
                PopulateCustomAttribute(pi, KnownTypesHandler.IndexAttribute);
                PopulateCustomAttribute(pi, KnownTypesHandler.SpecialNameAttribute);
            }
        }

        private static void PopulateCustomAttribute(PropertyInformation pi, string attributeName)
        {
            var c = pi.PropertyDefinition.GetCustomAttribute(attributeName);
            if(c != null)
            {
                pi.PropertyDefinition.CustomAttributes.Remove(c);
                pi.FieldDefinition.CustomAttributes.Add(c);
            }
        }
    }
}
