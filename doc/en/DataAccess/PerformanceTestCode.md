Performance Test Code
==========

````c#
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Definition;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.util;

public class TestTable
{
    [DbKey] public int id;
    public string UserName;
    public string EMail;
    public string HomePage;
}

class Program
{
    static void Main(string[] args)
    {
        RunAdoNetWithIndex();
        RunAdoNetWithIndex();
        RunAdoNetWithName();
        RunAdoNetWithName();
        RunAdoNetWithNameDontInsertToList();
        RunAdoNetWithNameDontInsertToList();
        RunDbEntry();
        RunDbEntry();
        RunDbEntryWithSQL();
        RunDbEntryWithSQL();
        RunDataSet();
        RunDataSet();

        Console.WriteLine("All end.");
        Console.ReadLine();
    }

    static void RunAdoNetWithIndex()
    {
        Console.WriteLine("Run ADO.NET with index:");
        TimeSpanCounter tc = new TimeSpanCounter();

        SqlStatement sql = new SqlStatement(
            "select [id],[UserName],[EMail],[HomePage] from [TestTable]");
        List<TestTable> l = new List<TestTable>();
        DbEntry.Context.ExecuteDataReader(sql, delegate(IDataReader dr)
        {
            while (dr.Read())
            {
                TestTable t = new TestTable();
                t.id = (int)dr[0];
                t.UserName = (string)dr[1];
                t.EMail = (string)dr[2];
                t.HomePage = (string)dr[3];
                l.Add(t);
            }
        });

        Console.WriteLine(tc);
        Console.WriteLine();
    }

    static void RunAdoNetWithName()
    {
        Console.WriteLine("Run ADO.NET with name:");
        TimeSpanCounter tc = new TimeSpanCounter();

        SqlStatement sql = new SqlStatement(
            "select [id],[UserName],[EMail],[HomePage] from [TestTable]");
        List<TestTable> l = new List<TestTable>();
        DbEntry.Context.ExecuteDataReader(sql, delegate(IDataReader dr)
        {
            while (dr.Read())
            {
                TestTable t = new TestTable();
                t.id = (int)dr["id"];
                t.UserName = (string)dr["UserName"];
                t.EMail = (string)dr["EMail"];
                t.HomePage = (string)dr["HomePage"];
                l.Add(t);
            }
        });

        Console.WriteLine(tc);
        Console.WriteLine();
    }

    static void RunAdoNetWithNameDontInsertToList()
    {
        Console.WriteLine("Run ADO.NET with name don't insert to list:");
        TimeSpanCounter tc = new TimeSpanCounter();

        SqlStatement sql = new SqlStatement(
            "select [id],[UserName],[EMail],[HomePage] from [TestTable]");
        DbEntry.Context.ExecuteDataReader(sql, delegate(IDataReader dr)
        {
            while (dr.Read())
            {
                TestTable t = new TestTable();
                t.id = (int)dr["id"];
                t.UserName = (string)dr["UserName"];
                t.EMail = (string)dr["EMail"];
                t.HomePage = (string)dr["HomePage"];
            }
        });

        Console.WriteLine(tc);
        Console.WriteLine();
    }

    static void RunDbEntry()
    {
        Console.WriteLine("Run DbEntry:");
        TimeSpanCounter tc = new TimeSpanCounter();

        List<TestTable> l = DbEntry.From<TestTable>().Where(null).Select();

        Console.WriteLine(tc);
        Console.WriteLine();
    }

    static void RunDbEntryWithSQL()
    {
        Console.WriteLine("Run DbEntry with SQL:");
        TimeSpanCounter tc = new TimeSpanCounter();

        List<TestTable> l = DbEntry.Context.ExecuteList<TestTable>(
            "select [id],[UserName],[EMail],[HomePage] from [TestTable]");

        Console.WriteLine(tc);
        Console.WriteLine();
    }

    static void RunDataSet()
    {
        Console.WriteLine("Run DataSet:");
        TimeSpanCounter tc = new TimeSpanCounter();

        DataSet ds = DbEntry.Context.ExecuteDataset(
            "select [id],[UserName],[EMail],[HomePage] from [TestTable]");

        Console.WriteLine(tc);
        Console.WriteLine();
    }
}
````
