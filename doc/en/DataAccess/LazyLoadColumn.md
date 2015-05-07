Lazy load column
==========

In DbEntry, we can define a column as lazy load just by define a attribute on it:

````c#
public class User : DbObjectModel<User>
{
    public string Name { get; set; }
    [LazyLoad] public string Profile { get; set; }
}
````

When we read the object from database, only other columns will be read:

````c#
User u = User.FindById(1);
````

It will call SQL as:

````sql
Select [Id],[Name] From [User] Where [Id]=1
````

When we first use this column, it will read from database:

````c#
Console.Write(o.Profile);
````

It will call SQL as:

````sql
Select [Profile] From [User] Where [Id]=1
````

For some case we want to load the lazy load field(s) without lazy-load :( , to do so, use SelectNoLazy as following:

````c#
var list = User.Where(p => p.Id == 1).SelectNoLazy();
````

It will call SQL as:

````sql
Select [Id],[Name],[Profile] From [User] Where [Id]=1
````

So if we have some large columns and want to delay to load them, we can use this feature now.
