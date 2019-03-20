using System;
using System.Collections.Generic;
using System.Linq;
using Leafing.Data.Common;
using Leafing.Data.Definition;
using Leafing.Data.Model.Member.Adapter;

namespace Leafing.Data.Model.Member {
    public class MemberBoolAttributes {
        public readonly bool Key;
        public readonly bool DbGenerate;
        public readonly bool DbGenerateGuid;
        public readonly bool HasOne;
        public readonly bool HasMany;
        public readonly bool HasAndBelongsToMany;
        public readonly bool BelongsTo;
        public readonly bool LazyLoad;
        public readonly bool StringType;
        public readonly bool Unicode;
        public readonly bool AllowNull;
        public readonly bool CreatedOn;
        public readonly bool UpdatedOn;
        public readonly bool SavedOn;
        public readonly bool LockVersion;
        public readonly bool Count;
        public readonly bool AutoSavedValue;
        public readonly bool SimpleField;
        public readonly bool RelationField;

        internal MemberBoolAttributes(MemberAdapter fi, List<Attribute> attributes) {
            Key = GetIsKey(attributes);
            DbGenerate = GetIsDbGenerate(fi.MemberType, attributes);
            DbGenerateGuid = GetIsDbGenerateGuid(fi.MemberType, attributes);
            HasOne = GetIsRelation(fi.MemberType, typeof(HasOne<>));
            HasMany = GetIsRelation(fi.MemberType, typeof(HasMany<>));
            HasAndBelongsToMany = GetIsRelation(fi.MemberType, typeof(HasAndBelongsToMany<>));
            BelongsTo = GetIsRelation(fi.MemberType, typeof(BelongsTo<,>));
            if (!BelongsTo) {
                BelongsTo = GetIsRelation(fi.MemberType, typeof(BelongsTo<>));
            }
            LazyLoad = GetIsRelation(fi.MemberType, typeof(LazyLoad<>));
            StringType = GetIsStringType(fi.MemberType);
            Unicode = GetIsUnicode(fi, attributes);
            AllowNull = GetIsAllowNull(fi, attributes);
            if (attributes.Any(o => o is SpecialNameAttribute)) {
                CreatedOn = ("CreatedOn" == fi.Name);
                UpdatedOn = ("UpdatedOn" == fi.Name);
                SavedOn = ("SavedOn" == fi.Name);
                LockVersion = ("LockVersion" == fi.Name);
                Count = ("Count" == fi.Name);
                if (CreatedOn || UpdatedOn || SavedOn || LockVersion || Count) {
                    AutoSavedValue = true;
                } else {
                    throw new ModelException(fi, "Can not set as special name.");
                }
            }
            RelationField = (HasOne || HasMany || HasAndBelongsToMany || BelongsTo);
            SimpleField = !(RelationField || LazyLoad);
            CheckValues(fi);
            if (BelongsTo) { AllowNull = true; }
        }

        private void CheckValues(MemberAdapter fi) {
            CheckToThrow(CreatedOn && fi.MemberType != typeof(DateTime), fi, "CreatedOn must be DateTime type or LazyLoad.");
            CheckToThrow(UpdatedOn && fi.MemberType != typeof(DateTime?), fi, "UpdatedOn must be nullable DateTime type or LazyLoad.");
            CheckToThrow(SavedOn && fi.MemberType != typeof(DateTime), fi, "SavedOn must be DateTime type or LazyLoad.");
            CheckToThrow(LockVersion && fi.MemberType != typeof(int), fi, "LockVersion must be int type or LazyLoad.");
            CheckToThrow(Count && fi.MemberType != typeof(int), fi, "Count must be int type or LazyLoad.");
        }

        private static void CheckToThrow(bool isTrue, MemberAdapter fi, string message) {
            if (isTrue) {
                throw new ModelException(fi, message);
            }
        }

        private static bool GetIsAllowNull(MemberAdapter fi, List<Attribute> attributes) {
            if (attributes.Any(o => o is AllowNullAttribute)) {
                if (fi.MemberType.IsValueType) {
                    throw new ModelException(fi, "Don't set AllowNull to a value type field, instead of to use nullable");
                }
                return true;
            }
            if (NullableHelper.IsNullableType(fi.MemberType)) {
                return true;
            }
            return false;
        }

        private static bool GetIsRelation(Type type, Type relType) {
            if (type.IsGenericType) {
                return type.GetGenericTypeDefinition() == relType;
            }
            return false;
        }

        private static bool GetIsKey(List<Attribute> attributes) {
            return attributes.Any(o => o is DbKeyAttribute);
        }

        private bool GetIsDbGenerate(Type type, List<Attribute> attributes) {
            if (Key) {
                var dk = (DbKeyAttribute)attributes.First(o => o is DbKeyAttribute);
                return dk.IsDbGenerate && type != typeof(Guid);
            }
            return false;
        }

        private bool GetIsDbGenerateGuid(Type type, List<Attribute> attributes) {
            if (Key) {
                var dk = (DbKeyAttribute)attributes.First(o => o is DbKeyAttribute);
                return dk.IsDbGenerate && type == typeof(Guid);
            }
            return false;
        }

        private bool GetIsStringType(Type type) {
            return (type == typeof(string) ||
                    (LazyLoad && type.GetGenericArguments()[0] == typeof(string)));
        }

        private bool GetIsUnicode(MemberAdapter fi, List<Attribute> attributes) {
            var sf = (StringColumnAttribute)attributes.FirstOrDefault(o => o is StringColumnAttribute);
            if (sf != null) {
                if (!StringType) {
                    throw new ModelException(fi, "StringFieldAttribute must set for String type member!");
                }
                return sf.IsUnicode;
            }
            return StringType;
        }
    }
}