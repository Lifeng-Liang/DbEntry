using System.Collections.Generic;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class MultipleRelationsTest
    {
        #region objects of HasMany and HasOne

        public class mrUser : DbObjectModel<mrUser>
        {
            public string Name { get; set; }
            [HasMany] public IList<mrProject> Projects { get; private set; }
        }

        public class mrProject : DbObjectModel<mrProject>
        {
            public string Name { get; set; }
            [BelongsTo] public mrUser Owner { get; set; }
            [HasMany] public IList<mrSubject> Subjects { get; private set; }
        }

        public class mrSubject : DbObjectModel<mrSubject>
        {
            public string Name { get; set; }
            [BelongsTo] public mrProject Owner { get; set; }
            [HasMany] public IList<mrAttribute> Attributes { get; private set; }
        }

        public class mrAttribute : DbObjectModel<mrAttribute>
        {
            public string Name { get; set; }
            [BelongsTo] public mrSubject Owner { get; set; }
            [HasOne] public mrTitle Title { get; set; }
        }

        public class mrTitle : DbObjectModel<mrTitle>
        {
            public string Name { get; set; }
            [BelongsTo] public mrAttribute Owner { get; set; }
        }

        #endregion

        #region objects of HasAndBelongsTo and HasOne and HasMany

        public class mrBook : DbObjectModel<mrBook>
        {
            public string Name { get; set; }
            [HasAndBelongsToMany] public IList<mrCategory> Categories { get; private set; }
        }

        public class mrCategory : DbObjectModel<mrCategory>
        {
            public string Name { get; set; }
            [HasAndBelongsToMany] public IList<mrBook> Books { get; private set; }
            [HasOne] public mrCateTitle Title { get; set; }
        }

        public class mrCateTitle : DbObjectModel<mrCateTitle>
        {
            public string Name { get; set; }
            [BelongsTo] public mrCategory Category { get; set; }
            [HasMany] public IList<mrCateTitleName> Names { get; private set; }
        }

        public class mrCateTitleName : DbObjectModel<mrCateTitleName>
        {
            public string Name { get; set; }
            [BelongsTo] public mrCateTitle Owner { get; set; }
        }

        #endregion

        #region simulate many to many

        public class mrReader : DbObjectModel<mrReader>
        {
            public string Name { get; set; }
            [HasMany] public IList<mrReaderAndArticle> xTable { get; private set; }
        }

        public class mrReaderAndArticle : DbObjectModel<mrReaderAndArticle>
        {
            public string Name { get; set; }
            [BelongsTo] public mrReader Reader { get; set; }
            [BelongsTo] public mrArticle Article { get; set; }
        }

        public class mrArticle : DbObjectModel<mrArticle>
        {
            public string Name { get; set; }
            [HasMany] public IList<mrReaderAndArticle> xTable { get; private set; }
        }

        #endregion

        [Test]
        public void TestHasManyAndHasOne1()
        {
            var u = new mrUser {Name = "user"};
            var p = new mrProject {Name = "project"};
            u.Projects.Add(p);
            Assert.AreEqual("user", p.Owner.Name);
            var s = new mrSubject {Name = "subject"};
            p.Subjects.Add(s);
            var a = new mrAttribute {Name = "attribute"};
            s.Attributes.Add(a);
            var t = new mrTitle {Name = "title"};
            a.Title = t;

            u.Save();

            mrUser user = mrUser.FindById(u.Id);

            Assert.AreEqual("user", user.Name);
            Assert.AreEqual("project", user.Projects[0].Name);
            Assert.AreEqual("user", user.Projects[0].Owner.Name);
            Assert.AreEqual("subject", user.Projects[0].Subjects[0].Name);
            Assert.AreEqual("attribute", user.Projects[0].Subjects[0].Attributes[0].Name);
            Assert.AreEqual("title", user.Projects[0].Subjects[0].Attributes[0].Title.Name);
        }

        [Test]
        public void TestHasManyAndHasOne2()
        {
            var u = new mrUser {Name = "user"};
            var p = new mrProject {Name = "project"};
            u.Projects.Add(p);
            Assert.AreEqual("user", p.Owner.Name);
            var s = new mrSubject {Name = "subject"};
            p.Subjects.Add(s);
            var a = new mrAttribute {Name = "attribute"};
            s.Attributes.Add(a);
            var t = new mrTitle {Name = "title"};
            a.Title = t;

            u.Save();

            mrUser user = mrUser.FindById(u.Id);

            user.Projects[0].Subjects[0].Attributes[0].Title.Name = "new value";
            user.Save();

            mrUser user1 = mrUser.FindById(u.Id);
            Assert.AreEqual("new value", user1.Projects[0].Subjects[0].Attributes[0].Title.Name);
        }

        [Test]
        public void TestHasAndBelongsToMany1()
        {
            var b = new mrBook {Name = "book"};
            var c = new mrCategory {Name = "category"};
            b.Categories.Add(c);
            var t = new mrCateTitle {Name = "title"};
            c.Title = t;
            var n = new mrCateTitleName {Name = "name"};
            t.Names.Add(n);

            b.Save();

            mrBook book = mrBook.FindById(b.Id);

            Assert.AreEqual("book", book.Name);
            Assert.AreEqual("category", book.Categories[0].Name);
            Assert.AreEqual("title", book.Categories[0].Title.Name);
            Assert.AreEqual("name", book.Categories[0].Title.Names[0].Name);
        }

        [Test, Ignore("for now")]
        public void TestHasAndBelongsToMany2()
        {
            var b = new mrBook {Name = "book"};
            var c = new mrCategory {Name = "category"};
            b.Categories.Add(c);
            Assert.AreEqual(1, c.Books.Count);
            var t = new mrCateTitle {Name = "title"};
            c.Title = t;
            var n = new mrCateTitleName {Name = "name"};
            t.Names.Add(n);

            c.Save();

            mrBook book = mrBook.FindById(b.Id);

            Assert.AreEqual("book", book.Name);
            Assert.AreEqual("category", book.Categories[0].Name);
            Assert.AreEqual("title", book.Categories[0].Title.Name);
            Assert.AreEqual("name", book.Categories[0].Title.Names[0].Name);
        }

        [Test]
        public void TestSimulateManyToMany()
        {
            var r = new mrReader {Name = "reader"};
            var a = new mrArticle {Name = "article"};
            var ra = new mrReaderAndArticle {Name = "x"};
            r.xTable.Add(ra);
            a.xTable.Add(ra);

            Assert.AreEqual(a, ra.Article);
            Assert.AreEqual(r, ra.Reader);
            Assert.AreEqual(1, a.xTable.Count);
            Assert.AreEqual("x", a.xTable[0].Name);
            Assert.AreEqual("article", r.xTable[0].Article.Name);

            r.Save();

            mrReader reader = mrReader.FindById(r.Id);
            Assert.AreEqual("reader", reader.Name);
            Assert.AreEqual("x", reader.xTable[0].Name);
            Assert.AreEqual("article", reader.xTable[0].Article.Name);
        }

        [Test]
        public void TestSimulateManyToMany2()
        {
            var r = new mrReader {Name = "reader"};
            var a = new mrArticle {Name = "article"};
            var ra = new mrReaderAndArticle {Name = "x"};
            r.xTable.Add(ra);
            r.xTable[0].Article = a;

            Assert.AreEqual(a, ra.Article);
            Assert.AreEqual(r, ra.Reader);
            Assert.AreEqual(1, a.xTable.Count);
            Assert.AreEqual("x", a.xTable[0].Name);
            Assert.AreEqual("article", r.xTable[0].Article.Name);

            r.Save();

            mrReader reader = mrReader.FindById(r.Id);
            Assert.AreEqual("reader", reader.Name);
            Assert.AreEqual("x", reader.xTable[0].Name);
            Assert.AreEqual("article", reader.xTable[0].Article.Name);
        }
    }
}
