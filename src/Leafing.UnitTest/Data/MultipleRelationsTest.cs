using System.Collections.Generic;
using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Data {
    [TestFixture]
    public class MultipleRelationsTest {
        #region objects of HasMany and HasOne

        public class mrUser : DbObjectModel<mrUser> {
            public string Name { get; set; }
            public HasMany<mrProject> Projects { get; private set; }
        }

        public class mrProject : DbObjectModel<mrProject> {
            public string Name { get; set; }
            public BelongsTo<mrUser, long> Owner { get; private set; }
            public HasMany<mrSubject> Subjects { get; private set; }
        }

        public class mrSubject : DbObjectModel<mrSubject> {
            public string Name { get; set; }
            public BelongsTo<mrProject, long> Owner { get; private set; }
            public HasMany<mrAttribute> Attributes { get; private set; }
        }

        public class mrAttribute : DbObjectModel<mrAttribute> {
            public string Name { get; set; }
            public BelongsTo<mrSubject, long> Owner { get; private set; }
            public HasOne<mrTitle> Title { get; private set; }
        }

        public class mrTitle : DbObjectModel<mrTitle> {
            public string Name { get; set; }
            public BelongsTo<mrAttribute, long> Owner { get; private set; }
        }

        #endregion

        #region objects of HasAndBelongsTo and HasOne and HasMany

        public class mrBook : DbObjectModel<mrBook> {
            public string Name { get; set; }
            public HasAndBelongsToMany<mrCategory> Categories { get; private set; }
        }

        public class mrCategory : DbObjectModel<mrCategory> {
            public string Name { get; set; }
            public HasAndBelongsToMany<mrBook> Books { get; private set; }
            public HasOne<mrCateTitle> Title { get; set; }
        }

        public class mrCateTitle : DbObjectModel<mrCateTitle> {
            public string Name { get; set; }
            public BelongsTo<mrCategory, long> Category { get; private set; }
            public HasMany<mrCateTitleName> Names { get; private set; }
        }

        public class mrCateTitleName : DbObjectModel<mrCateTitleName> {
            public string Name { get; set; }
            public BelongsTo<mrCateTitle, long> Owner { get; private set; }
        }

        #endregion

        #region simulate many to many

        public class mrReader : DbObjectModel<mrReader> {
            public string Name { get; set; }
            public HasMany<mrReaderAndArticle> xTable { get; private set; }
        }

        public class mrReaderAndArticle : DbObjectModel<mrReaderAndArticle> {
            public string Name { get; set; }
            public BelongsTo<mrReader, long> Reader { get; private set; }
            public BelongsTo<mrArticle, long> Article { get; private set; }
        }

        public class mrArticle : DbObjectModel<mrArticle> {
            public string Name { get; set; }
            public HasMany<mrReaderAndArticle> xTable { get; private set; }
        }

        #endregion

        [Test]
        public void TestHasManyAndHasOne1() {
            var u = new mrUser { Name = "user" };
            var p = new mrProject { Name = "project" };
            u.Projects.Add(p);
            Assert.AreEqual("user", p.Owner.Value.Name);
            var s = new mrSubject { Name = "subject" };
            p.Subjects.Add(s);
            var a = new mrAttribute { Name = "attribute" };
            s.Attributes.Add(a);
            var t = new mrTitle { Name = "title" };
            a.Title.Value = t;

            u.Save();

            mrUser user = mrUser.FindById(u.Id);

            Assert.AreEqual("user", user.Name);
            Assert.AreEqual("project", user.Projects[0].Name);
            Assert.AreEqual("user", user.Projects[0].Owner.Value.Name);
            Assert.AreEqual("subject", user.Projects[0].Subjects[0].Name);
            Assert.AreEqual("attribute", user.Projects[0].Subjects[0].Attributes[0].Name);
            Assert.AreEqual("title", user.Projects[0].Subjects[0].Attributes[0].Title.Value.Name);
        }

        [Test]
        public void TestHasManyAndHasOne2() {
            var u = new mrUser { Name = "user" };
            var p = new mrProject { Name = "project" };
            u.Projects.Add(p);
            Assert.AreEqual("user", p.Owner.Value.Name);
            var s = new mrSubject { Name = "subject" };
            p.Subjects.Add(s);
            var a = new mrAttribute { Name = "attribute" };
            s.Attributes.Add(a);
            var t = new mrTitle { Name = "title" };
            a.Title.Value = t;

            u.Save();

            mrUser user = mrUser.FindById(u.Id);

            user.Projects[0].Subjects[0].Attributes[0].Title.Value.Name = "new value";
            user.Save();

            mrUser user1 = mrUser.FindById(u.Id);
            Assert.AreEqual("new value", user1.Projects[0].Subjects[0].Attributes[0].Title.Value.Name);
        }

        [Test]
        public void TestHasAndBelongsToMany1() {
            var b = new mrBook { Name = "book" };
            var c = new mrCategory { Name = "category" };
            b.Categories.Add(c);
            var t = new mrCateTitle { Name = "title" };
            c.Title.Value = t;
            var n = new mrCateTitleName { Name = "name" };
            t.Names.Add(n);

            b.Save();

            mrBook book = mrBook.FindById(b.Id);

            Assert.AreEqual("book", book.Name);
            Assert.AreEqual("category", book.Categories[0].Name);
            Assert.AreEqual("title", book.Categories[0].Title.Value.Name);
            Assert.AreEqual("name", book.Categories[0].Title.Value.Names[0].Name);
        }

        [Test, Ignore("for now")]
        public void TestHasAndBelongsToMany2() {
            var b = new mrBook { Name = "book" };
            var c = new mrCategory { Name = "category" };
            b.Categories.Add(c);
            Assert.AreEqual(1, c.Books.Count);
            var t = new mrCateTitle { Name = "title" };
            c.Title.Value = t;
            var n = new mrCateTitleName { Name = "name" };
            t.Names.Add(n);

            c.Save();

            mrBook book = mrBook.FindById(b.Id);

            Assert.AreEqual("book", book.Name);
            Assert.AreEqual("category", book.Categories[0].Name);
            Assert.AreEqual("title", book.Categories[0].Title.Value.Name);
            Assert.AreEqual("name", book.Categories[0].Title.Value.Names[0].Name);
        }

        [Test]
        public void TestSimulateManyToMany() {
            var r = new mrReader { Name = "reader" };
            var a = new mrArticle { Name = "article" };
            var ra = new mrReaderAndArticle { Name = "x" };
            r.xTable.Add(ra);
            a.xTable.Add(ra);

            Assert.AreEqual(a, ra.Article.Value);
            Assert.AreEqual(r, ra.Reader.Value);
            Assert.AreEqual(1, a.xTable.Count);
            Assert.AreEqual("x", a.xTable[0].Name);
            Assert.AreEqual("article", r.xTable[0].Article.Value.Name);

            r.Save();

            mrReader reader = mrReader.FindById(r.Id);
            Assert.AreEqual("reader", reader.Name);
            Assert.AreEqual("x", reader.xTable[0].Name);
            Assert.AreEqual("article", reader.xTable[0].Article.Value.Name);
        }

        [Test]
        public void TestSimulateManyToMany2() {
            var r = new mrReader { Name = "reader" };
            var a = new mrArticle { Name = "article" };
            var ra = new mrReaderAndArticle { Name = "x" };
            r.xTable.Add(ra);
            r.xTable[0].Article.Value = a;

            Assert.AreEqual(a, ra.Article.Value);
            Assert.AreEqual(r, ra.Reader.Value);
            Assert.AreEqual(1, a.xTable.Count);
            Assert.AreEqual("x", a.xTable[0].Name);
            Assert.AreEqual("article", r.xTable[0].Article.Value.Name);

            r.Save();

            mrReader reader = mrReader.FindById(r.Id);
            Assert.AreEqual("reader", reader.Name);
            Assert.AreEqual("x", reader.xTable[0].Name);
            Assert.AreEqual("article", reader.xTable[0].Article.Value.Name);
        }
    }
}