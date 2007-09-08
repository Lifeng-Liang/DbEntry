
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
            Console.WriteLine("====Has And Belongs To Many====");
            //DbEntry.Context.DropAndCreate(typeof(Article));
            //DbEntry.Context.DropAndCreate(typeof(Reader));
            //DbEntry.Context.CreateManyToManyMediTable(typeof(Article), typeof(Reader));

            Article a = Article.New("fly away");
            a.Readers.Add(Reader.New("Kingkong"));
            a.Readers.Add(Reader.New("Spiderman"));
            a.Save();
            Article a1 = Article.FindById(a.Id);
            ShowHasManyAndBelongsTo(a1);

            Console.WriteLine("Done! Press Enter to exit.");
            Console.ReadLine();
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
    }
}
