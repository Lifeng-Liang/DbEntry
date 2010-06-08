using System;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Common;
using Mono.Cecil;
using Mono.Cecil.Cil;
using FieldAttributes = Mono.Cecil.FieldAttributes;

namespace Lephone.CodeGen.Processor
{
    public class ModelProcessor
    {
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

        private static void ProcessSerializable()
        {
            //TODO: process Serializable
            //TypeAttributes ta = DynamicObjectTypeAttr;
            //Type[] interfaces = null;
            //if (sourceType.IsSerializable)
            //{
            //    ta |= TypeAttributes.Serializable;
            //    interfaces = new[] { typeof(ISerializable) };
            //}
            //if (sourceType.IsSerializable)
            //{
            //    MethodInfo mi = typeof(DynamicObjectReference).GetMethod("SerializeObject", ClassHelper.StaticFlag);
            //    tb.OverrideMethod(ImplFlag, "GetObjectData", typeof(ISerializable), null,
            //                      new[] { typeof(SerializationInfo), typeof(StreamingContext) },
            //                      il => il.LoadArg(0).LoadArg(1).LoadArg(2).Call(mi));
            //}
        }

        private void ProcessProperties()
        {
            foreach (var pi in _properties)
            {
                var t = pi.PropertyDefinition.PropertyType;
                if (!t.IsValueType && !t.IsArray && t.FullName != KnownTypesHandler.String)
                {
                    if (pi.FieldType == FieldType.Normal || pi.FieldType == FieldType.LazyLoad)
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
            var processor = new IlBuilder(constructor.Body.GetILProcessor());
            foreach (var pi in _properties)
            {
                if(pi.FieldType != FieldType.Normal)
                {
                    ProcessGenericPropertyInConstructor(pi, processor);
                }
            }
            processor.LoadArg(0).Call(_handler.InitUpdateColumns);
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
                    result.Add(new PropertyInformation {PropertyDefinition = pi, FieldType = ft});
                }
            }
            return result;
        }

        private void ProcessProperty(PropertyInformation pi)
        {
            DefineField(pi);
            _model.Fields.Add(pi.FieldDefinition);

            ProcessPropertyGet(pi);
            ProcessPropertySet(pi);
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
            return new IlBuilder(method.Body.GetILProcessor());
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
                    ProcessPropertySetElse(pi, processor);
                    break;
            }
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

        private static void ProcessPropertySetHasOneBelongsToLazyLoad(PropertyInformation pi, IlBuilder processor)
        {
            processor.LoadArg(0);
            processor.LoadField(pi.FieldDefinition);
            processor.LoadArg(1);
            var setValue = pi.FieldDefinition.FieldType.GetMethod("set_Value");
            processor.CallVirtual(setValue);
            processor.Return();
            processor.Append();
        }

        private void ProcessPropertySetElse(PropertyInformation pi, IlBuilder processor)
        {
            //TODO: there was a bug of else before, need to be fixed this time.
            processor.LoadArg(0);
            processor.Append();
        }

        private void ProcessPropertySetCallUpdateColumn(PropertyInformation pi, IlBuilder processor)
        {
            processor.LoadArg(0);
            processor.LoadString(pi.PropertyDefinition.GetColumnName());
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
            //TODO: implements property set
            //il.DeclareLocal(typeof(bool));
            //if (TypeDict.ContainsKey(propertyType))
            //{
            //    il.LoadArg(1).LoadArg(0).LoadField(fi);
            //    MethodInfo mi = TypeDict[propertyType];
            //    il.Call(mi).LoadInt(0).Ceq().SetLoc(0).LoadLoc(0).BrTrue_S(label);
            //}
            //else
            //{
            //    il.LoadArg(1).LoadArg(0).LoadField(fi).Ceq();
            //    il.SetLoc(0).LoadLoc(0).BrTrue_S(label);
            //}
        }

        private void ProcessPropertySetNormalCompareString(PropertyInformation pi, IlBuilder processor)
        {
            //TODO: implements property set
            pi.PropertyDefinition.SetMethod.Body.Variables.Add(new VariableDefinition(_handler.BoolType));
            processor.LoadArg(1);
            processor.LoadArg(0);
            processor.LoadField(pi.FieldDefinition);
            //MethodInfo mi = TypeDict[propertyType];
            //il.Call(mi).LoadInt(0).Ceq().SetLoc(0).LoadLoc(0).BrTrue_S(label);
        }

        private void ProcessPropertySetNormalCompareGeneric(PropertyInformation pi, IlBuilder processor)
        {
            //TODO: implements property set
            //var l1 = il.DefineLabel();
            //var l2 = il.DefineLabel();
            //var getValueOrDefault = propertyType.GetMethod("GetValueOrDefault", new Type[] { });
            //var getHasValue = propertyType.GetMethod("get_HasValue", ClassHelper.AllFlag);
            //var realType = propertyType.GetGenericArguments()[0];
            //il.DeclareLocal(propertyType).DeclareLocal(propertyType).DeclareLocal(typeof(bool));
            //il.LoadArg(1).SetLoc(0).LoadArg(0).LoadField(fi).SetLoc(1).LoadLocala_S(0);
            //if (realType == typeof(decimal))
            //{
            //    il.Call(getValueOrDefault).LoadLocala_S(1).Call(getValueOrDefault).Call(TypeDict[realType]);
            //    il.BrTrue_S(l1).LoadLocala_S(0).Call(getHasValue).LoadLocala_S(1).Call(getHasValue);
            //    il.Ceq().LoadInt(0).Ceq().Br_S(l2);
            //}
            //else if (TypeDict.ContainsKey(realType))
            //{
            //    var l3 = il.DefineLabel();
            //    var l4 = il.DefineLabel();
            //    il.Call(getHasValue).LoadLocala_S(1).Call(getHasValue).Bne_Un_S(l1);
            //    il.LoadLocala_S(0).Call(getHasValue).BrFalse_S(l4).LoadLocala_S(0);
            //    il.Call(getValueOrDefault).LoadLocala_S(1).Call(getValueOrDefault).Call(TypeDict[realType]).Br_S(l3);
            //    il.MarkLabel(l4).LoadInt(0);
            //    il.MarkLabel(l3).Br_S(l2);
            //}
            //else
            //{
            //    il.Call(getValueOrDefault);
            //    il.ConvFloaty(realType);
            //    il.LoadLocala_S(1).Call(getValueOrDefault);
            //    il.ConvFloaty(realType);
            //    il.Bne_Un_S(l1).LoadLocala_S(0).Call(getHasValue).LoadLocala_S(1).Call(getHasValue);
            //    il.Ceq().LoadInt(0).Ceq().Br_S(l2);
            //}
            //il.MarkLabel(l1).LoadInt(1);
            //il.MarkLabel(l2).LoadInt(0);
            //il.Ceq().SetLoc(2).LoadLoc(2).BrTrue_S(label);
            
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
