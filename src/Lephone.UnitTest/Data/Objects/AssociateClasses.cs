using System.Collections.Generic;
using Lephone.Data.Definition;

namespace Lephone.UnitTest.Data.Objects
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

    [DbTable("Categories")]
    public abstract class Acategory : DbObjectModel<Acategory>
    {
        public abstract string Name { get; set; }
        [HasMany]
        public abstract IList<Abook> Books { get; set; }
    }

    [DbTable("Books")]
    public abstract class Abook : DbObjectModel<Abook>
    {
        public abstract string Name { get; set; }
        [BelongsTo, DbColumn("Category_Id")]
        public abstract Acategory CurCategory { get; set; }
    }

    public abstract class Article : DbObjectModel<Article>
    {
        public abstract string Name { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id")]
        public abstract IList<Reader> Readers { get; set; }

        public Article Init(string Name) { this.Name = Name; return this; }
    }

    public abstract class Reader : DbObjectModel<Reader>
    {
        public abstract string Name { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id")]
        public abstract IList<Article> Articles { get; set; }

        public Reader Init(string Name) { this.Name = Name; return this; }
    }

    public class Article_Reader : IDbObject
    {
        public long Article_Id;
        public long Reader_Id;
    }
}
