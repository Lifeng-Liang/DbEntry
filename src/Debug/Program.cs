
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

    public abstract class SampleData : DbObjectModel<SampleData>
    {
        [Length(50)]
        public abstract string Name { get; set; }

        public abstract UserRole Role { get; set; }

        public abstract DateTime JoinDate { get; set; }

        public abstract bool Enabled { get; set; }

        public abstract int? NullInt { get; set; }

        public SampleData() { }

        public SampleData Init(string Name, UserRole Role, DateTime JoinDate, bool Enabled, int? NullInt)
        {
            this.Name = Name;
            this.Role = Role;
            this.JoinDate = JoinDate;
            this.Enabled = Enabled;
            this.NullInt = NullInt;
            return this;
        }
    }

    [DbTable("Shippers")]
    public class Shipper : IDbObject
    {
        [DbColumn("Shipper ID")]
        public int Id;

        [DbColumn("Company Name")]
        public string Name;
    }

    class Program
    {
        static void Main(string[] args)
        {
            //DbEntry.Context.DropAndCreate(typeof(SampleData));

            //var list = SampleData.Find(CK.K["Id"] < 10);
            var list = DbEntry.From<SampleData>().Where(CK.K["Id"] < 10).OrderBy("Id").Range(2,2).Select();
            //var list = DbEntry.From<Shipper>().Where(CK.K["Shipper ID"] < 10).OrderBy(new DESC("Shipper ID")).Select();

            foreach (var o in list)
            {
                Console.WriteLine(o);
            }

            Console.ReadLine();
        }
    }
}
