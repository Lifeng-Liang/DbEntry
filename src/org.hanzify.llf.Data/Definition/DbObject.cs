
#region usings

using System;
using org.hanzify.llf.Data.Common;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    [Serializable]
    public abstract class DbObject : DbObjectBase
	{
		[DbKey, DbColumn("Id")]
		protected internal long m_Id = 0;

		[Exclude]
		public long Id
		{
			get { return m_Id; }
		}

        public DbObject()
        {
        }
    }
}
