
#region usings

using System;
using System.Collections.Generic;
using System.Text;

using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.Data.Definition;

#endregion

namespace Orm9
{
    public class lzUser1 : DbObject
    {
        public string Name;
        public LazyLoadField<string> Profile;
    }

    class Program
    {
        static void Main(string[] args)
        {
            lzUser1 u = new lzUser1();
            u.Name = "tom";
            u.Profile.Value = "test";
            DbEntry.Save(u);
            Console.WriteLine(u.Id);

            lzUser1 u1 = DbEntry.GetObject<lzUser1>(1);
            DbEntry.Save(u);

            lzUser1 u2 = DbEntry.GetObject<lzUser1>(1);
            Console.WriteLine(u2.Name);
            Console.WriteLine(u2.Profile.Value);
        }
    }
}
