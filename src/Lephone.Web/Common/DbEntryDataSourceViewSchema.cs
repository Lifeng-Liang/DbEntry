using System.Collections.Generic;
using System.Web.UI.Design;
using Lephone.Data.Common;

namespace Lephone.Web.Common
{
    public class DbEntryDataSourceViewSchema : IDataSourceViewSchema
    {
        private readonly ObjectInfo oi;

        public DbEntryDataSourceViewSchema(ObjectInfo oi)
        {
            this.oi = oi;
        }

        public IDataSourceViewSchema[] GetChildren()
        {
            return null;
        }

        public IDataSourceFieldSchema[] GetFields()
        {
            var list = new List<DbEntryDataSourceFieldSchema>();
            foreach (MemberHandler mh in oi.SimpleFields)
            {
                var s = new DbEntryDataSourceFieldSchema(
                    mh.MemberInfo.Name,
                    mh.FieldType,
                    mh.IsDbGenerate,
                    mh.IsKey,
                    mh.AllowNull
                    );
                list.Add(s);
            }
            return list.ToArray();
        }

        public string Name
        {
            get { return "DbEntry_" + oi.HandleType.Name; }
        }
    }
}
