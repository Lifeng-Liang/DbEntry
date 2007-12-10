
#region usings

using System;
using System.Data;

#endregion

namespace Lephone.Data.SqlEntry
{
	[Serializable]
	public enum DataType
	{
		String		= DbType.String,
		Date		= DbType.DateTime,
		Boolean		= DbType.Boolean,

		Byte		= DbType.Byte,
		SByte		= DbType.SByte,
		Decimal		= DbType.Decimal,
		Double		= DbType.Double,
		Single		= DbType.Single,

		Int32		= DbType.Int32,
		UInt32		= DbType.UInt32,
		Int64		= DbType.Int64,
		UInt64		= DbType.UInt64,
		Int16		= DbType.Int16,
		UInt16		= DbType.UInt16,

		Guid		= DbType.Guid,
		Binary		= DbType.Binary
	}
}
