using System;
using System.Data.Common;

namespace Lephone.MockSql
{
    public class RecorderParameter : DbParameter
    {
        public override System.Data.DbType DbType { get; set; }

        public override System.Data.ParameterDirection Direction { get; set; }

        public override bool IsNullable
        {
            get
            {
                throw RecorderFactory.NotImplemented;
            }
            set
            {
                throw RecorderFactory.NotImplemented;
            }
        }

        public override string ParameterName { get; set; }

        public override void ResetDbType()
        {
            throw RecorderFactory.NotImplemented;
        }

        public override int Size
        {
            get
            {
                throw RecorderFactory.NotImplemented;
            }
            set
            {
                throw RecorderFactory.NotImplemented;
            }
        }

        public override string SourceColumn { get; set; }

        public override bool SourceColumnNullMapping
        {
            get
            {
                throw RecorderFactory.NotImplemented;
            }
            set
            {
                throw RecorderFactory.NotImplemented;
            }
        }

        public override System.Data.DataRowVersion SourceVersion
        {
            get
            {
                throw RecorderFactory.NotImplemented;
            }
            set
            {
                throw RecorderFactory.NotImplemented;
            }
        }

        public override object Value { get; set; }

        public override string ToString()
        {
            string s = string.Format("{0}={1}:{2}",
                this.ParameterName,
                this.Value == DBNull.Value ? "<NULL>" : this.Value,
                this.DbType);
            return s;
        }
    }
}
