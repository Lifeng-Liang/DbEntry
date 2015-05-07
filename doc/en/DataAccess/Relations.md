Relations
==========

DbEntry supported 1:1, Many:1, 1:Many and Many:Many relations. And they named as HasOne, BelongsTo, HasMany and HasAndBelongsToMany just like they in RoR Active Record. I think it's more readable.

All of the relation objects use lazy loading. If we have a ``read`` operation in our code for relation object such as ``book.reader.Length``, it will cause read from database.  For HasMany and HasAndBelongsToMany, if we insert a new value into the relation list before it read from database, it will not read it again.

I recommended reading the unit tests to find out how to use the relation objects. The development of relations is based on TDD _(Test Driven Development)_ , so the unit tests have everything about relations.

````c#
using System;
using System.Collections.Generic;
using Leafing.Data;
using Leafing.Data.Definition;

namespace Orm9
{
    [DbTable("People")]
    public class Person : DbObjectModel<Person>
    {
        [Length(50)]
        public string Name { get; set; }

        [HasOne(OrderBy = "Id DESC")]
        public PersonalComputer PC { get; set; }
    }

    [DbTable("PCs")]
    public class PersonalComputer : DbObjectModel<PersonalComputer>
    {
        [Length(50)]
        public string Name { get; set; }

        [BelongsTo, DbColumn("Person_Id")]
        public Person Owner { get; set; }
    }

    [DbTable("Books")]
    public class Book : DbObjectModel<Book>
    {
        [Length(50)]
        public string Name { get; set; }

        [BelongsTo, DbColumn("Category_Id")]
        public Category Category { get; set; }
    }

    [DbTable("Categories")]
    public class Category : DbObjectModel<Category>
    {
        [Length(50)]
        public string Name { get; set; }

        [HasMany(OrderBy = "Id")]
        public IList<Book> Books { get; private set; }
    }

    public class Article : DbObjectModel<Article>
    {
        public string Name { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id")]
        public IList<Reader> Readers { get; private set; }
    }

    public class Reader : DbObjectModel<Reader>
    {
        public string Name { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id")]
        public IList<Article> Articles { get; private set; }
    }

    class Program
    {
        static void Main()
        {
            // RebuildTables();

            Console.WriteLine("Relationship objects and Auto create table");
            Console.WriteLine("============Has One============");
            ShowHasOne(Person.FindById(1));
            ShowHasOne(Person.FindById(2));
            ShowHasOne(Person.FindById(3));

            Console.WriteLine("============Has Many===========");
            ShowHasMany(Category.FindById(1));
            ShowHasMany(Category.FindById(2));
            ShowHasMany(Category.FindById(3));

            Console.WriteLine("===========Update One==========");
            var p = Person.FindById(2);
            Console.WriteLine(">>Before:");
            ShowHasOne(p);
            p.Name = "Neo";
            p.PC.Name = "Matrix";
            p.Save();
            Console.WriteLine(">>After:");
            ShowHasOne(Person.FindById(2));
            Console.WriteLine(">>Delete:");
            p.Delete();
            ShowHasOne(Person.FindById(2));

            Console.WriteLine("==========Update Many==========");
            var c = Category.FindById(3);
            Console.WriteLine(">>Before:");
            ShowHasMany(c);
            Console.WriteLine(">>After:");
            c.Name = "Sport";
            c.Books[0].Name = "Kungfu";
            c.Books[1].Name = "Dodge the bullets";
            c.Save();
            ShowHasMany(Category.FindById(3));
            Console.WriteLine(">>Delete:");
            c.Delete();
            ShowHasMany(Category.FindById(3));

            Console.WriteLine("====Has Many And Belongs To====");
            var a = new Article {Name = "fly away"};
            a.Readers.Add(new Reader {Name = "Kingkong"});
            a.Readers.Add(new Reader {Name = "Spiderman"});
            a.Save();
            var a1 = Article.FindById(a.Id);
            ShowHasManyAndBelongsTo(a1);

            Console.WriteLine("======Restoring Tables...======");
            RebuildTables();
            Console.WriteLine("============The End============");
            Console.WriteLine("Done! Press Enter to exit.");
            Console.ReadLine();
        }

        private static void RebuildTables()
        {
            DbEntry.DropAndCreate(typeof(Person));
            DbEntry.DropAndCreate(typeof(PersonalComputer));
            DbEntry.DropAndCreate(typeof(Category));
            DbEntry.DropAndCreate(typeof(Book));
            DbEntry.DropAndCreate(typeof(Article));
            DbEntry.DropAndCreate(typeof(Reader));
            DbEntry.CreateCrossTable(typeof(Article), typeof(Reader));
            DbEntry.Save( new Person {Name = "Tom"});
            var p = new Person {Name = "Jerry", PC = new PersonalComputer {Name = "IBM"``;
            DbEntry.Save(p);
            p = new Person {Name = "Mike", PC = new PersonalComputer {Name = "DELL"``;
            DbEntry.Save(p);
            p.PC = new PersonalComputer {Name = "HP"};
            DbEntry.Save(p);
            DbEntry.Save(new Category {Name = "Tech"});
            var c = new Category {Name = "Game"};
            c.Books.Add(new Book {Name = "Diablo"});
            DbEntry.Save(c);
            var c1 = new Category {Name = "Tour"};
            c1.Books.Add(new Book {Name = "Beijing"});
            c1.Books.Add(new Book {Name = "Shanghai"});
            DbEntry.Save(c1);
            c.Books.Clear();
            c.Books.Add(new Book {Name = "Pal95"});
            c.Books.Add(new Book {Name = "Wow"});
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
                foreach (var b in c.Books)
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
                foreach (var b in c.Readers)
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
````
