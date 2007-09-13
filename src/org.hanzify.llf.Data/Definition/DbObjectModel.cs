
#region usings

using System;
using System.Collections.Generic;
using Lephone.Data.Common;
using Lephone.Data.QuerySyntax;

#endregion

namespace Lephone.Data.Definition
{
    [Serializable]
    public class DbObjectModel<T, TKey> : DbObjectModelBase<T, TKey>
    {
        private TKey m_Id;

        [DbKey]
        public TKey Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
    }

    [Serializable]
    public class DbObjectModel<T> : DbObjectModel<T, long>
    {
    }
}
