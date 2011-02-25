using System.Collections.Generic;
using System.Web.UI.Design;
using Lephone.Data.Model;
using Lephone.Data.Model.Member;

namespace Lephone.Web.Common
{
    public class DbEntryDataSourceViewSchema : IDataSourceViewSchema
    {
        private readonly ObjectInfo _oi;

        public DbEntryDataSourceViewSchema(ObjectInfo oi)
        {
            this._oi = oi;
        }

        public IDataSourceViewSchema[] GetChildren()
        {
            return null;
        }

        public IDataSourceFieldSchema[] GetFields()
        {
            var list = new List<DbEntryDataSourceFieldSchema>();
            foreach (MemberHandler mh in _oi.SimpleMembers)
            {
                var s = new DbEntryDataSourceFieldSchema(
                    mh.MemberInfo.Name,
                    mh.MemberType,
                    mh.Is.DbGenerate,
                    mh.Is.Key,
                    mh.Is.AllowNull
                    );
                list.Add(s);
            }
            return list.ToArray();
        }

        public string Name
        {
            get { return "DbEntry_" + _oi.HandleType.Name; }
        }
    }
}
