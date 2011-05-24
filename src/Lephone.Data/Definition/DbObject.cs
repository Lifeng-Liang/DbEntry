using System;

namespace Lephone.Data.Definition
{
    [Serializable]
    public abstract class DbObject : DbObjectBase
	{
		private long _id;

        [DbKey]
        public long Id
		{
			get { return _id; }
            set { _id = value; }
		}

        protected DbObject()
        {
        }
    }
}
