using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;
using Lephone.Util;
using Mono.Cecil;
using DataException = Lephone.Data.DataException;
using SpecialNameAttribute = Lephone.Data.Definition.SpecialNameAttribute;

namespace Lephone.CodeGen.Processor
{
    public class KnownTypesHandler
    {
        public static readonly string Object = typeof(object).FullName;
        public static readonly string String = typeof(string).FullName;
        public static readonly string DbObjectSmartUpdate = typeof(DbObjectSmartUpdate).FullName;
        public static readonly string DbColumnAttribute = typeof(DbColumnAttribute).FullName;
        public static readonly string HasOneAttribute = typeof(HasOneAttribute).FullName;
        public static readonly string BelongsToAttribute = typeof(BelongsToAttribute).FullName;
        public static readonly string HasManyAttribute = typeof(HasManyAttribute).FullName;
        public static readonly string HasAndBelongsToManyAttribute = typeof(HasAndBelongsToManyAttribute).FullName;
        public static readonly string LazyLoadAttribute = typeof(LazyLoadAttribute).FullName;
        public static readonly string OrderByAttribute = typeof(OrderByAttribute).FullName;
        public static readonly string CompilerGeneratedAttribute = typeof(CompilerGeneratedAttribute).FullName;
        public static readonly string ExcludeAttribute = typeof(ExcludeAttribute).FullName;
        public static readonly string AllowNullAttribute = typeof(AllowNullAttribute).FullName;
        public static readonly string LengthAttribute = typeof(LengthAttribute).FullName;
        public static readonly string StringColumnAttribute = typeof(StringColumnAttribute).FullName;
        public static readonly string IndexAttribute = typeof(IndexAttribute).FullName;
        public static readonly string SpecialNameAttribute = typeof(SpecialNameAttribute).FullName;

        private static readonly Dictionary<string, FieldType> Jar;

        public readonly MethodReference ColumnUpdated;
        public readonly MethodReference InitUpdateColumns;
        public readonly TypeReference ModelHandlerBaseType;
        public readonly MethodReference ModelHandlerBaseTypeCtor;

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

        public readonly TypeReference ObjectType;
        public readonly TypeReference VoidType;
        public readonly TypeReference BoolType;
        public readonly TypeReference DataReaderInterface;
        public readonly TypeReference DictionaryStringObjectType;
        public readonly TypeReference ListKeyValuePairStringStringType;
        public readonly TypeReference KeyValueCollectionType;

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
            var dbase = typeof(DbObjectSmartUpdate);
            ColumnUpdated = _module.Import(dbase.GetMethod("m_ColumnUpdated", ClassHelper.InstanceFlag));
            InitUpdateColumns = _module.Import(dbase.GetMethod("m_InitUpdateColumns", ClassHelper.InstanceFlag));
            ModelHandlerBaseType = _module.Import(typeof(EmitObjectHandlerBase));
            ModelHandlerBaseTypeCtor = Import(ModelHandlerBaseType.GetConstructor());
            ObjectType = Import(typeof(object));
            VoidType = Import(typeof(void));
            BoolType = Import(typeof(bool));
            DataReaderInterface = Import(typeof(IDataReader));
            DictionaryStringObjectType = Import(typeof(Dictionary<string, object>));
            ListKeyValuePairStringStringType = Import(typeof(List<KeyValuePair<string, string>>));
            KeyValueCollectionType = Import(typeof(KeyValueCollection));
        }

        static KnownTypesHandler()
        {
            Jar = new Dictionary<string, FieldType>
                       {
                           {HasOneAttribute, FieldType.HasOne},
                           {BelongsToAttribute, FieldType.BelongsTo},
                           {HasManyAttribute, FieldType.HasMany},
                           {HasAndBelongsToManyAttribute, FieldType.HasAndBelongsToMany},
                           {LazyLoadAttribute, FieldType.LazyLoad},
                       };
        }

        public static FieldType GetFieldType(PropertyDefinition pi)
        {
            foreach (CustomAttribute ca in pi.CustomAttributes)
            {
                var name = ca.Constructor.DeclaringType.FullName;
                if(Jar.ContainsKey(name))
                {
                    return Jar[name];
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

        public TypeReference Import(Type type)
        {
            return _module.Import(type);
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
    }
}
