using System.Linq;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Linq
{
    [TestFixture]
    public class LinqObjectModelTest : DataTestBase
    {
        [DbTable("Books")]
        public class Book : DbObjectModel<Book>
        {
            public string Name { get; set; }
            public long Category_Id { get; set; }
        }

        [Test]
        public void Test1()
        {
            var list = Book.Find(p => p.Id >= 2 && p.Id <= 3, p => p.Id);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Beijing", list[0].Name);
            Assert.AreEqual("Shanghai", list[1].Name);
        }

        [Test]
        public void Test2()
        {
            var list = Book.Find(p => p.Name == "Pal95");
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(4, list[0].Id);
            Assert.AreEqual(2, list[0].Category_Id);
        }

        [Test]
        public void Test3()
        {
            var list = Book.Find(p => p.Id >= 2 && p.Id <= 3, "Id");
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Beijing", list[0].Name);
            Assert.AreEqual("Shanghai", list[1].Name);
        }

        [Test]
        public void Test4()
        {
            var item = Book.FindOne(p => p.Name == "Pal95");
            Assert.AreEqual(4, item.Id);
            Assert.AreEqual(2, item.Category_Id);
        }

        [Test]
        public void Test5()
        {
            var list = from p in Book.Table where p.Id == 2 select p;
            Assert.AreEqual("Beijing", list.ToList()[0].Name);
        }

        [Test]
        public void Test6()
        {
            var list = from p in Book.Table where p.Name == "Pal95" select p;
            Assert.AreEqual(4, list.ToList()[0].Id);
        }

        [Test]
        public void Test7()
        {
            var list = from p in Book.Table where p.Id >= 2 && p.Id <= 3 orderby p.Id select p;
            var l = list.ToList();
            Assert.AreEqual("Beijing", l[0].Name);
            Assert.AreEqual("Shanghai", l[1].Name);
        }

        [Test]
        public void Test8()
        {
            var list = from p in Book.Table where p.Id >= 2 && p.Id <= 3 orderby p.Id descending select p;
            var l = list.ToList();
            Assert.AreEqual("Shanghai", l[0].Name);
            Assert.AreEqual("Beijing", l[1].Name);
        }

        [Test]
        public void Test9()
        {
            var list = from p in Book.Table where p.Id >= 2 && p.Id <= 3 orderby p.Id descending, p.Name select p;
            var l = list.ToList();
            Assert.AreEqual("Shanghai", l[0].Name);
            Assert.AreEqual("Beijing", l[1].Name);
        }

        [Test]
        public void Test10()
        {
            var list = from p in Book.Table where p.Name == "Pal95" select p;
            foreach (Book o in list)
            {
                Assert.AreEqual(4, o.Id);
            }
        }

        [Test]
        public void Test11()
        {
            var l = Book.OrderBy(p => p.Id).Find(p => p.Id >= 2 && p.Id <= 3);
            Assert.AreEqual("Beijing", l[0].Name);
            Assert.AreEqual("Shanghai", l[1].Name);
        }

        [Test]
        public void Test12()
        {
            var l = Book.OrderBy(p => p.Name).Find(p => p.Id >= 1 && p.Id <= 3);
            Assert.AreEqual("Beijing", l[0].Name);
            Assert.AreEqual("Diablo", l[1].Name);
            Assert.AreEqual("Shanghai", l[2].Name);
        }

        [Test]
        public void Test13()
        {
            var l = Book.OrderByDescending(p => p.Name).Find(p => p.Id >= 1 && p.Id <= 3);
            Assert.AreEqual("Shanghai", l[0].Name);
            Assert.AreEqual("Diablo", l[1].Name);
            Assert.AreEqual("Beijing", l[2].Name);
        }

        [Test]
        public void Test14()
        {
            var l = Book.OrderByDescending(p => p.Id).Find(p => p.Id >= 2 && p.Id <= 3);
            Assert.AreEqual("Shanghai", l[0].Name);
            Assert.AreEqual("Beijing", l[1].Name);
        }

        [Test, Ignore]
        public void TestPartialSelect0()
        {
            var l = from book in Book.Table where book.Category_Id == 2 orderby book.Id select book.Name;
            var list = l.ToList();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("Diablo", list[0]);
            Assert.AreEqual("Pal95", list[1]);
            Assert.AreEqual("Wow", list[2]);
        }

        [Test]
        public void TestPartialSelect1()
        {
            var l = from book in Book.Table where book.Category_Id == 2 orderby book.Id select new { book.Id };
            var list = l.ToArray();
            Assert.AreEqual(3, list.Length);
            Assert.AreEqual(1, list[0].Id);
            Assert.AreEqual(4, list[1].Id);
            Assert.AreEqual(5, list[2].Id);
        }

        [Test]
        public void TestPartialSelect()
        {
            var l = from book in Book.Table where book.Category_Id == 2 orderby book.Id select new { book.Name };
            var list = l.ToList();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("Diablo", list[0].Name);
            Assert.AreEqual("Pal95", list[1].Name);
            Assert.AreEqual("Wow", list[2].Name);
        }

        [Test]
        public void TestPartialSelect2()
        {
            var l = from book in Book.Table where book.Category_Id == 2 orderby book.Id select new { book.Name, book.Category_Id };
            var list = l.ToList();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("Diablo", list[0].Name);
            Assert.AreEqual(2, list[0].Category_Id);
            Assert.AreEqual("Pal95", list[1].Name);
            Assert.AreEqual(2, list[1].Category_Id);
            Assert.AreEqual("Wow", list[2].Name);
            Assert.AreEqual(2, list[2].Category_Id);
        }

        [Test]
        public void TestPartialSelect3()
        {
            var l = from book in Book.Table where book.Category_Id == 2 orderby book.Id select new { book.Category_Id, book.Name };
            var list = l.ToList();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("Diablo", list[0].Name);
            Assert.AreEqual(2, list[0].Category_Id);
            Assert.AreEqual("Pal95", list[1].Name);
            Assert.AreEqual(2, list[1].Category_Id);
            Assert.AreEqual("Wow", list[2].Name);
            Assert.AreEqual(2, list[2].Category_Id);
        }

        [Test]
        public void TestPartialSelect4()
        {
            var l = from book in Book.Table where book.Category_Id == 2 orderby book.Id select new { book.Name, CID = book.Category_Id };
            var list = l.ToList();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("Diablo", list[0].Name);
            Assert.AreEqual(2, list[0].CID);
            Assert.AreEqual("Pal95", list[1].Name);
            Assert.AreEqual(2, list[1].CID);
            Assert.AreEqual("Wow", list[2].Name);
            Assert.AreEqual(2, list[2].CID);
        }
    }
}
