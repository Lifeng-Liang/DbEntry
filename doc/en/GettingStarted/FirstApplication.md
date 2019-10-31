First Application
==========

The following steps shows how to create a basic database application using DbEntry.Net:

1\. Create a console application in Visual Studio 2015/2017.

2\. Create a console application with .Net 4.6.1 or higher.

3\. In Nuget Package Manager Console, enter Install-Package DbEntry.Net.

4\. In Nuget Package Manager Console, enter Install-Package NSQLite.

5\. Add an *App.config* to this application. Open it in Visual Studio, and change it as following:

````xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="Leafing.Settings"
      type="Leafing.Core.Setting.NameValueSectionHandler, Leafing.Core" />
  </configSections>

  <Leafing.Settings>
    <add key="AutoScheme" value="CreateTable" />
    <add key="DataBase" value="@NSQLite : @~test.db" />
    <add key="DbProviderFactory" value="System.Data.SQLite.SQLiteClientFactory, System.Data.SQLite, Version=4.0.0.0, Culture=neutral, PublicKeyToken=c7316bd79fc5e65e"/>
  </Leafing.Settings>
</configuration>
````

6\. Add a class file named *User.cs* to the project. And change the code to:

````c#
using Leafing.Data.Definition;

public class User : DbObjectModel<User>
{
    public string Name { get; set; }
}
````

7\. Open *Program.cs* and change it as following:

````c#
using System;
using Leafing.Data;

class Program
{
    static void Main(string[] args)
    {
        var u = new User {Name = "tom"};
        Console.WriteLine("New object:\n{0}", u);
        u.Save();
        Console.WriteLine("Saved object:\n{0}", u);
        Console.ReadLine();
    }
}
````

8\. Run this application, it will shows:

````
New object:
{ Id = 0, Name = tom }
Saved object:
{ Id = 1, Name = tom }
````

9\. Use a SQLite manage software (SQLQuerier works here) to check *test.db* file, confirm the *User* table already created and it has one row data, check the data of this row, confirm the *Name* column is *tom*:

| Id    | Name  |
| ----- | ----- |
| 1     | tom   |

10\. Re-run this application, it will shows:

````
New object:
{ Id = 0, Name = tom }
Saved object:
{ Id = 2, Name = tom }
````

11\. Check the *test.db* file, confirm the data of *User* table is following:

| Id    | Name  |
| ----- | ----- |
| 1     | tom   |
| 2     | tom   |
