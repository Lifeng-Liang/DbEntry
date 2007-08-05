
#region usings

using System;
using System.Runtime.CompilerServices;
using org.hanzify.llf.Data.Common;

#endregion

[assembly: InternalsVisibleTo(MemoryAssembly.DefaultAssemblyName + ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100ebce81d3bd5481a9f62666c880d1425968d3074786f29f38f5f42ba7d2497ac56456084097085b82f8980304dd9048da30716d8bcfd920a24a4cee2580fd09cecbe40f7eb8e7211e3e3f592f3aba3b38268c99e124525e7a200015e3ee1e061e6f1387ac474577b8023af58c3bbcc790f26b1745b454862ada11213b130097ef")]

namespace org.hanzify.llf.Data.Definition
{
    [Serializable]
    public abstract class DbObject : DbObjectBase
	{
		[DbKey(UnsavedValue=0L), DbColumn("Id")]
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
