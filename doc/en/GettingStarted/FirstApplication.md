First Application
==========

The following steps shows how to create a basic database application using DbEntry.Net:
----------

1\. Create a console application in Visual Studio 2010/2012/2013/2015.

2\. Create a console application.

3\. Create a new Access mdb file named *test.mdb*, and store it to *c:\Test* (Make sure it exists). If you are using Access 2007, please save it as Access 2003 format.

4\. Add the references of *Leafing.Data.dll* and *Leafing.Core.dll* to this application.

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
    <add key="DataBase" value="@Access : @C:\Test\test.mdb" />
  </Leafing.Settings>
  <startup useLegacyV2RuntimeActivationPolicy="true">
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/>
  </startup>
</configuration>
````

6\. Add a class file named *User.cs* to the project. And change the code to:

````c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

9\. Check *c:\Test\test.mdb* file, confirm the *User* table already created and it has one row data, check the data of this row, confirm the *Name* column is *tom*:

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

11\. Check the *c:\test.mdb* file, confirm the data of *User* table is following:

| Id    | Name  |
| ----- | ----- |
| 1     | tom   |
| 2     | tom   |
