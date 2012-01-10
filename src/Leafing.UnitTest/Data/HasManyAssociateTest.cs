using Leafing.Data;
using Leafing.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [TestFixture]
    public class HasManyAssociateTest : DataTestBase
    {
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
        public void TestHasMany1A()
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
            var tech = DbEntry.GetObject<Category>(1);
            var game = DbEntry.GetObject<Category>(2);
            var tour = DbEntry.GetObject<Category>(3);

            Assert.IsNotNull(tech);
            Assert.IsTrue(0 == tech.Books.Count);
            Assert.IsNotNull(game);
            Assert.IsTrue(3 == game.Books.Count);
            Assert.IsNotNull(tour);
            Assert.IsTrue(2 == tour.Books.Count);

            Assert.AreEqual("Tech", tech.Name);
            Assert.AreEqual("Game", game.Name);
            Assert.AreEqual("Tour", tour.Name);

            Assert.AreEqual("Diablo", game.Books[0].Name);
            Assert.AreEqual("Beijing", tour.Books[0].Name);
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
        public void TestHasMany4A()
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
        public void TestHasMany4B()
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
            // A.Delete will delete itself, and set B.A_Id to null *
            var c = Category.FindById(2);
            Assert.IsNotNull(c);
            Assert.IsTrue(3 == c.Books.Count);
            var bid1 = c.Books[0].Id;
            var bid2 = c.Books[1].Id;
            var bid3 = c.Books[2].Id;
            // do delete
            c.Delete();
            var c1 = Category.FindById(2);
            Assert.IsNull(c1);
            var bs = Book.Find(CK.K["Category_Id"] == null, "Id");
            Assert.AreEqual(3, bs.Count);
            Assert.AreEqual(bid1, bs[0].Id);
            Assert.AreEqual(bid2, bs[1].Id);
            Assert.AreEqual(bid3, bs[2].Id);
        }

        [Test]
        public void TestHasMany6()
        {
            // B has a foreign key A_id
            // B.a = A will set value of B.A_id
            // B.a = A will set A.a = b ????
            var c = DbEntry.GetObject<Category>(3);
            var b = new Book {Name = "Luoyang", CurCategory = {Value = c}};

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
            Assert.IsTrue(0 != c.Id);
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

        [Test]
        public void TestRemoveSubItem()
        {
            var c = DbEntry.GetObject<Acategory>(2);
            Assert.AreEqual(3, c.Books.Count);

            c.Books.RemoveAt(1);
            DbEntry.Save(c);

            var c2 = DbEntry.GetObject<Acategory>(2);
            Assert.AreEqual(2, c2.Books.Count);
        }

        [Test]
        public void TestReadAfterSave()
        {
            var c = DbEntry.GetObject<Category>(2);
            Assert.IsNotNull(c);
            DbEntry.Save(c);

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
        public void TestSaveFromBelongsTo()
        {
            var c = new Category { Name = "test" };
            var b1 = new Book { Name = "123" };
            var b2 = new Book { Name = "456" };
            var b3 = new Book { Name = "789" };
            c.Books.Add(b1);
            c.Books.Add(b2);
            c.Books.Add(b3);
            c.Save();

            var c1 = Category.FindById(c.Id);
            Assert.IsNotNull(c1);
            Assert.AreEqual("test", c1.Name);
            Assert.AreEqual(3, c1.Books.Count);
            Assert.AreEqual("123", c1.Books[0].Name);
            Assert.AreEqual("456", c1.Books[1].Name);
            Assert.AreEqual("789", c1.Books[2].Name);
        }

        [Test]
        public void TestSaveFromBelongsTo1()
        {
            var c = new Category { Name = "test" };
            var b1 = new Book { Name = "123" };
            var b2 = new Book { Name = "456" };
            c.Books.Add(b1);
            c.Books.Add(b2);
            b2.Save();

            var c1 = Category.FindById(c.Id);
            Assert.IsNotNull(c1);
            Assert.AreEqual("test", c1.Name);
            Assert.AreEqual(2, c1.Books.Count);
            Assert.AreEqual("123", c1.Books[0].Name);
            Assert.AreEqual("456", c1.Books[1].Name);
        }

        [Test]
        public void TestSaveFromBelongsTo2()
        {
            var c = new Category { Name = "test" };
            var b1 = new Book { Name = "123" };
            var b2 = new Book { Name = "456" };
            var b3 = new Book { Name = "789" };
            b1.CurCategory.Value = c;
            b2.CurCategory.Value = c;
            b3.CurCategory.Value = c;
            b3.Save();

            var c1 = Category.FindById(c.Id);
            Assert.IsNotNull(c1);
            Assert.AreEqual("test", c1.Name);
            Assert.AreEqual(3, c1.Books.Count);
            Assert.AreEqual("123", c1.Books[0].Name);
            Assert.AreEqual("456", c1.Books[1].Name);
            Assert.AreEqual("789", c1.Books[2].Name);
        }

        [Test]
        public void TestHasManyCutTheRelationByDelete()
        {
            // B.Delete() will cut the relation of it from A
            var c = Category.FindById(2);
            Assert.IsNotNull(c);
            Assert.AreEqual(3, c.Books.Count);
            Assert.AreEqual("Game", c.Name);
            Assert.AreEqual("Diablo", c.Books[0].Name);

            var b = c.Books[0];
            b.Delete();
            Assert.AreEqual(2, c.Books.Count);
            Assert.AreEqual("Pal95", c.Books[0].Name);
            Assert.AreEqual("Wow", c.Books[1].Name);
            Assert.IsNull(b.CurCategory);

            c.Save();
            c = Category.FindById(2);
            Assert.IsNotNull(c);
            Assert.AreEqual(2, c.Books.Count);
            Assert.AreEqual("Pal95", c.Books[0].Name);
            Assert.AreEqual("Wow", c.Books[1].Name);
        }
    }
}
