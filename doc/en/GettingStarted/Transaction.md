Transaction
==========

After the basic operation, let's continue to learn how to use transaction.

1. First, create a new *c:\Test\test.mdb* file.

2. Open *Program.cs* and change it as following:

````c#
using System;
using Leafing.Data;
 
class Program
{
    static void Main()
    {
        // Insert
        new User{Name = "tom"}.Save();
        ShowAll("Insert tom :");
        DbEntry.UsingTransaction(delegate
        {
            new User{Name = "jerry"}.Save();
            new User{Name = "mike"}.Save();
        });
        ShowAll("Insert jerry and mike :");
        // Insert and excepton
        try
        {
            DbEntry.UsingTransaction(delegate
            {
                new User{Name = "rose"}.Save();
                new User{Name = "bill"}.Save();
                int n = 0;
                n = 5 / n; // emulate a exception
            });
        }
        catch {}
        ShowAll("Inserte rose and bill, but has exception :");
 
        Console.ReadLine();
    }
 
    static void ShowAll(string msg)
    {
        Console.WriteLine(msg);
        foreach(User o in User.FindAll(new OrderBy("Id")))
        {
            Console.WriteLine(o);
        }
        Console.WriteLine();
    }
}
````

3. Run this application, it will shows:

````
Insert tom :
{ Id = 1, Name = tom }

Insert jerry and mike :
{ Id = 1, Name = tom }
{ Id = 2, Name = jerry }
{ Id = 3, Name = mike }

Inserte rose and bill, but has exception :
{ Id = 1, Name = tom }
{ Id = 2, Name = jerry }
{ Id = 3, Name = mike }
````

4. Open the *c:\Test\test.mdb* file, confirm the *User* table is following: 

| Id    | Name  |
| ----- | ----- |
| 1     | tom   |
| 2     | jerry |
| 3     | mike  |
