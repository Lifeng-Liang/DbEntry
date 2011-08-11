using System;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Lephone.Processor
{
    public class ModelProcessor
    {
        private const MethodAttributes FlagGetObjectData
            = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot
              | MethodAttributes.Virtual | MethodAttributes.Final;
        private readonly TypeDefinition _model;
        private readonly KnownTypesHandler _handler;
        private readonly List<PropertyInformation> _properties;

        private readonly List<KeyValuePair<TypeDefinition, FieldDefinition>> _coTypes = new List<KeyValuePair<TypeDefinition, FieldDefinition>>();

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
                if (!field.IsStatic && field.IsCompilerGenerated())
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
                CheckProperty(pi);
                ProcessProperty(pi);
            }
            foreach(var property in _properties)
            {
                if(property.IsSpecialForeignKey)
                {
                    ProcessSpecialForeignKey(property);
                }
            }
        }

        private void CheckProperty(PropertyInformation pi)
        {
            CheckNormalProperty(pi);
            CheckComposedOfProperty(pi);
            CheckPropertyAccessable(pi);
        }

        private void CheckNormalProperty(PropertyInformation pi)
        {
            var t = pi.PropertyDefinition.PropertyType;
            if (!t.IsValueType && !t.IsArray && t.FullName != KnownTypesHandler.String)
            {
                if (!pi.IsExclude && !pi.IsComposedOf && (pi.FieldType == FieldType.Normal || pi.FieldType == FieldType.LazyLoad))
                {
                    Throw(pi, "The property '{0}' should define as relation field and can not set lazy load attribute");
                }
            }
        }

        private static void CheckPropertyAccessable(PropertyInformation pi)
        {
            if(pi.IsSpecialForeignKey)
            {
                CheckIsPublicGetAndPrivateSet(pi);
            }
            else if(pi.FieldType == FieldType.HasMany || pi.FieldType == FieldType.HasAndBelongsToMany)
            {
                if(!pi.PropertyDefinition.PropertyType.Name.StartsWith("IList`1"))
                {
                    Throw(pi, "Property [{0}] should be IList<T> but not");
                }
                CheckIsPublicGetAndPrivateSet(pi);
            }
            else if (!pi.IsComposedOf)
            {
                if (!pi.PropertyDefinition.SetMethod.IsPublic || !pi.PropertyDefinition.GetMethod.IsSpecialName)
                {
                    Throw(pi, "Property [{0}] should have public getter and setter but not");
                }
            }
        }

        private static void CheckIsPublicGetAndPrivateSet(PropertyInformation pi)
        {
            if(pi.PropertyDefinition.SetMethod.IsPublic)
            {
                Throw(pi, "Property [{0}] should have private setter but not");
            }
            if(!pi.PropertyDefinition.GetMethod.IsPublic)
            {
                Throw(pi, "Property [{0}] should have public getter but not");
            }
        }

        private static void Throw(PropertyInformation pi, string stringTemplate)
        {
            throw new DataException(string.Format(stringTemplate, pi.PropertyDefinition.Name));
        }

        private static void CheckComposedOfProperty(PropertyInformation pi)
        {
            if(pi.IsComposedOf)
            {
                CheckIsPublicGetAndPrivateSet(pi);
                var t = pi.PropertyDefinition.PropertyType;
                if (t.IsValueType || t.IsArray || t.FullName == KnownTypesHandler.String)
                {
                    Throw(pi, "Property [{0}] mark as ComposedOf but the type is wrong");
                }
                if (pi.FieldType != FieldType.Normal)
                {
                    Throw(pi, "Property [{0}] could not mark as ComposedOf and relations");
                }
            }
        }

        private void ProcessSpecialForeignKey(PropertyInformation property)
        {
            // process set
            var processor = PropertyProcessor.PreProcessPropertyMethod(property.PropertyDefinition.SetMethod);
            PropertyProcessor.ProcessPropertySetElse(processor);
            // remove field
            _model.Fields.Remove(property.FieldDefinition);
            // process get
            var name = property.PropertyDefinition.Name;
            name = name.Substring(0, name.Length - 2);
            var pi = FindProperty(name);
            processor = PropertyProcessor.PreProcessPropertyMethod(property.PropertyDefinition.GetMethod);
            processor.LoadArg(0);
            processor.LoadField(pi.FieldDefinition);
            processor.CallVirtual(_handler.BelongsToInterfaceGetForeignKey);
            processor.CastOrUnbox(property.PropertyDefinition.PropertyType, _handler);
            processor.Return();
            processor.Append();
            // add exclude
            property.PropertyDefinition.CustomAttributes.Add(_handler.GetExclude());
        }

        private PropertyInformation FindProperty(string name)
        {
            foreach(var property in _properties)
            {
                if(property.PropertyDefinition.Name == name && property.IsBelongsTo)
                {
                    return property;
                }
            }
            throw new DataException("Can not find BelongsTo property named : " + name + " in " + _model.Name);
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
            ProcessComposedOfInit(processor);
            var target = GetCallBaseCtor(constructor);
            processor.InsertAfter(target);
        }

        private void ProcessComposedOfInit(IlBuilder processor)
        {
            foreach(var kv in _coTypes)
            {
                processor.LoadArg(0);
                processor.LoadArg(0);
                processor.NewObj(kv.Key.GetConstructor());
                processor.SetField(kv.Value);
            }
        }

        private void ProcessGenericPropertyInConstructor(PropertyInformation pi, IlBuilder processor)
        {
            processor.LoadArg(0).LoadArg(0);
            MethodReference ci1;
            var ft = (GenericInstanceType)pi.FieldDefinition.FieldType;
            if (pi.IsHasOne || pi.IsHasMany || pi.IsHasAndBelongsToMany)
            {
                ci1 = ft.GetConstructor(typeof(DbObjectSmartUpdate), typeof(string), typeof(string));
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
                ci1 = ft.GetConstructor(typeof(DbObjectSmartUpdate), typeof(string));
            }
            if(pi.IsLazyLoad)
            {
                processor.LoadString(pi.ColumnName);
            }
            else
            {
                processor.Nop().Nop().Nop().Nop().LoadString(pi.FieldDefinition.Name);
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
                    result.Add(new PropertyInformation
                                   {
                                       PropertyDefinition = pi,
                                       FieldType = ft,
                                       IsComposedOf = ico,
                                       IsExclude = KnownTypesHandler.IsExclude(pi)
                                   });
                }
            }
            return result;
        }

        private void ProcessProperty(PropertyInformation pi)
        {
            var pp = new PropertyProcessor(pi, _model, _handler);
            pp.Process();

            if (pi.IsComposedOf)
            {
                ProcessComposedOfAttribute(pi);
            }
        }

        private void ProcessComposedOfAttribute(PropertyInformation pi)
        {
            var gen = new ComposedOfClassGenerator(_model, pi, _handler);
            _coTypes.Add(new KeyValuePair<TypeDefinition, FieldDefinition>(gen.Generate(), pi.FieldDefinition));

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
    }
}
