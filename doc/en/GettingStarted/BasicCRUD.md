Basic CRUD
==========

In [First Application](FirstApplication.md), it shows how to insert a new object to database:

````c#
    var u = new User {Name = "tom"};
    u.Save();
````

Now, let's continue this work to change this code to complete basic CRUD for database.

1\. First, make sure the *test.db* has *User* table, and the table data is following:

| Id    | Name  |
| ----- | ----- |
| 1     | tom   |
| 2     | tom   |

2\. Open *Program.cs* and change it as following:

````c#
using System;
using Leafing.Data;
 
class Program
{
    static void Main(string[] args)
    {
        var u = User.FindById(1);
        Console.WriteLine(u);
        Console.ReadLine();
    }
}
````

3\. Run this application, it will show:

````
{ Id = 1, Name = tom }
````

4\. Edit the *Program.cs* as following:

````c#
using System;
using Leafing.Data;
 
class Program
{
    static void Main(string[] args)
    {
        var u = User.FindById(1);
        Console.WriteLine("Read Object:\n{0}", u);
        u.Name = "jerry";
        u.Save();
        var u1 = User.FindById(1);
        Console.WriteLine("Updated Object:\n{0}", u1);
        Console.ReadLine();
    }
}
````

5\. Run this application, it will show:

````
Read Object:
{ Id = 1, Name = tom }
Updated Object:
{ Id = 1, Name = jerry }
````

6\. Edit the *Program.cs* as following:

````c#
using System;
using Leafing.Data;
 
class Program
{
    static void Main(string[] args)
    {
        var u = User.FindById(2);
        Console.WriteLine("Read Object:\n{0}", u);
        u.Delete();
        var u1 = User.FindById(2);
        if (u1 == null)
        {
            Console.WriteLine("After delete, the object doesn't find.");
        }
        Console.ReadLine();
    }
}
````

7\. Run this application, it will show:

````
Read Object:
{ Id = 2, Name = tom }
After delete, the object doesn't find.
````

8\. Open the *test.db* file, confirm the *User* table is following:

| Id    | Name  |
| ----- | ----- |
| 1     | jerry |

9\. Ok, we just finished the basic CRUD, have fun!


