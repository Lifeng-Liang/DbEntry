using System.Data;
using Leafing.Data.Common;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Dialect
{
    public class SqlServer2008 : SqlServer2005
    {
        public SqlServer2008()
        {
            TypeNames[DataType.Date] = "DATE";
            TypeNames[DataType.Time] = "TIME";
        }

        public override IDataReader GetDataReader(IDataReader dr, System.Type returnType)
        {
            return new TimeSpanLessDataReader(dr);
        }
    }
}
