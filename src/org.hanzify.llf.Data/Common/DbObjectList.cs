
#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using org.hanzify.llf.Data.Builder.Clause;
using org.hanzify.llf.Data.Common;

#endregion

namespace org.hanzify.llf.Data.Common
{
    [Serializable]
	public class DbObjectList<T> : List<T>
	{
        internal DbObjectList() { }

        public DbObjectList<Tout> OfType<Tout>() where Tout : new()
        {
            if (typeof(T) == typeof(Tout))
            {
                return (DbObjectList<Tout>)((object)this);
            }
            DbObjectList<Tout> ret = new DbObjectList<Tout>();
            foreach (Tout i in (IEnumerable)this)
            {
                ret.Add(i);
            }
            return ret;
        }
	}
}
