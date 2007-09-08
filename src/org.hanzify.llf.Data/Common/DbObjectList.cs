
#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Common;

#endregion

namespace Lephone.Data.Common
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
