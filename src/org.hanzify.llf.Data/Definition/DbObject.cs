
#region usings

using System;
using Lephone.Data.Common;

#endregion

namespace Lephone.Data.Definition
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
