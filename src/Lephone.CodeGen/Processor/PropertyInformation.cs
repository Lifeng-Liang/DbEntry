using Lephone.Data.Common;
using Mono.Cecil;

namespace Lephone.CodeGen.Processor
{
    public class PropertyInformation
    {
        public PropertyDefinition PropertyDefinition;
        public FieldType FieldType;
        public FieldDefinition FieldDefinition;

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
