using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;
using Lephone.Core;
using Mono.Cecil;
using DataException = Lephone.Data.DataException;
using SpecialNameAttribute = Lephone.Data.Definition.SpecialNameAttribute;

namespace Lephone.Processor
{
    public class KnownTypesHandler
    {
        public static readonly string LephoneData = typeof(DbEntry).Assembly.FullName;
        public static readonly string Object = typeof(object).FullName;
        public static readonly string String = typeof(string).FullName;
        public static readonly string Decimal = typeof(decimal).FullName;
        public static readonly string Bool = typeof(bool).FullName;
        public static readonly string Guid = typeof(Guid).FullName;
        public static readonly string DbObjectSmartUpdate = typeof(DbObjectSmartUpdate).FullName;
        public static readonly string DbColumnAttribute = typeof(DbColumnAttribute).FullName;
        public static readonly string DbTableAttribute = typeof(DbTableAttribute).FullName;
        public static readonly string HasOneAttribute = typeof(HasOneAttribute).FullName;
        public static readonly string BelongsToAttribute = typeof(BelongsToAttribute).FullName;
        public static readonly string HasManyAttribute = typeof(HasManyAttribute).FullName;
        public static readonly string HasAndBelongsToManyAttribute = typeof(HasAndBelongsToManyAttribute).FullName;
        public static readonly string LazyLoadAttribute = typeof(LazyLoadAttribute).FullName;
        public static readonly string OrderByAttribute = typeof(OrderByAttribute).FullName;
        public static readonly string CompilerGeneratedAttribute = typeof(CompilerGeneratedAttribute).FullName;
        public static readonly string ExcludeAttribute = typeof(ExcludeAttribute).FullName;
        public static readonly string DbKeyAttribute = typeof(DbKeyAttribute).FullName;
        public static readonly string AllowNullAttribute = typeof(AllowNullAttribute).FullName;
        public static readonly string LengthAttribute = typeof(LengthAttribute).FullName;
        public static readonly string StringColumnAttribute = typeof(StringColumnAttribute).FullName;
        public static readonly string IndexAttribute = typeof(IndexAttribute).FullName;
        public static readonly string SpecialNameAttribute = typeof(SpecialNameAttribute).FullName;
        public static readonly string DbObjectInterface = typeof(IDbObject).FullName;
        public static readonly string AssemblyProcessed = typeof(AssemblyProcessed).FullName;
        public static readonly string DbObjectModel1 = typeof(DbObjectModel<>).FullName;
        public static readonly string DbObjectModel2 = typeof(DbObjectModel<,>).FullName;
        public static readonly string ComposedOfAttribute = typeof(ComposedOfAttribute).FullName;

        private static readonly Dictionary<string, FieldType> RelationAttributes;
        private static readonly Dictionary<string, FieldType> Relations;

        public readonly MethodReference ColumnUpdated;
        public readonly MethodReference InitUpdateColumns;
        public readonly TypeReference ModelHandlerBaseType;
        public readonly MethodReference ModelHandlerBaseTypeCtor;
        public readonly MethodReference ModelHandlerBaseTypeGetNullable;
        public readonly MethodReference ModelHandlerBaseTypeNewKeyValue;
        public readonly MethodReference ModelHandlerBaseTypeNewKeyValueDirect;
        public readonly MethodReference ModelHandlerBaseTypeAddKeyValue;

        public readonly MethodReference DictionaryStringObjectAdd;
        public readonly MethodReference KeyValuePairStringStringCtor;
        public readonly MethodReference ListKeyValuePairStringStringAdd;
        public readonly MethodReference KeyValueCollectionAdd;

        private readonly ModuleDefinition _module;

        private readonly TypeReference _hasOne;
        private readonly TypeReference _hasMany;
        private readonly TypeReference _belongsTo;
        private readonly TypeReference _hasAndBelongsToMany;
        private readonly TypeReference _lazyLoadField;

        private readonly TypeReference _string;
        private readonly TypeReference _type;
        private readonly MethodReference _dbColumn;
        private readonly MethodReference _crossTable;
        private readonly MethodReference _modelHandler;
        private readonly MethodReference _assemblyProcessed;
        private readonly MethodReference _exclude;

        public readonly TypeReference ObjectType;
        public readonly TypeReference VoidType;
        public readonly TypeReference BoolType;
        public readonly TypeReference DataReaderInterface;
        public readonly TypeReference DictionaryStringObjectType;
        public readonly TypeReference ListKeyValuePairStringStringType;
        public readonly TypeReference KeyValueCollectionType;
        public readonly TypeReference SerializableInterface;
        public readonly TypeReference SerializationInfoType;
        public readonly TypeReference StreamingContextType;
        public readonly TypeReference AutoValueType;
        public readonly TypeReference DbObjectHandlerInterface;

