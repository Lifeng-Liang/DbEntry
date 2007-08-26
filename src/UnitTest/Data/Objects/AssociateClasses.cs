
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

        public Person()
        {
            PC = new HasOne<PersonalComputer>(this, "");
        }
    }

    [DbTable("PCs")]
    public class PersonalComputer : DbObject
    {
        public string Name = null;

        [DbColumn("Person_Id")]
        public BelongsTo<Person> Owner;

        public PersonalComputer()
        {
            Owner = new BelongsTo<Person>(this);
        }
    }

    // HasMany

    [DbTable("Books")]
    public class Book : DbObject
    {
        public string Name = null;

        [DbColumn("Category_Id")]
        public BelongsTo<Category> CurCategory;

        public Book()
        {
            CurCategory = new BelongsTo<Category>(this);
        }
    }

    [DbTable("Categories")]
    public class Category : DbObject
    {
        public string Name = null;
        public HasMany<Book> Books;

        public Category()
        {
            Books = new HasMany<Book>(this, "Id");
        }
    }

    public class Article : DbObject
    {
        public string Name = null;

        [DbColumn("Reader_Id")]
        public HasAndBelongsToMany<Reader> Readers;

        public Article()
        {
            Readers = new HasAndBelongsToMany<Reader>(this, new OrderBy("Id"));
        }
        public Article(string Name) : this() { this.Name = Name; }
    }

    public class Reader : DbObject
    {
        public string Name = null;

        [DbColumn("Article_Id")]
        public HasAndBelongsToMany<Article> Articles;

        public Reader()
        {
            Articles = new HasAndBelongsToMany<Article>(this, new OrderBy("Id"));
        }
        public Reader(string Name) : this() { this.Name = Name; }
    }

    public class Article_Reader
    {
        public long Article_Id;
        public long Reader_Id;
    }
}
