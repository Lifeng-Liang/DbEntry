
#region usings

using System;
using org.hanzify.llf.DemoObject;
using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Common;

#endregion

namespace org.hanzify.llf
{
    public class Program
    {
        static void Main(string[] args)
        {
            Process(DbEntry.Context, "-->> Using Access : ");
            Process(new DbContext(EntryConfig.GetDriver(1)), "-->> Using SqlServer2000 : ");
            Process(new DbContext(EntryConfig.GetDriver(2)), "-->> Using MySql : ");
            Process(new DbContext(EntryConfig.GetDriver(3)), "-->> Using SQLite : ");

            Console.WriteLine("-- The End --");
            Console.ReadLine();
        }

        private static void Process(DbContext ds, string s)
        {
            Console.WriteLine("-->>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>\n{0}", s);
            ds.Create(typeof(Person));
            ds.Create(typeof(Department));
            ds.Insert(new Person(11, true, "Tom"));
            ds.Insert(new Department("Manager"));
            ds.From<Person>().Where(CK.K["Id"] > 3 && CK.K["Id"] < 10).Select();
            Console.WriteLine();
        }
    }
}
