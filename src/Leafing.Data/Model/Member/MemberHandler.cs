using System;
using System.Collections.Generic;
using Leafing.Core;
using Leafing.Data.Common;
using Leafing.Data.Definition;
using Leafing.Core.Text;
using Leafing.Data.Model.Member.Adapter;

namespace Leafing.Data.Model.Member {
    public class MemberHandler {
        public readonly MemberAdapter MemberInfo;
        public readonly MemberBoolAttributes Is;
        public readonly string Name;
        public readonly object UnsavedValue;
        public readonly int MinLength;
        public readonly int MaxLength;
        public readonly int DecimalPart;
        public readonly string LengthErrorMessage;
        public readonly string Regular;
        public readonly string RegularErrorMessage;
        public readonly string CrossTableName;
        public readonly string OrderByString;
        public readonly string UniqueErrorMessage;
        public readonly string ShowString;
        public readonly string Description;

        public Type MemberType {
            get { return MemberInfo.MemberType; }
        }

        protected MemberHandler(MemberAdapter fi) {
            MemberInfo = fi;
            var attributes = GetAttributes();
            Is = new MemberBoolAttributes(fi, attributes);
            Name = GetName(attributes);
            CrossTableName = GetCrossTableName(attributes);
            UnsavedValue = GetUnsavedValue(attributes);

            var len = GetLength(attributes);
            MinLength = len.Min;
            MaxLength = len.Max;
            LengthErrorMessage = len.ErrorMessage;

            var prec = GetPrecision(attributes);
            MaxLength = prec.IntegralPart;
            DecimalPart = prec.DecimalPart;

            Regular = GetRegular(attributes, false);
            RegularErrorMessage = GetRegular(attributes, true);
            OrderByString = GetOrderByString(attributes);
            UniqueErrorMessage = GetUniqueErrorMessage();
            ShowString = GetShowString(attributes);
            Description = GetDescription(attributes);
        }

        private List<Attribute> GetAttributes() {
            var list = new List<Attribute>();
            var array = MemberInfo.GetMemberInfo().GetCustomAttributes(false);
            foreach (Attribute o in array) {
                list.Add(o);
            }
            return list;
        }

        private PrecisionAttribute GetPrecision(List<Attribute> attributes) {
            if (MemberInfo.MemberType == typeof(decimal) || MemberInfo.MemberType == typeof(decimal?)) {
                var precision = attributes.GetAttribute<PrecisionAttribute>();
                if (precision != null) {
                    return precision;
                }
                return new PrecisionAttribute(16, 2);
            }
            return new PrecisionAttribute(MaxLength, 0);
        }

        private LengthAttribute GetLength(List<Attribute> attributes) {
            var lengthAttribute = attributes.GetAttribute<LengthAttribute>();
            if (lengthAttribute != null) {
                var type = MemberInfo.MemberType;
                if (type.IsGenericType) {
                    var ts = type.GetGenericArguments();
                    if (ts.Length == 1) {
                        type = ts[0];
                    }
                }
                if (type != typeof(string) && type != typeof(byte[])) {
                    throw new ModelException(MemberInfo, "LengthAttribute can only define on string or byte array!");
                }
                return lengthAttribute;
            }
            return new LengthAttribute(0, 0);
        }

        private static string GetRegular(List<Attribute> attributes, bool getMessage) {
            var sf = attributes.GetAttribute<StringColumnAttribute>();
            if (sf != null) {
                return getMessage ? sf.ErrorMessage : sf.Regular;
            }
            return null;
        }

        private string GetUniqueErrorMessage() {
            var indexs = MemberInfo.GetAttributes<IndexAttribute>(false);
            foreach (var index in indexs) {
                if (index.UNIQUE && !string.IsNullOrEmpty(index.UniqueErrorMessage)) {
                    return index.UniqueErrorMessage;
                }
            }
            return null;
        }

        private static string GetDescription(List<Attribute> attributes) {
            var da = attributes.GetAttribute<DescriptionAttribute>();
            return (da != null) ? da.Text : null;
        }

        private string GetShowString(List<Attribute> attributes) {
            var ss = attributes.GetAttribute<ShowStringAttribute>();
            return (ss != null) ? ss.ShowString : Name;
        }

        private static string GetOrderByString(List<Attribute> attributes) {
            var os = attributes.GetAttribute<OrderByAttribute>();
            if (os != null) {
                return os.OrderBy;
            }
            return null;
        }

        private object GetUnsavedValue(List<Attribute> attributes) {
            var dk = attributes.GetAttribute<DbKeyAttribute>();
            if (dk != null) {
                if (dk.UnsavedValue == null && (dk.IsDbGenerate || MemberInfo.MemberType == typeof(Guid))) {
                    return Util.GetEmptyValue(MemberInfo.MemberType,
                        false, "Unknown type of db key must set UnsavedValue");
                }
                return dk.UnsavedValue;
            }
            return null;
        }

        private string GetName(List<Attribute> attributes) {
            var fn = attributes.GetAttribute<DbColumnAttribute>();
            var noDbColumn = (fn == null);
            if (Is.BelongsTo && noDbColumn) {
                Type ot = MemberInfo.MemberType.GetGenericArguments()[0];
                return ObjectInfo.GetObjectFromClause(ot).MainModelName + "_Id";
            }
            if (Is.HasAndBelongsToMany && noDbColumn) {
                Type ot1 = MemberInfo.MemberType.GetGenericArguments()[0];
                return ObjectInfo.GetObjectFromClause(ot1).MainModelName + "_Id";
            }
            return noDbColumn ? MemberInfo.Name : fn.Name;
        }

        private string GetCrossTableName(List<Attribute> attributes) {
            if (Is.HasAndBelongsToMany) {
                var ctn = attributes.GetAttribute<CrossTableNameAttribute>();
                if (ctn != null) {
                    return ctn.Name;
                }
            }
            return null;
        }

        public void SetValue(object obj, object value) {
            if (value == DBNull.Value) {
                MemberInfo.SetValue(obj, null);
            } else {
                InnerSetValue(obj, value);
            }
        }

        protected virtual void InnerSetValue(object obj, object value) {
            if (MemberInfo.MemberType == typeof(Date) && value is DateTime) {
                value = (Date)(DateTime)value;
            } else if (MemberInfo.MemberType == typeof(Time) && value is DateTime) {
                value = (Time)(DateTime)value;
            } else if (MemberInfo.MemberType == typeof(Guid) && value != null && value.GetType() != typeof(Guid)) {
                value = new Guid(value.ToString());
            }
            MemberInfo.SetValue(obj, value);
        }

        public virtual object GetValue(object obj) {
            return MemberInfo.GetValue(obj);
        }

        public static MemberHandler NewObject(MemberAdapter fi) {
            if (fi.MemberType.IsEnum) {
                return new EnumMemberHandler(fi);
            }
            if (fi.MemberType == typeof(bool)) {
                return new BooleanMemberHandler(fi);
            }
            if (NullableHelper.IsNullableType(fi.MemberType)) {
                if (fi.MemberType.GetGenericArguments()[0] == typeof(bool)) {
                    return new NullableBooleanMemberHandler(fi);
                }
                return new NullableMemberHandler(fi);
            }
            return new MemberHandler(fi);
        }

        public override string ToString() {
            return string.Format("{0} ({1})", Name, MemberType.Name);
        }
    }
}