        public readonly MethodReference ObjectTypeCtor;

        public readonly MethodReference DynamicObjectReferenceSerializeObject;
        public readonly MethodReference SerializableGetObjectData;
        public readonly MethodReference BelongsToInterfaceSetForeignKey;
        public readonly MethodReference LazyLoadingInterfaceInit;

        public readonly MethodReference CreateInstance;
        public readonly MethodReference LoadSimpleValuesByIndex;
        public readonly MethodReference LoadSimpleValuesByName;
        public readonly MethodReference LoadRelationValuesByIndex;
        public readonly MethodReference LoadRelationValuesByName;
        public readonly MethodReference GetKeyValueDirect;
        public readonly MethodReference GetKeyValuesDirect;
        public readonly MethodReference SetValuesForSelectDirect;
        public readonly MethodReference SetValuesForInsertDirect;
        public readonly MethodReference SetValuesForUpdateDirect;

        public readonly Type[] EmptyTypes = new Type[] { };
        public readonly MethodReference DateEx;
        public readonly MethodReference TimeEx;

        private static readonly Dictionary<string, string> DataReaderMethods;
        public readonly Dictionary<string, MethodReference> TypeDict;

        static KnownTypesHandler()
        {
            RelationAttributes = new Dictionary<string, FieldType>
                       {
                           {HasOneAttribute, FieldType.HasOne},
                           {BelongsToAttribute, FieldType.BelongsTo},
                           {HasManyAttribute, FieldType.HasMany},
                           {HasAndBelongsToManyAttribute, FieldType.HasAndBelongsToMany},
                           {LazyLoadAttribute, FieldType.LazyLoad},
                       };
            Relations = new Dictionary<string, FieldType>
                            {
                                {typeof(HasOne<>).FullName, FieldType.HasOne},
                                {typeof(BelongsTo<>).FullName, FieldType.BelongsTo},
                                {typeof(HasMany<>).FullName, FieldType.HasMany},
                                {typeof(HasAndBelongsToMany<>).FullName, FieldType.HasAndBelongsToMany},
                                {typeof(LazyLoadField<>).FullName, FieldType.LazyLoad},
                            };
            DataReaderMethods = new Dictionary<string, string>
                      {
                          {typeof (long).FullName, "GetInt64"},
                          {typeof (int).FullName, "GetInt32"},
                          {typeof (short).FullName, "GetInt16"},
                          {typeof (byte).FullName, "GetByte"},
                          {typeof (bool).FullName, "GetBoolean"},
                          {typeof (DateTime).FullName, "GetDateTime"},
                          {typeof (Date).FullName, "GetDateTime"},
                          {typeof (Time).FullName, "GetDateTime"},
                          {typeof (string).FullName, "GetString"},
                          {typeof (decimal).FullName, "GetDecimal"},
                          {typeof (float).FullName, "GetFloat"},
                          {typeof (double).FullName, "GetDouble"},
                          {typeof (Guid).FullName, "GetGuid"},
                          {typeof (ulong).FullName, "GetInt64"},
                          {typeof (uint).FullName, "GetInt32"},
                          {typeof (ushort).FullName, "GetInt16"}
                      };
        }

