using System.Data;
using Lephone.Data.Common;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Dialect
{
    public class SqlServer2008 : SqlServer2005
    {
        public SqlServer2008()
        {
            TypeNames[DataType.Date] = "DATE";
            TypeNames[DataType.Time] = "TIME";
        }

        public override IDataReader GetDataReader(IDataReader dr, System.Type ReturnType)
        {
            return new TimeSpanLessDataReader(dr);
        }
    }
}
