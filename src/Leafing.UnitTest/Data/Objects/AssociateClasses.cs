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
    }

    [DbTable("PCs")]
    public class PersonalComputer : DbObjectModel<PersonalComputer>
    {
        public string Name { get; set; }

        [DbColumn("Person_Id")]
        public BelongsTo<Person, long> Owner;
    }

    [DbTable("People"), DbContext("SQLite")]
    public class PersonSqlite : DbObjectModel<PersonSqlite>
    {
        public string Name { get; set; }

        public HasOne<PersonalComputerSqlite> PC;
    }

    [DbTable("PCs"), DbContext("SQLite")]
    public class PersonalComputerSqlite : DbObjectModel<PersonalComputerSqlite>
    {
        public string Name { get; set; }

        [DbColumn("Person_Id")]
        public BelongsTo<PersonSqlite, long> Owner;
    }

    [DbTable("People"), DbContext("SqlServerMock")]
    public class PersonSql : DbObjectModel<PersonSql>
    {
        public string Name { get; set; }

        public HasOne<PersonalComputerSql> PC;
    }

    [DbTable("PCs"), DbContext("SqlServerMock")]
    public class PersonalComputerSql : DbObjectModel<PersonalComputerSql>
    {
        public string Name { get; set; }

        [DbColumn("Person_Id")]
        public BelongsTo<PersonSql, long> Owner;
    }

    // HasMany

    [DbTable("Books")]
    public class Book : DbObjectModel<Book>
    {
        public string Name { get; set; }

        [DbColumn("Category_Id")]
        public BelongsTo<Category, long> CurCategory;
    }

    [DbTable("Categories")]
    public class Category : DbObjectModel<Category>
    {
        public string Name { get; set; }

        public HasMany<Book> Books;
    }

    [DbTable("Books"), DbContext("SQLite")]
    public class BookSqlite : DbObjectModel<BookSqlite>
    {
        public string Name { get; set; }

        [DbColumn("Category_Id")]
        public BelongsTo<CategorySqlite, long> CurCategory;
    }

    [DbTable("Categories"), DbContext("SQLite")]
    public class CategorySqlite : DbObjectModel<CategorySqlite>
    {
        public string Name { get; set; }

        public HasMany<BookSqlite> Books;
    }

    [DbTable("Categories")]
    public class Acategory : DbObjectModel<Acategory>
    {
        public string Name { get; set; }

		public HasMany<Abook> Books { get; private set; }
    }

    [DbTable("Books")]
    public class Abook : DbObjectModel<Abook>
    {
        public string Name { get; set; }

        [DbColumn("Category_Id")]
		public BelongsTo<Acategory, long> CurCategory { get; private set; }
    }

    public class Article : DbObjectModel<Article>
    {
        public string Name { get; set; }

		[OrderBy("Id")]
		public HasAndBelongsToMany<Reader> Readers { get; private set; }
    }

    public class Reader : DbObjectModel<Reader>
    {
        public string Name { get; set; }

		[OrderBy("Id")]
		public HasAndBelongsToMany<Article> Articles { get; private set; }
    }

    [DbTable("Article"), DbContext("SQLite")]
    public class ArticleSqlite : DbObjectModel<ArticleSqlite>
    {
        public string Name { get; set; }

		public HasAndBelongsToMany<ReaderSqlite> Readers { get; private set; }
    }

    [DbTable("Reader"), DbContext("SQLite")]
    public class ReaderSqlite : DbObjectModel<ReaderSqlite>
    {
        public string Name { get; set; }

		public HasAndBelongsToMany<ArticleSqlite> Articles { get; private set; }
    }

    public class Article_Reader : IDbObject
    {
        public long Article_Id;
        public long Reader_Id;
    }
}
