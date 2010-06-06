using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Util;
using Mono.Cecil;

namespace Lephone.CodeGen.Processor
{
    public class KnownTypesHandler
    {
        public static readonly string Object = typeof(object).FullName;
        public static readonly string String = typeof(string).FullName;
        public static readonly string DbColumnAttribute = typeof(DbColumnAttribute).FullName;
        public static readonly string HasOneAttribute = typeof(HasOneAttribute).FullName;
        public static readonly string BelongsToAttribute = typeof(BelongsToAttribute).FullName;
        public static readonly string HasManyAttribute = typeof(HasManyAttribute).FullName;
        public static readonly string HasAndBelongsToManyAttribute = typeof(HasAndBelongsToManyAttribute).FullName;
        public static readonly string LazyLoadAttribute = typeof(LazyLoadAttribute).FullName;
        public static readonly string OrderByAttribute = typeof(OrderByAttribute).FullName;
        public static readonly string CompilerGeneratedAttribute = typeof(CompilerGeneratedAttribute).FullName;
        public static readonly string ExcludeAttribute = typeof(ExcludeAttribute).FullName;

        private static readonly Dictionary<string, FieldType> Jar;

        public readonly MethodReference ColumnUpdated;
        public readonly MethodReference InitUpdateColumns;

        private readonly ModuleDefinition _module;

        private readonly TypeReference _hasOne;
        private readonly TypeReference _hasMany;
        private readonly TypeReference _belongsTo;
        private readonly TypeReference _hasAndBelongsToMany;
        private readonly TypeReference _lazyLoadField;

        public KnownTypesHandler(ModuleDefinition module)
        {
            this._module = module;
            _hasOne = _module.Import(typeof(HasOne<>));
            _hasMany = _module.Import(typeof(HasMany<>));
            _belongsTo = _module.Import(typeof(BelongsTo<>));
            _hasAndBelongsToMany = _module.Import(typeof(HasAndBelongsToMany<>));
            _lazyLoadField = _module.Import(typeof(LazyLoadField<>));
            var dbase = typeof(DbObjectSmartUpdate);
            ColumnUpdated = _module.Import(dbase.GetMethod("m_ColumnUpdated", ClassHelper.InstanceFlag));
            InitUpdateColumns = _module.Import(dbase.GetMethod("m_InitUpdateColumns", ClassHelper.InstanceFlag));
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
                    return TypeHelper.MakeGenericType(_hasMany, propertyType.GenericParameters[0]);
                case FieldType.BelongsTo:
                    return TypeHelper.MakeGenericType(_belongsTo, propertyType);
                case FieldType.HasAndBelongsToMany:
                    return TypeHelper.MakeGenericType(_hasAndBelongsToMany, propertyType.GenericParameters[0]);
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
    }
}
