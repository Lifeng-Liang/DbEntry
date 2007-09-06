
#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.Data.Definition;

#endregion

namespace Orm9
{
    public enum UserRole
    {
        Manager,
        Worker,
        Client
    }

    public abstract class SampleData : DbObjectModel<SampleData>
    {
        [MaxLength(50)]
        public abstract string Name { get; set; }

        public abstract UserRole Role { get; set; }

        public abstract DateTime JoinDate { get; set; }

        public abstract bool Enabled { get; set; }

        public abstract int? NullInt { get; set; }

        public SampleData() { }

        public SampleData(string Name, UserRole Role, DateTime JoinDate, bool Enabled)
            : this(Name, Role, JoinDate, Enabled, null)
        {
        }

        public SampleData(string Name, UserRole Role, DateTime JoinDate, bool Enabled, int? NullInt)
        {
            this.Name = Name;
            this.Role = Role;
            this.JoinDate = JoinDate;
            this.Enabled = Enabled;
            this.NullInt = NullInt;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Using PagedSelector and PagedCollection:\n");
            IPagedSelector ps = DbEntry
                .From<SampleData>()
                .Where(null)
                .OrderBy("Id")
                .PageSize(2)
                .GetPagedSelector();

            Show("1st page:", new PagedCollection(ps, 0));

            Show("2nd page:", new PagedCollection(ps, 1));

            Show("3rd page:", new PagedCollection(ps, 2));

            Show("last page:", new PagedCollection(ps, 4));

            Console.ReadLine();
        }

        private static void Show(string ShowString, ICollection ic)
        {
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine(ShowString);
            int i = 0;
            foreach (object o in ic)
            {
                Console.Write("({0})", ++i);
                if (o == null)
                {
                    Console.WriteLine("<NULL>");
                }
                else
                {
                    Console.WriteLine(o);
                }
            }
            Console.WriteLine();
        }
    }
}