        public KnownTypesHandler(ModuleDefinition module)
        {
            this._module = module;
            _hasOne = _module.Import(typeof(HasOne<>));
            _hasMany = _module.Import(typeof(HasMany<>));
            _belongsTo = _module.Import(typeof(BelongsTo<>));
            _hasAndBelongsToMany = _module.Import(typeof(HasAndBelongsToMany<>));
            _lazyLoadField = _module.Import(typeof(LazyLoadField<>));
            _string = _module.Import(typeof(string));
            _type = _module.Import(typeof(Type));
            _dbColumn = Import(Import(typeof(DbColumnAttribute)).GetConstructor(typeof(string)));
            _crossTable = Import(Import(typeof(CrossTableNameAttribute)).GetConstructor(typeof(string)));
            _modelHandler = Import(Import(typeof(ModelHandlerAttribute)).GetConstructor(typeof(Type)));
            _assemblyProcessed = Import(Import(typeof(AssemblyProcessed)).GetConstructor());
            _exclude = Import(Import(typeof(ExcludeAttribute)).GetConstructor());
            var dbase = typeof(DbObjectSmartUpdate);
            ColumnUpdated = _module.Import(dbase.GetMethod("m_ColumnUpdated", ClassHelper.InstanceFlag));
            InitUpdateColumns = _module.Import(dbase.GetMethod("m_InitUpdateColumns", ClassHelper.InstanceFlag));
            var emitBase = typeof(EmitObjectHandlerBase);
            ModelHandlerBaseType = _module.Import(emitBase);
            ModelHandlerBaseTypeCtor = Import(ModelHandlerBaseType.GetConstructor());
            ModelHandlerBaseTypeGetNullable = Import(ModelHandlerBaseType.GetMethod("GetNullable"));
            ModelHandlerBaseTypeNewKeyValue = Import(ModelHandlerBaseType.GetMethod("NewKeyValue"));
            ModelHandlerBaseTypeNewKeyValueDirect = Import(ModelHandlerBaseType.GetMethod("NewKeyValueDirect"));
            ModelHandlerBaseTypeAddKeyValue = Import(ModelHandlerBaseType.GetMethod("AddKeyValue"));
            var ci = typeof(KeyValuePair<string, string>).GetConstructor(new[] { typeof(string), typeof(string) });
            KeyValuePairStringStringCtor = _module.Import(ci);
            ListKeyValuePairStringStringAdd = Import(typeof(List<KeyValuePair<string, string>>).GetMethod("Add"));
            ObjectType = Import(typeof(object));
            VoidType = Import(typeof(void));
            BoolType = Import(typeof(bool));
            DataReaderInterface = Import(typeof(IDataReader));
            DictionaryStringObjectType = Import(typeof(Dictionary<string, object>));
            DictionaryStringObjectAdd = Import(typeof(Dictionary<string, object>).GetMethod("Add"));
            ListKeyValuePairStringStringType = Import(typeof(List<KeyValuePair<string, string>>));
            KeyValueCollectionType = Import(typeof(KeyValueCollection));
            SerializableInterface = Import(typeof(ISerializable));
            SerializationInfoType = Import(typeof(SerializationInfo));
            StreamingContextType = Import(typeof(StreamingContext));
            AutoValueType = Import(typeof(AutoValue));
            DbObjectHandlerInterface = Import(typeof(IDbObjectHandler));

            ObjectTypeCtor = Import(ObjectType.GetConstructor());

            KeyValueCollectionAdd = Import(typeof(KeyValueCollection).GetMethod("Add", new[] {typeof(KeyValue)}));

            DynamicObjectReferenceSerializeObject =
                Import(Import(typeof(DynamicObjectReference)).GetMethod("SerializeObject"));
            SerializableGetObjectData = Import(Import(typeof(ISerializable)).GetMethod("GetObjectData"));
            BelongsToInterfaceSetForeignKey = Import(Import(typeof(IBelongsTo)).GetMethod("set_ForeignKey"));
            LazyLoadingInterfaceInit = Import(Import(typeof(ILazyLoading)).GetMethod("Init"));

            CreateInstance = Import(emitBase.GetMethod("CreateInstance", ClassHelper.AllFlag));
            LoadSimpleValuesByIndex = Import(emitBase.GetMethod("LoadSimpleValuesByIndex", ClassHelper.AllFlag));
            LoadSimpleValuesByName = Import(emitBase.GetMethod("LoadSimpleValuesByName", ClassHelper.AllFlag));
            LoadRelationValuesByIndex = Import(emitBase.GetMethod("LoadRelationValuesByIndex", ClassHelper.AllFlag));
            LoadRelationValuesByName = Import(emitBase.GetMethod("LoadRelationValuesByName", ClassHelper.AllFlag));
            GetKeyValueDirect = Import(emitBase.GetMethod("GetKeyValueDirect", ClassHelper.AllFlag));
            GetKeyValuesDirect = Import(emitBase.GetMethod("GetKeyValuesDirect", ClassHelper.AllFlag));
            SetValuesForSelectDirect = Import(emitBase.GetMethod("SetValuesForSelectDirect", ClassHelper.AllFlag));
            SetValuesForInsertDirect = Import(emitBase.GetMethod("SetValuesForInsertDirect", ClassHelper.AllFlag));
            SetValuesForUpdateDirect = Import(emitBase.GetMethod("SetValuesForUpdateDirect", ClassHelper.AllFlag));

            DateEx = _module.Import(typeof(Date).GetMethod("op_Explicit", new[] { typeof(DateTime) }));
            TimeEx = _module.Import(typeof(Time).GetMethod("op_Explicit", new[] { typeof(DateTime) }));

            TypeDict = new Dictionary<string, MethodReference>();
            var types = new[] { typeof(Date), typeof(Time), typeof(DateTime), typeof(Guid), typeof(TimeSpan), typeof(decimal), typeof(string) };
            foreach (var type in types)
            {
                var mi = _module.Import(type.GetMethod("op_Inequality", ClassHelper.AllFlag));
                TypeDict.Add(type.FullName, mi);
            }
        }

