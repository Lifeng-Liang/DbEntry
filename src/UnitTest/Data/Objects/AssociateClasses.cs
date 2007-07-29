
#region usings

using System;
using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Definition;

#endregion

namespace org.hanzify.llf.UnitTest.Data.Objects
{
    // HasOne

    [DbTable("People")]
    public class Person : DbObject
    {
        public string Name = null;
        public HasOne<PersonalComputer> PC;
    }

    [DbTable("PCs")]
    public class PersonalComputer : DbObject
    {
        public string Name = null;

        [DbColumn("Person_Id")]
        public BelongsTo<Person> Owner = null;
    }

    // HasMany

    [DbTable("Books")]
    public class Book : DbObject
    {
        public string Name = null;

        [DbColumn("Category_Id")]
        public BelongsTo<Category> CurCategory = null;
    }

    [DbTable("Categories")]
    public class Category : DbObject
    {
        public string Name = null;
        public HasMany<Book> Books = new HasMany<Book>(new OrderBy("Id"));
    }

    public class Article : DbObject
    {
        public string Name = null;

        [DbColumn("Reader_Id")]
        public HasManyAndBelongsTo<Reader> Readers = new HasManyAndBelongsTo<Reader>(new OrderBy("Id"));

        public Article() { }
        public Article(string Name) { this.Name = Name; }
    }

    public class Reader : DbObject
    {
        public string Name = null;

        [DbColumn("Article_Id")]
        public HasManyAndBelongsTo<Article> Articles = new HasManyAndBelongsTo<Article>(new OrderBy("Id"));

        public Reader() { }
        public Reader(string Name) { this.Name = Name; }
    }

    public class Article_Reader
    {
        public long Article_Id;
        public long Reader_Id;
    }
}
