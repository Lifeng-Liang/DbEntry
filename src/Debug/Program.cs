
#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.SqlEntry;
using Lephone.Data.Definition;

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
            SampleData.Find(CK.K["Id"] >= 5, new OrderBy("Id"))
            .ForEach(delegate(SampleData d)
            {
                Console.WriteLine(d);
            });
            Console.WriteLine();
        }
    }
}