        public MethodReference GetDataReaderMethod(TypeReference t)
        {
            var typeName = t.FullName;
            var drt = _module.Import(typeof(IDataRecord));
            if (DataReaderMethods.ContainsKey(typeName))
            {
                string n = DataReaderMethods[typeName];
                var mi = _module.Import(drt.GetMethod(n));
                return mi;
            }
            if (t.Resolve().IsEnum)
            {
                return _module.Import(drt.GetMethod("GetInt32"));
            }
            return null;
        }

        public MethodReference GetDataReaderMethodInt()
        {
            var drt = _module.Import(typeof(IDataRecord));
            return _module.Import(drt.GetMethod("get_Item", typeof(int)));
        }

        public MethodReference GetDataReaderMethodString()
        {
            var drt = _module.Import(typeof(IDataRecord));
            return _module.Import(drt.GetMethod("get_Item", typeof(string)));
        }

        public static bool IsComposedOf(IMemberDefinition pi)
        {
            foreach (CustomAttribute ca in pi.CustomAttributes)
            {
                var name = ca.Constructor.DeclaringType.FullName;
                if (ComposedOfAttribute == name)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsExclude(IMemberDefinition pi)
        {
            foreach (CustomAttribute ca in pi.CustomAttributes)
            {
                var name = ca.Constructor.DeclaringType.FullName;
                if (ExcludeAttribute == name)
                {
                    return true;
                }
            }
            return false;
        }

        public static FieldType GetFieldType(IMemberDefinition pi)
        {
            foreach (CustomAttribute ca in pi.CustomAttributes)
            {
                var name = ca.Constructor.DeclaringType.FullName;
                if(RelationAttributes.ContainsKey(name))
                {
                    return RelationAttributes[name];
                }
            }
            return FieldType.Normal;
        }

        public static FieldType GetFieldType(FieldDefinition field)
        {
            if(field.FieldType.IsGenericInstance)
            {
                if(field.FieldType is GenericInstanceType)
                {
                    var name = ((GenericInstanceType)field.FieldType).ElementType.FullName;
                    if (Relations.ContainsKey(name))
                    {
                        return Relations[name];
                    }
                }
            }
            return FieldType.Normal;
        }

        public TypeReference GetRealType(PropertyInformation pi)
        {
            var propertyType = pi.PropertyDefinition.PropertyType;
            switch (pi.FieldType)
            {
                case FieldType.HasOne:
                    return TypeHelper.MakeGenericType(_hasOne, propertyType);
                case FieldType.HasMany:
                    return TypeHelper.MakeGenericType(_hasMany, 
                        ((GenericInstanceType)propertyType).GenericArguments[0]);
                case FieldType.BelongsTo:
                    return TypeHelper.MakeGenericType(_belongsTo, propertyType);
                case FieldType.HasAndBelongsToMany:
                    return TypeHelper.MakeGenericType(_hasAndBelongsToMany,
                        ((GenericInstanceType)propertyType).GenericArguments[0]);
                case FieldType.LazyLoad:
                    return TypeHelper.MakeGenericType(_lazyLoadField, propertyType);
                default:
                    throw new DataException("Impossible");
            }
        }

        public MethodReference Import(MethodReference type)
        {
            return _module.Import(type);
        }

        public MethodReference Import(MethodInfo type)
        {
            return _module.Import(type);
        }

        public FieldReference Import(FieldInfo type)
        {
            return _module.Import(type);
        }

        public TypeReference Import(Type type)
        {
            return _module.Import(type);
        }

        public TypeDefinition Import(string fullName)
        {
            var t = _module.GetType(fullName);
            if(t != null)
            {
                return t;
            }
            var type = Type.GetType(fullName);
            return _module.Import(type).Resolve();
        }

        public CustomAttribute GetDbColumn(string name)
        {
            var r = new CustomAttribute(_dbColumn);
            r.ConstructorArguments.Add(new CustomAttributeArgument(_string, name));
            return r;
        }

        public CustomAttribute GetCrossTable(string name)
        {
            var r = new CustomAttribute(_crossTable);
            r.ConstructorArguments.Add(new CustomAttributeArgument(_string, name));
            return r;
        }

        public CustomAttribute GetModelHandler(TypeReference type)
        {
            var r = new CustomAttribute(_modelHandler);
            r.ConstructorArguments.Add(new CustomAttributeArgument(_type, type));
            return r;
        }

        public CustomAttribute GetAssemblyProcessed()
        {
            var c = new CustomAttribute(_assemblyProcessed);
            return c;
        }

        public CustomAttribute GetExclude()
        {
            var c = new CustomAttribute(_exclude);
            return c;
        }
    }
}
