Query Syntax
==========

Linq
----------

Object definition:

````c#
public class Person : DbObjectModel<Person>
{
    [DbColumn("Name")]
    public string FirstName { get; set; }
}
````

Operation code:

````c#
var list = from p in Person.Table
    where p.FirstName.StartsWith("T") && (p.Id >= 1 || p.Id == 15)
    order by p.Id select p;

foreach(Person p in list)
{
    Console.WriteLine(p);
}
````

We can also operate it as following:

````c#
var list = DbEntry.From<Person>()
    .Where(p => p.FirstName.StartsWith("T") && (p.Id >= 1 || p.Id == 15))
    .OrderBy(p => p.Id)
    .Select();

foreach(Person p in list)
{
    Console.WriteLine(p);
}
````

The composed SQL just like:

````sql
Select [Id],[Name] From [Person] Where ([Name] Like 'T%') And (([Id] >= 1) Or ([Id] = 15)) Order By [Id] ASC;
````

Or we can use the new style condition and old style order by for query:

````c#
var list = DbEntry.From<Person>()
    .Where(p => p.FirstName.StartsWith("T") && (p.Id >= 1 || p.Id == 15))
    .OrderBy("Id DESC")
    .Select();
````

And select new expression of linq also supported:

````c#
var list = from p in Person.Table
    where p.FirstName.StartsWith("T") && (p.Id >= 1 || p.Id == 15)
    order by p.Id select new { p.Name };
````

With it, we have complie time type check of the sytax like condition, order by etc. And it will provide us intellisense of the fields in IDE.

And we can use it in bussiness layer like:

````c#
public Person FindPerson(string name, string password)
{
    var p = Person.FindOne(p => p.Name == name && p.Password == password);
    return p;
}
````

We can use In function in Linq as well:

````c#
Person.Find(p => p.Id.In(1, 3, 5));
````

And use SQL in InSql function:

````c#
Person.Find(p => p.Id.InSql("Select [UID] From Others"));
````

In most cases we might want to use SqlStatement in InStatement function as:

````c#
var smt = DbEntry.From<PCs>().Where(p => p.Id >= 2).GetStatement(p => p.Id);
var list = DbEntry.From<Person>().Where(CK.K["Id"].InStatement(smt)).OrderBy(p => p.Id).Select();
````

There are NotInXXX functions as well.

Non-Linq
----------

DbEntry provides query methods like linq but do not need linq support. It allow user to define compare s clause with user definded column name instead of field name of model class.

````c#
User.Find(CK.K["Age"] > 15);
````

``CK.K`` is the converter to make the operator overloading works. If you don't like this name, it has an alias ``CK.Column``.

The column name should be the name in database, it is NOT the field name in the class.

If we neglected the ``CK.K`` and the quotes for the column name, it will like ``[Age] > 15``, just like SQL statement, isn't it?

The find function allows ``OrderBy`` clause as well:

````c#
User.Find(CK.K["Age"] > 18, new OrderBy("Id"));
````

The ``OderBy`` can be multiple parameters too:

````c#
User.Find(CK.K["Age"] > 18, new OrderBy((ASC)"Id" , (DESC)"Age"));
````

If support operators such as ``and``, ``or``:

````c#
User.Find(CK.K["Age"] > 15 && CK.K["Gender"] == true || CK.K["Name"] == "tom");
````

And we can specify the priority by brackets:

````c#
User.Find(CK.K["Age"] > 15 && (CK.K["Gender"] == true || CK.K["Name"] == "tom"));
````

``Like`` clause is supported too:

````c#
User.Find(CK.K["Name"].Like("%tom%") && CK.K["Age"] > 18);
````

We can compare columns too:

````c#
User.Find(CK.K["Age"].Gt(CK.K["Count"]));
````

The value ``null`` is fully supported in the condition:

````c#
User.Find(CK.K["Birthday"] != null);
````

If the condition parameter is ``Condition.Empty``, it means no ``where`` clause in the SQL, or we can say it means find all.

````c#
User.Find(Condition.Empty);
````

The above queries all returns ``DbObjectList`` _(it inherits from List)_ , if we just want one item to be returned, we can use ``FindOne``.

````c#
User.FindOne(CK.K["Name"] == "tom");
````

The ToUpper and ToLower functions are supported as well:

````c#
User.FindOne(CK.K["Name"].ToUpper() == "TOM");
User.FindOne(CK.K["Name"].ToLower() == "tom");
````

And the Like method with ToUpper and ToLower should be:

````c#
User.FindOne(CK.K["Name"].ToUpper().Like("%TOM%"));
User.FindOne(CK.K["Name"].ToLower().Like("%tom%"));
````

