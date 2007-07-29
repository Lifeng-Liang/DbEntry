
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
    public abstract class User : DbObjectModel<User>
    {
        public abstract string Name { get; set; }
        public abstract int Age { get; set; }
        public abstract bool Gender { get; set; }
        public abstract DateTime Birthday { get; set; }

        public User() { }
        public User(string Name, int Age, bool Gender, DateTime Birthday)
        {
            this.Name = Name;
            this.Age = Age;
            this.Gender = Gender;
            this.Birthday = Birthday;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Create
            User u = User.New("tom", 18, true, DateTime.Now);
            u.Save();
            // Read
            User u1 = User.FindById(u.Id);
            // Update
            u1.Name = "jerry";
            u1.Save();
            // Delete
            u1.Delete();
            // Complex Query
            List<User> ls = User.Find(CK.K["Age"] > 15 && CK.K["Gender"] == true);
            // Use Sql
            List<User> ls1 = User.FindBySql(
                "Select * From [User] Where [Age] > 15 And [Gender] = true");
        }
    }
}
