
#region usings

using System;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    [Serializable]
    public abstract class DbObject : DbObjectBase
	{
		[DbKey(UnsavedValue=0L), DbColumn("Id")]
		protected long m_Id = 0;

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
