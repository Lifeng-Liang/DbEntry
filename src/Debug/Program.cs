
using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Data;
using Lephone.Data.Definition;

namespace OrmA
{
    public enum UserRole
    {
        Manager,
        Worker,
        Client
    }

    [Cacheable]
    public abstract class SampleData : DbObjectModel<SampleData>
    {
        [Length(50)]
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
            Console.WriteLine("Cache Sample");
            Console.WriteLine("====================================");
            SampleData d = SampleData.FindById(1);

            Console.WriteLine(d);
            Console.WriteLine();

            d.Name = "test cache";
            d.Save();

            d = SampleData.FindById(1);
            Console.WriteLine(d);
            Console.WriteLine();

            d.Name = "( 1)liang lifeng";
            d.Save();


            try
            {
                DbEntry.UsingTransaction(delegate()
                {
                    // emulate exception of transaction
                    int m = 0;
                    int n = 1 / m;
                });
            }
            catch { }

            d = SampleData.FindById(1);
            Console.WriteLine(d);
            Console.WriteLine();

            Console.ReadLine();
        }
    }
}
