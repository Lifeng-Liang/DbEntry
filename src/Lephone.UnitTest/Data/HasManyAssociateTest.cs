using Lephone.Data;
using Lephone.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class HasManyAssociateTest
    {
        #region Init

        [SetUp]
        public void SetUp()
        {
            InitHelper.Init();
        }

        [TearDown]
        public void TearDown()
        {
            InitHelper.Clear();
        }

        #endregion

        [Test]
        public void TestHasMany1()
        {
            // A.Select will read B (LazyLoading*)
            var c = DbEntry.GetObject<Category>(2);
            Assert.IsNotNull(c);
            Assert.IsTrue(3 == c.Books.Count);

            Assert.AreEqual("Game", c.Name);
            Assert.AreEqual("Diablo", c.Books[0].Name);
            Assert.AreEqual(1, c.Books[0].Id);
            Assert.AreEqual("Pal95", c.Books[1].Name);
            Assert.AreEqual(4, c.Books[1].Id);
            Assert.AreEqual("Wow", c.Books[2].Name);
            Assert.AreEqual(5, c.Books[2].Id);
        }

        [Test]
        public void TestHasMany1_1()
        {
            // A.Select will read B (LazyLoading*), and set B.a as A
            var c = DbEntry.GetObject<Category>(2);
            Assert.IsNotNull(c);
            Assert.IsTrue(3 == c.Books.Count);
            Assert.AreEqual("Game", c.Name);
            c.Name = "Sport";
            Assert.AreEqual("Sport", c.Books[0].CurCategory.Value.Name);
        }

        [Test]
        public void TestHasMany2()
        {
            // A owns 0 or multiple B, so b(b.Value) could be null
            var Tech = DbEntry.GetObject<Category>(1);
            var Game = DbEntry.GetObject<Category>(2);
            var Tour = DbEntry.GetObject<Category>(3);

            Assert.IsNotNull(Tech);
            Assert.IsTrue(0 == Tech.Books.Count);
            Assert.IsNotNull(Game);
            Assert.IsTrue(3 == Game.Books.Count);
            Assert.IsNotNull(Tour);
            Assert.IsTrue(2 == Tour.Books.Count);

            Assert.AreEqual("Tech", Tech.Name);
            Assert.AreEqual("Game", Game.Name);
            Assert.AreEqual("Tour", Tour.Name);

            Assert.AreEqual("Diablo", Game.Books[0].Name);
            Assert.AreEqual("Beijing", Tour.Books[0].Name);
        }

        [Test]
        public void TestHasMany3()
        {
            // A.b = new B() will set B.a = A
            var c = new Category {Name = "Sport"};
            var b = new Book {Name = "Basketball"};
            c.Books.Add(b);
            Assert.AreEqual(b.CurCategory.Value, c);
        }

        [Test]
        public void TestHasMany4_1()
        {
            // A.Save will save B, if is A Update, then save B
            var c = DbEntry.GetObject<Category>(1);
            Assert.IsNotNull(c);
            Assert.IsTrue(0 == c.Books.Count);
            c.Books.Add(new Book());
            c.Books[0].Name = "C#";
            Assert.AreEqual(0, c.Books[0].Id);
            DbEntry.Save(c);
            Assert.IsTrue(1 == c.Id);
            Assert.IsTrue(0 != c.Books[0].Id);

            var c1 = DbEntry.GetObject<Category>(1);
            Assert.IsNotNull(c1);
            Assert.IsTrue(1 == c1.Books.Count);
            Assert.AreEqual("C#", c1.Books[0].Name);
        }

        [Test]
        public void TestHasMany4_2()
        {
            // A.Save will save B, if is A Insert, then save A first, then set B.A_id, and then save B
            var c = new Category {Name = "Sport"};
            c.Books.Add(new Book());
            c.Books[0].Name = "Basketball";
            DbEntry.Save(c);
            Assert.IsTrue(0 != c.Id);
            Assert.IsTrue(0 != c.Books[0].Id);
            Assert.IsTrue(0 != (long)c.Books[0].CurCategory.ForeignKey);

            var c1 = DbEntry.GetObject<Category>(c.Id);
            Assert.AreEqual("Sport", c1.Name);
            Assert.IsTrue(1 == c1.Books.Count);
            Assert.IsNotNull(c1.Books[0]);
            Assert.AreEqual("Basketball", c1.Books[0].Name);
        }

        [Test]
        public void TestHasMany5()
        {
            // A.Delete will delete itself, and delete B *
            var c = DbEntry.GetObject<Category>(2);
            Assert.IsNotNull(c);
            Assert.IsTrue(3 == c.Books.Count);
            long bid1 = c.Books[0].Id;
            long bid2 = c.Books[1].Id;
            long bid3 = c.Books[2].Id;
            // do delete
            DbEntry.Delete(c);
            var c1 = DbEntry.GetObject<Category>(2);
            Assert.IsNull(c1);
            Assert.IsNull(DbEntry.GetObject<Book>(bid1));
            Assert.IsNull(DbEntry.GetObject<Book>(bid2));
            Assert.IsNull(DbEntry.GetObject<Book>(bid3));
        }

        [Test]
        public void TestHasMany6()
        {
            // B has a foreign key A_id
            // B.a = A will set value of B.A_id
            // B.a = A will set A.a = b ????
            var c = DbEntry.GetObject<Category>(3);
            var b = new Book();
            b.Name = "Luoyang";
            b.CurCategory.Value = c;

            Assert.AreEqual(b.CurCategory.ForeignKey, 3);
        }

        [Test]
        public void TestHasMany7()
        {
            // B.Save will save itself
            var c = new Category {Name = "Sport"};
            var b = new Book {Name = "Basketball"};
            c.Books.Add(b);

            DbEntry.Save(b);

            Assert.IsTrue(0 != b.Id);
            Assert.IsTrue(0 == c.Id);
        }

        [Test]
        public void TestHasMany8()
        {
            // B.Delete will delete itself
            var c = DbEntry.GetObject<Category>(2);
            Assert.IsNotNull(c);
            Assert.IsTrue(3 == c.Books.Count);
            Book b = c.Books[0];
            DbEntry.Delete(b);
            Assert.IsTrue(0 == b.Id);
            c = DbEntry.GetObject<Category>(2);
            Assert.IsNotNull(c);
            Assert.IsTrue(2 == c.Books.Count);
        }

        [Test]
        public void TestHasMany9()
        {
            // If not loading B, and insert a new item in B, then don't loading, when save it, only save which in the memory
            var c = DbEntry.GetObject<Category>(2);
            var b = new Book {Name = "Next"};
            c.Books.Add(b);
            Assert.AreEqual(1, c.Books.Count);
            DbEntry.Save(c);

            // verify
            c = DbEntry.GetObject<Category>(2);
            Assert.IsNotNull(c);
            Assert.IsTrue(4 == c.Books.Count);

            Assert.AreEqual("Game", c.Name);
            Assert.AreEqual("Diablo", c.Books[0].Name);
            Assert.AreEqual(1, c.Books[0].Id);
            Assert.AreEqual("Pal95", c.Books[1].Name);
            Assert.AreEqual(4, c.Books[1].Id);
            Assert.AreEqual("Wow", c.Books[2].Name);
            Assert.AreEqual(5, c.Books[2].Id);
            Assert.AreEqual("Next", c.Books[3].Name);
            Assert.AreEqual(6, c.Books[3].Id);
        }

        [Test]
        public void TestHasMany10()
        {
            // B.Select will read A (LazyLoading*)
            var c = DbEntry.GetObject<Book>(2);
            Assert.IsNotNull(c);
            Assert.IsNotNull(c.CurCategory.Value);
            Assert.AreEqual("Tour", c.CurCategory.Value.Name);
        }

        [Test]
        public void TestHasMany11()
        {
            // B.Select will read A (LazyLoading*), and B.a.b[x] == B
            var c = DbEntry.GetObject<Book>(2);
            Assert.IsNotNull(c);
            Assert.AreEqual("Tour", c.CurCategory.Value.Name);
            Assert.AreEqual(2, c.CurCategory.Value.Books.Count);
            Assert.AreEqual("Beijing", c.CurCategory.Value.Books[0].Name);
            Assert.AreEqual("Shanghai", c.CurCategory.Value.Books[1].Name);
            c.Name = "Pingxiang";
            Assert.AreEqual("Pingxiang", c.CurCategory.Value.Books[0].Name);
        }

        [Test]
        public void TestHasMany12()
        {
            // A.Save will save B, if b(b.Value) is null, then don't save B
            var c = new Category {Name = "Sport"};
            DbEntry.Save(c);
            var c1 = DbEntry.GetObject<Category>(c.Id);
            Assert.IsNotNull(c1);
            Assert.IsTrue(0 == c1.Books.Count);
            Assert.AreEqual(c.Name, c1.Name);
        }

        [Test]
        public void TestHasMany13()
        {
            // DbEntry.Save(c.Books) will save all data in the list,
            var c = DbEntry.GetObject<Category>(3);
            Assert.AreEqual("Tour", c.Name);
            Assert.AreEqual(2, c.Books.Count);
            c.Name = "Sport";
            c.Books[0].Name = "Hongkong";
            c.Books[1].Name = "Luoyang";
            var b = new Book {Name = "Pingxiang"};
            c.Books.Add(b);
            DbEntry.Save(c.Books);

            var c1 = DbEntry.GetObject<Category>(3);
            Assert.AreEqual("Tour", c1.Name);
            Assert.AreEqual(3, c1.Books.Count);
            Assert.AreEqual("Hongkong", c1.Books[0].Name);
            Assert.AreEqual("Luoyang", c1.Books[1].Name);
            Assert.AreEqual("Pingxiang", c1.Books[2].Name);
        }

        [Test]
        public void TestRemoveAnItem()
        {
            var c = DbEntry.GetObject<Category>(3);
            Assert.AreEqual("Tour", c.Name);
            Assert.AreEqual(2, c.Books.Count);
            Assert.AreEqual("Beijing", c.Books[0].Name);
            Assert.AreEqual("Shanghai", c.Books[1].Name);

            c.Books.RemoveAt(0);
            DbEntry.Save(c);

            c = DbEntry.GetObject<Category>(3);
            Assert.AreEqual("Tour", c.Name);
            Assert.AreEqual(1, c.Books.Count);
            Assert.AreEqual("Shanghai", c.Books[0].Name);
        }

        [Test]
        public void TestRemoveAnItem2()
        {
            var b = DbEntry.GetObject<Book>(2);
            Assert.AreEqual("Beijing", b.Name);
            Assert.IsNotNull(b.CurCategory);

            b.CurCategory.Value = null;
            DbEntry.Save(b);

            b = DbEntry.GetObject<Book>(2);
            Assert.AreEqual("Beijing", b.Name);
            Assert.IsNull(b.CurCategory.Value);
        }

        [Test]
        public void TestListClear()
        {
            var c = DbEntry.GetObject<Category>(3);
            Assert.AreEqual(2, c.Books.Count);

            c.Books.Clear();
            DbEntry.Save(c);

            var c1 = DbEntry.GetObject<Category>(3);
            Assert.AreEqual(0, c1.Books.Count);
        }

        [Test]
        public void TestListClear2()
        {
            var c = DbEntry.GetObject<Category>(3);
            c.Books.Clear();
            DbEntry.Save(c);

            var c1 = DbEntry.GetObject<Category>(3);
            Assert.AreEqual(0, c1.Books.Count);
        }

        [Test]
        public void TestListClear3()
        {
            var c = DbEntry.GetObject<Category>(3);
            var b = new Book {Name = "test"};
            c.Books.Add(b);
            c.Books.Clear();
            DbEntry.Save(c);

            var c1 = DbEntry.GetObject<Category>(3);
            Assert.AreEqual(2, c1.Books.Count);
        }
    }
}
