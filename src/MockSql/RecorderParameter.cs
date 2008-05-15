using System;
using System.Data.Common;

namespace Lephone.MockSql
{
    public class RecorderParameter : DbParameter
    {
        private string _ParameterName;
        private object _Value;
        private System.Data.DbType _DbType;
        private System.Data.ParameterDirection _Direction;

        public override System.Data.DbType DbType
        {
            get
            {
                return _DbType;
            }
            set
            {
                _DbType = value;
            }
        }

        public override System.Data.ParameterDirection Direction
        {
            get
            {
                return _Direction;
            }
            set
            {
                _Direction = value;
            }
        }

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

        public override string ParameterName
        {
            get
            {
                return _ParameterName;
            }
            set
            {
                _ParameterName = value;
            }
        }

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

        public override string SourceColumn
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

        public override object Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

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
