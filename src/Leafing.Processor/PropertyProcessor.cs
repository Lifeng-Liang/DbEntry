using Leafing.Data.Common;
using Mono.Cecil;

namespace Leafing.Processor
{
    public class PropertyProcessor
    {
        private const string MemberPrifix = "$";

        private readonly PropertyInformation _pi;
        private readonly TypeDefinition _model;
        private readonly KnownTypesHandler _handler;

        public PropertyProcessor(PropertyInformation pi, TypeDefinition model, KnownTypesHandler handler)
        {
            this._pi = pi;
            this._model = model;
            this._handler = handler;
        }

        public void Process()
        {
            DefineField();
            _model.Fields.Add(_pi.FieldDefinition);

            ProcessPropertyGet();
            ProcessPropertySet();
        }

        private void DefineField()
        {
            var pd = _pi.PropertyDefinition;
            var name = MemberPrifix + pd.Name;
            var propertyType = pd.PropertyType;

            if (_pi.FieldType == FieldType.Normal)
            {
                _pi.FieldDefinition = new FieldDefinition(name, FieldAttributes.Private, propertyType);
                return;
            }

            var t = _handler.GetRealType(_pi);
            _pi.FieldDefinition = new FieldDefinition(name, FieldAttributes.FamORAssem, t);
            PopulateDbColumn();
            PopulateQueryRequired();
            PopulateIndex();
            GenerateCrossTableForHasManyAndBelongsTo();
            PopulateCustomAttributeForLazyLoadColumn();
        }

        private void ProcessPropertyGet()
        {
            var processor = PreProcessPropertyMethod(_pi.PropertyDefinition.GetMethod);
            processor.LoadArg(0);
            processor.LoadField(_pi.FieldDefinition);
            if (_pi.FieldType == FieldType.BelongsTo || _pi.FieldType == FieldType.HasOne || _pi.FieldType == FieldType.LazyLoad)
            {
                var getValue = _handler.Import(_pi.FieldDefinition.FieldType.GetMethod("get_Value"));
                processor.CallVirtual(getValue);
            }
            processor.Return();
            processor.Append();
        }

        private void ProcessPropertySet()
        {
            var processor = PreProcessPropertyMethod(_pi.PropertyDefinition.SetMethod);
            var exclude = _pi.PropertyDefinition.GetCustomAttribute(KnownTypesHandler.ExcludeAttribute);
            if (exclude != null)
            {
                ProcessPropertySetExclude(_pi, processor);
                return;
            }
            switch (_pi.FieldType)
            {
                case FieldType.Normal:
                    ProcessPropertySetNormal(processor);
                    ProcessPropertySetCallUpdateColumn(processor);
                    break;
                case FieldType.HasOne:
                    ProcessPropertySetHasOneBelongsToLazyLoad(processor);
                    break;
                case FieldType.BelongsTo:
                    ProcessPropertySetHasOneBelongsToLazyLoad(processor);
                    break;
                case FieldType.LazyLoad:
                    ProcessPropertySetHasOneBelongsToLazyLoad(processor);
                    ProcessPropertySetCallUpdateColumn(processor);
                    break;
                default:
                    ProcessPropertySetElse(processor);
                    break;
            }
        }

        public static IlBuilder PreProcessPropertyMethod(MethodDefinition method)
        {
            RemovePropertyCompilerGeneratedAttribute(method);
            method.Body.Instructions.Clear();
            return new IlBuilder(method.Body);
        }

        private static void ProcessPropertySetExclude(PropertyInformation pi, IlBuilder processor)
        {
            processor.LoadArg(0);
            processor.LoadArg(1);
            processor.SetField(pi.FieldDefinition);
            processor.Return();
            processor.Append();
        }

        private void ProcessPropertySetNormal(IlBuilder processor)
        {
            processor.Return();
            processor.Append();
            if(ProcessorSettings.AddCompareToSetProperty)
            {
                ProcessPropertySetNormalCompare(processor);
            }
            processor.LoadArg(0);
            processor.LoadArg(1);
            processor.SetField(_pi.FieldDefinition);
            processor.InsertBefore(_pi.PropertyDefinition.SetMethod.Body.Instructions.LastItem());
        }

        private void ProcessPropertySetHasOneBelongsToLazyLoad(IlBuilder processor)
        {
            processor.LoadArg(0);
            processor.LoadField(_pi.FieldDefinition);
            processor.LoadArg(1);
            var sv = _pi.FieldDefinition.FieldType.GetMethod("set_Value");
            var setValue = _handler.Import(sv);
            processor.CallVirtual(setValue);
            processor.Return();
            processor.Append();
        }

        public static void ProcessPropertySetElse(IlBuilder processor)
        {
            processor.Return();
            processor.Append();
        }

