
#region usings

using System;
using Lephone.Data.Common;

#endregion

namespace Lephone.Data.Definition
{
    [Serializable]
    public abstract class DbObject : DbObjectBase
	{
		private long m_Id = 0;

        [DbKey]
        public long Id
		{
			get { return m_Id; }
            set { m_Id = value; }
		}

        public DbObject()
        {
        }
    }
}
