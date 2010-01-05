using System;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;

namespace Debug
{
    public abstract class T1 : DbObjectModel<T1>
    {
        public abstract double Price { get; set; }
    }


    public abstract class T2 : DbObjectModel<T2>
    {
        public abstract long T1Id { get; set; }
    }

    public class TM : IDbObject
    {
        public double Price { get; set; }
    }

    class Program
    {
        static void Main()
        {
            DbEntry.Context.DropAndCreate(typeof(T1));
            DbEntry.Context.DropAndCreate(typeof(T2));

            var t1 = T1.New;
            t1.Price = 0;
            t1.Save();

            var t2 = T2.New;
            t2.T1Id = 1;
            t2.Save();

            var m1 = T1.FindAll();
DbEntry.Context.ExecuteDataReader(new SqlStatement("SELECT Price FROM T1 WHERE Id = 1;"),
    dr =>
    {
        dr.Read();
        var p = dr["Price"];
        Console.WriteLine(p.GetType());
        Console.WriteLine(p);
    });
DbEntry.Context.ExecuteDataReader(new SqlStatement("SELECT T1.Price as Price FROM T1 INNER JOIN  T2 on T1.Id = T2.T1Id;"),
    dr =>
    {
        dr.Read();
        var p = dr["Price"];
        Console.WriteLine(p.GetType());
        Console.WriteLine(p);
    });

            Console.ReadLine();
        }
    }
}