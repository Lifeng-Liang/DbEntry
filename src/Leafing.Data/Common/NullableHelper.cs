using System;
using System.Collections.Specialized;
using System.Reflection;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Common {
    internal class NullableHelper {
        private static readonly HybridDictionary NullableTypesToDataType;
        private static readonly HybridDictionary NullableTypesToUnderlyingType;

        static NullableHelper() {
            NullableTypesToDataType = new HybridDictionary();
            NullableTypesToDataType[typeof(Nullable<bool>)] = DataType.Boolean;
            NullableTypesToDataType[typeof(Nullable<byte>)] = DataType.Byte;
            NullableTypesToDataType[typeof(Nullable<DateTime>)] = DataType.DateTime;
            NullableTypesToDataType[typeof(Nullable<Date>)] = DataType.Date;
            NullableTypesToDataType[typeof(Nullable<Time>)] = DataType.Time;
            NullableTypesToDataType[typeof(Nullable<decimal>)] = DataType.Decimal;
            NullableTypesToDataType[typeof(Nullable<double>)] = DataType.Double;
            NullableTypesToDataType[typeof(Nullable<float>)] = DataType.Single;
            NullableTypesToDataType[typeof(Nullable<int>)] = DataType.Int32;
            NullableTypesToDataType[typeof(Nullable<long>)] = DataType.Int64;
            NullableTypesToDataType[typeof(Nullable<sbyte>)] = DataType.SByte;
            NullableTypesToDataType[typeof(Nullable<short>)] = DataType.Int16;
            NullableTypesToDataType[typeof(Nullable<uint>)] = DataType.UInt32;
            NullableTypesToDataType[typeof(Nullable<ulong>)] = DataType.UInt64;
            NullableTypesToDataType[typeof(Nullable<ushort>)] = DataType.UInt16;
            NullableTypesToDataType[typeof(Nullable<Guid>)] = DataType.Guid;

            NullableTypesToUnderlyingType = new HybridDictionary();
            NullableTypesToUnderlyingType[typeof(Nullable<bool>)] = typeof(Boolean);
            NullableTypesToUnderlyingType[typeof(Nullable<byte>)] = typeof(Byte);
            NullableTypesToUnderlyingType[typeof(Nullable<DateTime>)] = typeof(DateTime);
            NullableTypesToUnderlyingType[typeof(Nullable<Date>)] = typeof(Date);
            NullableTypesToUnderlyingType[typeof(Nullable<Time>)] = typeof(Time);
            NullableTypesToUnderlyingType[typeof(Nullable<decimal>)] = typeof(Decimal);
            NullableTypesToUnderlyingType[typeof(Nullable<double>)] = typeof(Double);
            NullableTypesToUnderlyingType[typeof(Nullable<float>)] = typeof(Single);
            NullableTypesToUnderlyingType[typeof(Nullable<int>)] = typeof(Int32);
            NullableTypesToUnderlyingType[typeof(Nullable<long>)] = typeof(Int64);
            NullableTypesToUnderlyingType[typeof(Nullable<sbyte>)] = typeof(SByte);
            NullableTypesToUnderlyingType[typeof(Nullable<short>)] = typeof(Int16);
            NullableTypesToUnderlyingType[typeof(Nullable<uint>)] = typeof(UInt32);
            NullableTypesToUnderlyingType[typeof(Nullable<ulong>)] = typeof(UInt64);
            NullableTypesToUnderlyingType[typeof(Nullable<ushort>)] = typeof(UInt16);
            NullableTypesToUnderlyingType[typeof(Nullable<Guid>)] = typeof(Guid);
        }

        public static bool IsNullableType(Type t) {
            return NullableTypesToDataType.Contains(t);
        }

        public static DataType GetDataType(Type nullableType) {
            return (DataType)NullableTypesToDataType[nullableType];
        }

        public static Type GetUnderlyingType(Type nullableType) {
            return (Type)NullableTypesToUnderlyingType[nullableType];
        }

        public static ConstructorInfo GetConstructorInfo(Type nullableType) {
            if (!IsNullableType(nullableType)) {
                throw new ArgumentOutOfRangeException();
            }
            ConstructorInfo ci = nullableType.GetConstructor(
                new[] { GetUnderlyingType(nullableType) });
            return ci;
        }

        public static object CreateNullableObject(Type nullableType, object value) {
            ConstructorInfo ci = GetConstructorInfo(nullableType);
            return ci.Invoke(new[] { value });
        }
    }
}