Or if we just want get the object by ``Id``, we can use ``FindById``:

````c#
User.FindById(1);
````

Compose Conditions
----------

Most application allow users to input one or more keywords to search by multiple conditions.

Such as title and content, if the user only input keyword in the title textbox, we just search title with the keyword. If the user input keyword both of title and content, we search title and content with the keywords both.

In DbEntry, it's very easy.

The following code shows how to use ConditionBuilder to do it by Linq style:

````c#
public List<News> Query(string Title, string Content)
{
    var builder = new ConditionBuilder<News>();
    if (!String.IsNullOrEmpty(Title))
        builder &= p => p.Title.Contains(Title);
    if (!String.IsNullOrEmpty(Content))
        builder &= p => p.Content.Contains(Content);
    var condition = builder.ToCondition();
    if (condition == null)
        throw new Exception("The condition couldn't all be empty!");
    return News.Find(condition);
}
````

And there is Non-Linq version:

````c#
public List<News> Query(string Title, string Content)
{
    Condition c = null;
    if (!String.IsNullOrEmpty(Title))
        c &= CK.K["Title"].MiddleLike(Title);
    if (!String.IsNullOrEmpty(Content))
        c &= CK.K["Content"].MiddleLike(Content);
    if (c == null)
        throw new Exception("The condition couldn't all be empty!");
    return News.Find(c);
}
````

Query by class DbEntry
----------

DbEntry class provides From function to support fluent interface. The sample query is:

````c#
DbEntry.From<User>().Where(p => p.Age > 15).OrderBy(p => p.Id).Select();
````

If we want get the object by primary key:

````c#
DbEntry.GetObject<User>(1);
````

The range clause can be specified in the ``From`` syntax:

````c#
DbEntry.From<User>().Where(Condition.Empty).OrderBy("Id").Range(1, 10).Select();
````

The parameters in the range clause are start-with and end-with. It's including the value of them. And the minimal of start-with is 1.

The ``From`` syntax also provides ``PagedSelector``:

````c#
var ps = DbEntry
    .From<User>()
    .Where(Condition.Empty)
    .OrderBy("Id")
    .PageSize(10)
    .GetPagedSelector();
````

More details about it please read [Paged query].

The ``From`` syntax also provides ``GroupBy``:

````c#
var list = DbEntry
    .From<Book>()
    .Where(Condition.Empty)
    .OrderBy((DESC)DbEntry.CountColumn)
    .GroupBy<long>("Category_Id");
````

The type of result is little longer. but we can just use ``var`` to define it.

The ``DbEntry.CountColumn`` is a special name of group count. It used to give the count an alias name.

Distinct also supported, but distinct stuff only for the model who implements IDbObject directly since the Id column will make distinct failed:

````c#
var list = DbEntry
    .From<Book>()
    .Where(Condition.Empty)
    .OrderBy((DESC)DbEntry.CountColumn)
    .SelectDistinct();
````

If all of above couldn't help you to solve your problem, your query is too complex. Please  use [SQL statement](SQLStatement.md) to solve it.

QueryRequired
----------

A lot of big projects need to support multi company but don't want they effect each other.

The common design is a Companies table and the FK Company_Id in other tables.

For query OTHER tables we always need use FK as a condition so we need write the query statement  very careful. And it may cause big leak issue if we miss the FK condition.

DbEntry support QueryRequired attribute to ensure we write the condition in all queries.

We just need to define a field as QueryRequired:

````c#
public class User : DbObjectModel<User>
{
    [QueryRequired]
    public string Name { get; set; }

    public int Age { get; set; }
}
````

It will cause exception if there is no Name condition in the query. And if the condition have Name OR Id will be OK:

````c#
User.Find(p => p.Age > 18); // this will cause exception
User.Find(p => p.Name == "tom"); // OK
User.FindById(1); // OK
````

Visual Basic
----------

Visual Basic has module and a special syntax for string, so we can make it easier than ``CK.K`` in C#.

First, define the helper module in Visual Basic:

````vb.net
Imports Leafing.Data

Public Class DbEntryVbExtention
    Default Public ReadOnly Property Col(ByVal ColName As String) As CK
        Get
            Return CK.K(ColName)
        End Get
    End Property
End Class

Module DbEntryVbExtentionModule
    Public r As DbEntryVbExtention = New DbEntryVbExtention()
End Module
````

And then, we can use the ``Find`` function by this syntax:

````vb.net
User.Find(r!Id > 5 And r!Age > 18 And r!Gender = True)
````

Have fun!

