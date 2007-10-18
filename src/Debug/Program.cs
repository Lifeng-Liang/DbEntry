
#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.SqlEntry;
using Lephone.Data.Definition;

#endregion

namespace Orm9
{
    [DbTable("People")]
    public abstract class Person : DbObjectModel<Person>
    {
        [Length(50)]
        public abstract string Name { get; set; }

        [HasOne(OrderBy = "Id DESC")]
        public abstract PersonalComputer PC { get; set; }

        public Person() { }
        public Person(string Name) { this.Name = Name; }
    }

    [DbTable("PCs")]
    public abstract class PersonalComputer : DbObjectModel<PersonalComputer>
    {
        [Length(50)]
        public abstract string Name { get; set; }

        [BelongsTo, DbColumn("Person_Id")]
        public abstract Person Owner { get; set; }

        public PersonalComputer() { }
        public PersonalComputer(string Name) { this.Name = Name; }
    }

    [DbTable("Books")]
    public abstract class Book : DbObjectModel<Book>
    {
        [Length(50)]
        public abstract string Name { get; set; }

        [BelongsTo, DbColumn("Category_Id")]
        public abstract Category Category { get; set; }

        public Book() { }
        public Book(string Name) { this.Name = Name; }
    }

    [DbTable("Categories")]
    public abstract class Category : DbObjectModel<Category>
    {
        [Length(50)]
        public abstract string Name { get; set; }

        [HasMany(OrderBy = "Id")]
        public abstract HasMany<Book> Books { get; set; }

        public Category() { }
        public Category(string Name) { this.Name = Name; }
    }

    public abstract class Article : DbObjectModel<Article>
    {
        public abstract string Name { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id")]
        public abstract IList<Reader> Readers { get; set; }

        public Article() { }
        public Article(string Name) { this.Name = Name; }
    }

    public abstract class Reader : DbObjectModel<Reader>
    {
        public abstract string Name { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id")]
        public abstract IList<Article> Articles { get; set; }

        public Reader() { }
        public Reader(string Name) { this.Name = Name; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // RebuildTables();

            Console.WriteLine("Relationship objects and Auto create table");
            Console.WriteLine("============Has One============");
            ShowHasOne(DbEntry.GetObject<Person>(1));
            ShowHasOne(DbEntry.GetObject<Person>(2));
            ShowHasOne(DbEntry.GetObject<Person>(3));

            Console.WriteLine("============Has Many===========");
            ShowHasMany(DbEntry.GetObject<Category>(1));
            ShowHasMany(DbEntry.GetObject<Category>(2));
            ShowHasMany(DbEntry.GetObject<Category>(3));

            Console.WriteLine("===========Update One==========");
            Person p = DbEntry.GetObject<Person>(2);
            Console.WriteLine(">>Before:");
            ShowHasOne(p);
            p.Name = "Neo";
            p.PC.Name = "Matrix";
            DbEntry.Save(p);
            Console.WriteLine(">>After:");
            ShowHasOne(DbEntry.GetObject<Person>(2));
            Console.WriteLine(">>Delete:");
            DbEntry.Delete(p);
            ShowHasOne(DbEntry.GetObject<Person>(2));

            Console.WriteLine("==========Update Many==========");
            Category c = DbEntry.GetObject<Category>(3);
            Console.WriteLine(">>Before:");
            ShowHasMany(c);
            Console.WriteLine(">>After:");
            c.Name = "Sport";
            c.Books[0].Name = "Kungfu";
            c.Books[1].Name = "Dodge the bullets";
            DbEntry.Save(c);
            ShowHasMany(DbEntry.GetObject<Category>(3));
            Console.WriteLine(">>Delete:");
            DbEntry.Delete(c);
            ShowHasMany(DbEntry.GetObject<Category>(3));

            Console.WriteLine("====Has And Belongs To Many====");
            Article a = Article.New("fly away");
            a.Readers.Add(Reader.New("Kingkong"));
            a.Readers.Add(Reader.New("Spiderman"));
            a.Save();
            Article a1 = Article.FindById(a.Id);
            ShowHasManyAndBelongsTo(a1);

            Console.WriteLine("======Restoring Tables...======");
            RebuildTables();
            Console.WriteLine("============The End============");
            Console.WriteLine("Done! Press Enter to exit.");
            Console.ReadLine();
        }

        private static void RebuildTables()
        {
            DbContext de = DbEntry.Context;
            de.DropTable(typeof(Person));
            de.DropTable(typeof(PersonalComputer));
            de.DropTable(typeof(Category));
            de.DropTable(typeof(Book));
            de.DropTable(typeof(Article));
            de.DropTable(typeof(Reader));
            DbEntry.Save(Person.New("Tom"));
            Person p = Person.New("Jerry");
            p.PC = PersonalComputer.New("IBM");
            DbEntry.Save(p);
            p = Person.New("Mike");
            p.PC = PersonalComputer.New("DELL");
            DbEntry.Save(p);
            p.PC = PersonalComputer.New("HP");
            DbEntry.Save(p);
            DbEntry.Save(Category.New("Tech"));
            Category c = Category.New("Game");
            c.Books.Add(Book.New("Diablo"));
            DbEntry.Save(c);
            Category c1 = Category.New("Tour");
            c1.Books.Add(Book.New("Beijing"));
            c1.Books.Add(Book.New("Shanghai"));
            DbEntry.Save(c1);
            c.Books.Clear();
            c.Books.Add(Book.New("Pal95"));
            c.Books.Add(Book.New("Wow"));
            DbEntry.Save(c);
        }

        private static void ShowHasMany(Category c)
        {
            if (c == null)
            {
                Console.WriteLine("(NULL Category)");
            }
            else
            {
                Console.Write(c.Name);
                Console.WriteLine("=>");
                foreach (Book b in c.Books)
                {
                    Console.WriteLine("\t{0}", b.Name);
                }
            }
        }

        private static void ShowHasManyAndBelongsTo(Article c)
        {
            if (c == null)
            {
                Console.WriteLine("(NULL Article)");
            }
            else
            {
                Console.Write(c.Name);
                Console.WriteLine("=>");
                foreach (Reader b in c.Readers)
                {
                    Console.WriteLine("\t{0}", b.Name);
                }
            }
        }

        private static void ShowHasOne(Person p)
        {
            if (p == null)
            {
                Console.WriteLine("(NULL Person)");
            }
            else
            {
                Console.Write(p.Name);
                Console.Write("=>\t");
                if (p.PC != null)
                {
                    Console.WriteLine(p.PC.Name);
                }
                else
                {
                    Console.WriteLine("(NULL)");
                }
            }
        }
    }
}
