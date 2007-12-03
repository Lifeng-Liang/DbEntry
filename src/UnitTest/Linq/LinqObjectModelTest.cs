
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lephone.Data.Definition;
using Lephone.Data;
using Lephone.Linq;
using NUnit.Framework;

namespace Lephone.UnitTest.Linq
{
    [TestFixture]
    public class LinqObjectModelTest
    {
        [DbTable("Books")]
        public abstract class Book : LinqObjectModel<Book>
        {
            public abstract string Name { get; set; }
            public abstract int Category_Id { get; set; }
        }

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
        }
    }
}
