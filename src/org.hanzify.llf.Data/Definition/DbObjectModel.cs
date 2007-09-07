
#region usings

using System;
using System.Collections.Generic;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.QuerySyntax;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    [Serializable]
    public class DbObjectModel<T, TKey> : DbObjectModelBase<T, TKey>
    {
        [DbKey, DbColumn("Id")]
        protected internal TKey m_Id;

        [Exclude]
        public TKey Id
        {
            get { return m_Id; }
        }
    }

    [Serializable]
    public class DbObjectModel<T> : DbObjectModel<T, long>
    {
    }
}
