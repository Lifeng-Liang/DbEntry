using System;

namespace Lephone.Data.Definition
{
    [Serializable]
    public abstract class DbObject : DbObjectBase
	{
		private long m_Id;

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
