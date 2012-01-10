using Leafing.Data.Common;
using Mono.Cecil;

namespace Leafing.Processor
{
    public class PropertyInformation
    {
        public FieldType FieldType;
        public FieldDefinition FieldDefinition;
        public string ColumnName;
        public bool IsComposedOf { get; set; }
        public bool IsExclude { get; set; }

        private PropertyDefinition _propertyDefinition;

        public PropertyDefinition PropertyDefinition
        {
            get { return _propertyDefinition; }
            set
            {
                ColumnName = value.GetColumnName();
                _propertyDefinition = value;
            }
        }

        public bool IsHasOne
        {
            get
            {
                return FieldType == FieldType.HasOne;
            }
        }

        public bool IsHasMany
        {
            get
            {
                return FieldType == FieldType.HasMany;
            }
        }

        public bool IsBelongsTo
        {
            get
            {
                return FieldType == FieldType.BelongsTo;
            }
        }

        public bool IsHasAndBelongsToMany
        {
            get
            {
                return FieldType == FieldType.HasAndBelongsToMany;
            }
        }

        public bool IsLazyLoad
        {
            get
            {
                return FieldType == FieldType.LazyLoad;
            }
        }

        public bool IsSpecialForeignKey
        {
            get
            {
                return _propertyDefinition.Name.Length > 3 &&
                       _propertyDefinition.Name.EndsWith("Id") &&
                       _propertyDefinition.IsSpecialName();
            }
        }
    }
}
