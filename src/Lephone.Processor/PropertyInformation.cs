using Lephone.Data.Common;
using Mono.Cecil;

namespace Lephone.Processor
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

        public bool IsHasAndBelongsToMany
        {
            get
            {
                return FieldType == FieldType.HasAndBelongsToMany;
            }
        }
    }
}
