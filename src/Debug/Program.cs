using System;
using System.Linq;
using Lephone.Data.Definition;

namespace Debug
{
    class Program
    {
        public class User : DbObjectModel<User>
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool Gender { get; set; }
            public DateTime Birthday { get; set; }
        }

        static void Main()
        {
            // Create
            var u = new User {Name = "tom", Age = 18, Gender = true, Birthday = DateTime.Now};
            u.Save();
            // Read
            var u1 = User.FindById(u.Id);
            // Update
            u1.Name = "jerry";
            u1.Save();
            // Delete
            u1.Delete();
            // Query
            var ids = from p in User.Table where p.Age > 15 select new { p.Id };
            var l1 = from p in User.Table where p.Age > 15 && p.Gender == true select p;
            var l2 = User.Find(p => p.Age > 15 && p.Gender == true); // another style of linq
            var l3 = User.FindBySql("Select * From [User] Where [Age] > 15 And [Gender] = true"); // sql
        }
    }
}