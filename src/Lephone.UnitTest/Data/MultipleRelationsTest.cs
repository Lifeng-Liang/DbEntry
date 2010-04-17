using System.Collections.Generic;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class MultipleRelationsTest
    {
        #region objects of HasMany and HasOne

        public abstract class mrUser : DbObjectModel<mrUser>
        {
            public abstract string Name { get; set; }
            [HasMany] public abstract IList<mrProject> Projects { get; set; }
            public mrUser Init(string name) { Name = name; return this; }
        }

        public abstract class mrProject : DbObjectModel<mrProject>
        {
            public abstract string Name { get; set; }
            [BelongsTo] public abstract mrUser Owner { get; set; }
            [HasMany] public abstract IList<mrSubject> Subjects { get; set; }
            public mrProject Init(string name) { Name = name; return this; }
        }

        public abstract class mrSubject : DbObjectModel<mrSubject>
        {
            public abstract string Name { get; set; }
            [BelongsTo] public abstract mrProject Owner { get; set; }
            [HasMany] public abstract IList<mrAttribute> Attributes { get; set; }
            public mrSubject Init(string name) { Name = name; return this; }
        }

        public abstract class mrAttribute : DbObjectModel<mrAttribute>
        {
            public abstract string Name { get; set; }
            [BelongsTo] public abstract mrSubject Owner { get; set; }
            [HasOne] public abstract mrTitle Title { get; set; }
            public mrAttribute Init(string name) { Name = name; return this; }
        }

        public abstract class mrTitle : DbObjectModel<mrTitle>
        {
            public abstract string Name { get; set; }
            [BelongsTo] public abstract mrAttribute Owner { get; set; }
            public mrTitle Init(string name) { Name = name; return this; }
        }

        #endregion

        #region objects of HasAndBelongsTo and HasOne and HasMany

        public abstract class mrBook : DbObjectModel<mrBook>
        {
            public abstract string Name { get; set; }
            [HasAndBelongsToMany] public abstract IList<mrCategory> Categories { get; set; }
            public mrBook Init(string name) { Name = name; return this; }
        }

        public abstract class mrCategory : DbObjectModel<mrCategory>
        {
            public abstract string Name { get; set; }
            [HasAndBelongsToMany] public abstract IList<mrBook> Books { get; set; }
            [HasOne] public abstract mrCateTitle Title { get; set; }
            public mrCategory Init(string name) { Name = name; return this; }
        }

        public abstract class mrCateTitle : DbObjectModel<mrCateTitle>
        {
            public abstract string Name { get; set; }
            [BelongsTo] public abstract mrCategory Category { get; set; }
            [HasMany] public abstract IList<mrCateTitleName> Names { get; set; }
            public mrCateTitle Init(string name) { Name = name; return this; }
        }

        public abstract class mrCateTitleName : DbObjectModel<mrCateTitleName>
        {
            public abstract string Name { get; set; }
            [BelongsTo] public abstract mrCateTitle Owner { get; set; }
            public mrCateTitleName Init(string name) { Name = name; return this; }
        }

        #endregion

        #region simulate many to many

        public abstract class mrReader : DbObjectModel<mrReader>
        {
            public abstract string Name { get; set; }
            [HasMany] public abstract IList<mrReaderAndArticle> xTable { get; set; }
            public abstract mrReader Init(string name);
        }

        public abstract class mrReaderAndArticle : DbObjectModel<mrReaderAndArticle>
        {
            public abstract string Name { get; set; }
            [BelongsTo] public abstract mrReader Reader { get; set; }
            [BelongsTo] public abstract mrArticle Article { get; set; }
            public abstract mrReaderAndArticle Init(string name);
        }

        public abstract class mrArticle : DbObjectModel<mrArticle>
        {
            public abstract string Name { get; set; }
            [HasMany] public abstract IList<mrReaderAndArticle> xTable { get; set; }
            public abstract mrArticle Init(string name);
        }

        #endregion

        [Test]
        public void TestHasManyAndHasOne1()
        {
            mrUser u = mrUser.New.Init("user");
            mrProject p = mrProject.New.Init("project");
            u.Projects.Add(p);
            Assert.AreEqual("user", p.Owner.Name);
            mrSubject s = mrSubject.New.Init("subject");
            p.Subjects.Add(s);
            mrAttribute a = mrAttribute.New.Init("attribute");
            s.Attributes.Add(a);
            mrTitle t = mrTitle.New.Init("title");
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
            mrUser u = mrUser.New.Init("user");
            mrProject p = mrProject.New.Init("project");
            u.Projects.Add(p);
            Assert.AreEqual("user", p.Owner.Name);
            mrSubject s = mrSubject.New.Init("subject");
            p.Subjects.Add(s);
            mrAttribute a = mrAttribute.New.Init("attribute");
            s.Attributes.Add(a);
            mrTitle t = mrTitle.New.Init("title");
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
            mrBook b = mrBook.New.Init("book");
            mrCategory c = mrCategory.New.Init("category");
            b.Categories.Add(c);
            mrCateTitle t = mrCateTitle.New.Init("title");
            c.Title = t;
            mrCateTitleName n = mrCateTitleName.New.Init("name");
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
            mrBook b = mrBook.New.Init("book");
            mrCategory c = mrCategory.New.Init("category");
            b.Categories.Add(c);
            Assert.AreEqual(1, c.Books.Count);
            mrCateTitle t = mrCateTitle.New.Init("title");
            c.Title = t;
            mrCateTitleName n = mrCateTitleName.New.Init("name");
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
            mrReader r = mrReader.New.Init("reader");
            mrArticle a = mrArticle.New.Init("article");
            mrReaderAndArticle ra = mrReaderAndArticle.New.Init("x");
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
            mrReader r = mrReader.New.Init("reader");
            mrArticle a = mrArticle.New.Init("article");
            mrReaderAndArticle ra = mrReaderAndArticle.New.Init("x");
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