        private void ProcessPropertySetCallUpdateColumn(IlBuilder processor)
        {
            processor.LoadArg(0);
            processor.LoadString(_pi.ColumnName);
            processor.Call(_handler.ColumnUpdated);
            processor.InsertBefore(_pi.PropertyDefinition.SetMethod.Body.Instructions.LastItem());
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

        private void ProcessPropertySetNormalCompare(IlBuilder processor)
        {
            if (_pi.PropertyDefinition.PropertyType.IsValueType)
            {
                if (_pi.PropertyDefinition.PropertyType.IsGenericInstance)
                {
                    ProcessPropertySetNormalCompareGeneric(processor);
                }
                else
                {
                    ProcessPropertySetNormalCompareValueType(processor);
                }
            }
            else if (_pi.PropertyDefinition.PropertyType.FullName == KnownTypesHandler.String)
            {
                ProcessPropertySetNormalCompareString(processor);
            }
        }

        private void ProcessPropertySetNormalCompareValueType(IlBuilder processor)
        {
            processor.DeclareLocal(_handler.BoolType);
            processor.LoadArg(1).LoadArg(0).LoadField(_pi.FieldDefinition);
            if (_handler.TypeDict.ContainsKey(_pi.PropertyDefinition.PropertyType.FullName))
            {
                var mi = _handler.TypeDict[_pi.PropertyDefinition.PropertyType.FullName];
                processor.Call(mi).LoadInt(0);
            }
            processor.Ceq().SetLoc(0).LoadLoc(0);
            processor.BrTrue_S(_pi.PropertyDefinition.SetMethod.Body.Instructions.LastItem());
        }

        private void ProcessPropertySetNormalCompareString(IlBuilder processor)
        {
            processor.DeclareLocal(_handler.BoolType);
            processor.LoadArg(1);
            processor.LoadArg(0);
            processor.LoadField(_pi.FieldDefinition);
            var mi = _handler.TypeDict[_pi.PropertyDefinition.PropertyType.FullName];
            processor.Call(mi);
            processor.LoadInt(0);
            processor.Ceq();
            processor.SetLoc(0);
            processor.LoadLoc(0);
            processor.BrTrue_S(_pi.PropertyDefinition.SetMethod.Body.Instructions.LastItem());
        }

        private void ProcessPropertySetNormalCompareGeneric(IlBuilder processor)
        {
            processor.LoadInt(1);
            processor.LoadInt(0);
            var l1 = processor.Instructions[0];
            var l2 = processor.Instructions[1];
            processor.Instructions.Clear();

            var getValueOrDefault = _handler.Import(_pi.PropertyDefinition.PropertyType.GetMethod("GetValueOrDefault"));
            var getHasValue = _handler.Import(_pi.PropertyDefinition.PropertyType.GetMethod("get_HasValue"));
            var type = (GenericInstanceType)_pi.PropertyDefinition.PropertyType;
            var realType = type.GenericArguments[0].FullName;

            var v0 = processor.DeclareLocal(_pi.PropertyDefinition.PropertyType);
            var v1 = processor.DeclareLocal(_pi.PropertyDefinition.PropertyType);
            processor.DeclareLocal(_handler.BoolType);
            processor.LoadArg(1).SetLoc(0).LoadArg(0).LoadField(_pi.FieldDefinition).SetLoc(1).LoadLocala_S(v0);

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
            processor.BrTrue_S(_pi.PropertyDefinition.SetMethod.Body.Instructions.LastItem());
        }

        private void PopulateDbColumn()
        {
            var pd = _pi.PropertyDefinition;
            var bc = pd.GetCustomAttribute(KnownTypesHandler.DbColumnAttribute);
            if (bc != null)
            {
                pd.CustomAttributes.Remove(bc);
                _pi.FieldDefinition.CustomAttributes.Add(bc);
            }
            else if (_pi.FieldType == FieldType.LazyLoad)
            {
                var c = _handler.GetDbColumn(_pi.PropertyDefinition.Name);
                _pi.FieldDefinition.CustomAttributes.Add(c);
            }
        }

        private void PopulateQueryRequired()
        {
            var pd = _pi.PropertyDefinition;
            var bc = pd.GetCustomAttribute(KnownTypesHandler.QueryRequiredAttribute);
            if (bc != null)
            {
                pd.CustomAttributes.Remove(bc);
                _pi.FieldDefinition.CustomAttributes.Add(bc);
            }
        }

        private void PopulateIndex()
        {
            var pd = _pi.PropertyDefinition;
            var bcs = pd.GetCustomAttributes(KnownTypesHandler.IndexAttribute);
            if (bcs != null && bcs.Count > 0)
            {
                foreach (var bc in bcs)
                {
                    pd.CustomAttributes.Remove(bc);
                    _pi.FieldDefinition.CustomAttributes.Add(bc);
                }
            }
        }

        private void GenerateCrossTableForHasManyAndBelongsTo()
        {
            var pd = _pi.PropertyDefinition;
            if (_pi.FieldType == FieldType.HasAndBelongsToMany)
            {
                var mm = pd.GetCustomAttribute(KnownTypesHandler.HasAndBelongsToManyAttribute);
                var ctName = (string)mm.GetField("CrossTableName");
                if (!string.IsNullOrEmpty(ctName))
                {
                    var c = _handler.GetCrossTable(ctName);
                    _pi.FieldDefinition.CustomAttributes.Add(c);
                }
            }
        }

        private void PopulateCustomAttributeForLazyLoadColumn()
        {
            if (_pi.FieldType == FieldType.LazyLoad)
            {
                PopulateCustomAttribute(KnownTypesHandler.AllowNullAttribute);
                PopulateCustomAttribute(KnownTypesHandler.LengthAttribute);
                PopulateCustomAttribute(KnownTypesHandler.StringColumnAttribute);
                PopulateCustomAttribute(KnownTypesHandler.IndexAttribute);
                PopulateCustomAttribute(KnownTypesHandler.SpecialNameAttribute);
            }
        }

        private void PopulateCustomAttribute(string attributeName)
        {
            var c = _pi.PropertyDefinition.GetCustomAttribute(attributeName);
            if (c != null)
            {
                _pi.PropertyDefinition.CustomAttributes.Remove(c);
                _pi.FieldDefinition.CustomAttributes.Add(c);
            }
        }
    }
}
