using System;
using Lephone.Data.Common;
using Mono.Cecil;

namespace Lephone.CodeGen.Processor
{
    public class ModelMember
    {
        public readonly bool IsDbKey;
        public readonly TypeReference MemberType;
        private MethodReference _constructor;
        public readonly bool AllowNull;
        public readonly IMemberDefinition Member;
        public readonly string Name;
        public readonly bool IsLazyLoad;
        public readonly bool IsHasOne;
        public readonly bool IsHasMany;
        public readonly bool IsHasAndBelongsToMany;
        public readonly bool IsBelongsTo;
        public readonly bool IsDbGenerate;
        public readonly bool IsCreatedOn;
        public readonly bool IsUpdatedOn;
        public readonly bool IsSavedOn;
        public readonly bool IsCount;
        public readonly TypeReference DeclaringType;

        public ModelMember(IMemberDefinition member, TypeReference memberType, FieldType fieldType, TypeReference declaringType)
        {
            Member = member;
            MemberType = memberType;
            DeclaringType = declaringType;

            #region Name

            var column = member.GetDbColumnAttribute();
            if (column != null)
            {
                Name = (string)column.ConstructorArguments[0].Value;
            }
            else
            {
                if (fieldType == FieldType.Normal)
                {
                    Name = Member.Name;
                }
                else
                {
                    var relType = GetFirstGenericArgument();
                    var table = relType.GetCustomAttribute(KnownTypesHandler.DbTableAttribute);
                    Name = table != null ? table.ConstructorArguments[0].Value + "_Id" : relType.Name + "_Id";
                }
            }
            
            #endregion

            #region DbKey

            var key = Member.GetDbKey();
            if (key != null)
            {
                IsDbKey = true;
                foreach (var argument in key.ConstructorArguments)
                {
                    if (argument.Type.FullName == KnownTypesHandler.Bool)
                    {
                        IsDbGenerate = (bool)argument.Value;
                        break;
                    }
                }
            }
            #endregion

            AllowNull = Member.IsAllowNull();

            #region Relations

            switch (fieldType)
            {
                case FieldType.LazyLoad:
                    IsLazyLoad = true;
                    break;
                case FieldType.HasOne:
                    IsHasOne = true;
                    break;
                case FieldType.HasMany:
                    IsHasMany = true;
                    break;
                case FieldType.HasAndBelongsToMany:
                    IsHasAndBelongsToMany = true;
                    break;
                case FieldType.BelongsTo:
                    IsBelongsTo = true;
                    break;
            }

            #endregion

            #region SpecialName

            if (Member.IsSpecialName())
            {
                switch(Member.Name)
                {
                    case "CreatedOn":
                        IsCreatedOn = true;
                        break;
                    case "UpdatedOn":
                        IsUpdatedOn = true;
                        break;
                    case "SavedOn":
                        IsSavedOn = true;
                        break;
                    case "Count":
                        IsCount = true;
                        break;
                    default:
                        throw new ApplicationException();
                }
            }

            #endregion
        }

        public MethodReference Constructor
        {
            get
            {
                if (!MemberType.IsValueType)
                {
                    if (_constructor == null)
                    {
                        _constructor = MemberType.GetConstructor();
                    }
                }
                return _constructor;
            }
        }

        public TypeDefinition GetFirstGenericArgument()
        {
            return (((GenericInstanceType)MemberType)).GenericArguments[0].Resolve();
        }
    }
}
