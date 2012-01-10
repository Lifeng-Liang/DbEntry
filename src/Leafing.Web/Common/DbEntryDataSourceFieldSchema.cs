using System;
using System.Web.UI.Design;

namespace Leafing.Web.Common
{
    public class DbEntryDataSourceFieldSchema : IDataSourceFieldSchema
    {
        public DbEntryDataSourceFieldSchema(string name, Type dataType,
            bool identity, bool primaryKey, bool nullable)
        {
            _name = name;
            _dataType = dataType;
            _identity = identity;
            _primaryKey = primaryKey;
            _nullable = nullable;
        }

        private readonly Type _dataType;

        public Type DataType
        {
            get { return _dataType; }
        }

        private readonly bool _identity;

        public bool Identity
        {
            get { return _identity; }
        }

        private readonly string _name;

        public string Name
        {
            get { return _name; }
        }

        private readonly bool _primaryKey;

        public bool PrimaryKey
        {
            get { return _primaryKey; }
        }

        public bool IsReadOnly
        {
            get { return _identity; }
        }

        public bool IsUnique
        {
            get { return _primaryKey || _identity; }
        }

        public int Length
        {
            get { return 0; }
        }

        private readonly bool _nullable;

        public bool Nullable
        {
            get { return _nullable; }
        }

        public int Precision
        {
            get { return 0; }
        }

        public int Scale
        {
            get { return 0; }
        }
    }
}
