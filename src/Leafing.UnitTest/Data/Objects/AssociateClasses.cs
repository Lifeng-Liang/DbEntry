using System.Collections.Generic;
using Leafing.Data.Definition;

namespace Leafing.UnitTest.Data.Objects
{
    // HasOne

    [DbTable("People")]
    public class Person : DbObjectModel<Person>
    {
        public string Name { get; set; }

        public HasOne<PersonalComputer> PC;

        public Person()
        {
            PC = new HasOne<PersonalComputer>(this);
        }
    }

    [DbTable("PCs")]
    public class PersonalComputer : DbObjectModel<PersonalComputer>
    {
        public string Name { get; set; }

        [DbColumn("Person_Id")]
        public BelongsTo<Person, long> Owner;

        public PersonalComputer()
        {
            Owner = new BelongsTo<Person, long>(this);
        }
    }

    [DbTable("People"), DbContext("SQLite")]
    public class PersonSqlite : DbObjectModel<PersonSqlite>
    {
        public string Name { get; set; }

        public HasOne<PersonalComputerSqlite> PC;

        public PersonSqlite()
        {
            PC = new HasOne<PersonalComputerSqlite>(this);
        }
    }

    [DbTable("PCs"), DbContext("SQLite")]
    public class PersonalComputerSqlite : DbObjectModel<PersonalComputerSqlite>
    {
        public string Name { get; set; }

        [DbColumn("Person_Id")]
        public BelongsTo<PersonSqlite, long> Owner;

        public PersonalComputerSqlite()
        {
            Owner = new BelongsTo<PersonSqlite, long>(this);
        }
    }

    [DbTable("People"), DbContext("SqlServerMock")]
    public class PersonSql : DbObjectModel<PersonSql>
    {
        public string Name { get; set; }

        public HasOne<PersonalComputerSql> PC;

        public PersonSql()
        {
            PC = new HasOne<PersonalComputerSql>(this);
        }
    }

    [DbTable("PCs"), DbContext("SqlServerMock")]
    public class PersonalComputerSql : DbObjectModel<PersonalComputerSql>
    {
        public string Name { get; set; }

        [DbColumn("Person_Id")]
        public BelongsTo<PersonSql, long> Owner;

        public PersonalComputerSql()
        {
            Owner = new BelongsTo<PersonSql, long>(this);
        }
    }

    // HasMany

    [DbTable("Books")]
    public class Book : DbObjectModel<Book>
    {
        public string Name { get; set; }

        [DbColumn("Category_Id")]
        public BelongsTo<Category, long> CurCategory;

        public Book()
        {
            CurCategory = new BelongsTo<Category, long>(this);
        }
    }

    [DbTable("Categories")]
    public class Category : DbObjectModel<Category>
    {
        public string Name { get; set; }

        public HasMany<Book> Books;

        public Category()
        {
            Books = new HasMany<Book>(this);
        }
    }

    [DbTable("Books"), DbContext("SQLite")]
    public class BookSqlite : DbObjectModel<BookSqlite>
    {
        public string Name { get; set; }

        [DbColumn("Category_Id")]
        public BelongsTo<CategorySqlite, long> CurCategory;

        public BookSqlite()
        {
            CurCategory = new BelongsTo<CategorySqlite, long>(this);
        }
    }

    [DbTable("Categories"), DbContext("SQLite")]
    public class CategorySqlite : DbObjectModel<CategorySqlite>
    {
        public string Name { get; set; }

        public HasMany<BookSqlite> Books;

        public CategorySqlite()
        {
            Books = new HasMany<BookSqlite>(this);
        }
    }

    [DbTable("Categories")]
    public class Acategory : DbObjectModel<Acategory>
    {
        public string Name { get; set; }

		public HasMany<Abook> Books { get; private set; }

		public Acategory ()
		{
			Books = new HasMany<Abook> (this);
		}
    }

    [DbTable("Books")]
    public class Abook : DbObjectModel<Abook>
    {
        public string Name { get; set; }

        [DbColumn("Category_Id")]
		public BelongsTo<Acategory, long> CurCategory { get; private set; }

		public Abook ()
		{
			CurCategory = new BelongsTo<Acategory, long> (this);
		}
    }

    public class Article : DbObjectModel<Article>
    {
        public string Name { get; set; }

		public HasAndBelongsToMany<Reader> Readers { get; private set; }

		public Article ()
		{
			Readers = new HasAndBelongsToMany<Reader> (this);
		}
    }

    public class Reader : DbObjectModel<Reader>
    {
        public string Name { get; set; }

		public HasAndBelongsToMany<Article> Articles { get; private set; }

		public Reader ()
		{
			Articles = new HasAndBelongsToMany<Article> (this);
		}
    }

    [DbTable("Article"), DbContext("SQLite")]
    public class ArticleSqlite : DbObjectModel<ArticleSqlite>
    {
        public string Name { get; set; }

		public HasAndBelongsToMany<ReaderSqlite> Readers { get; private set; }

		public ArticleSqlite ()
		{
			Readers = new HasAndBelongsToMany<ReaderSqlite> (this);
		}
    }

    [DbTable("Reader"), DbContext("SQLite")]
    public class ReaderSqlite : DbObjectModel<ReaderSqlite>
    {
        public string Name { get; set; }

		public HasAndBelongsToMany<ArticleSqlite> Articles { get; private set; }

		public ReaderSqlite ()
		{
			Articles = new HasAndBelongsToMany<ArticleSqlite> (this);
		}
    }

    public class Article_Reader : IDbObject
    {
        public long Article_Id;
        public long Reader_Id;
    }
}
