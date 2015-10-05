Partial Update
==========

When we define a class that inherits from ``DbObjectModel``, we also get the partial update feature. For example:

````c#
public class User : DbObjectModel<User>
{
    public string Name { get; set; }
    public int Age { get; set; }
}
````

In above code, we defined a class with two columns: ``Name`` and ``Age``.

Now, let's suppose the ``User`` table has one row:

| Id  | Name | Age |
| --- | ---- | --- |
| 1   | tom  | 18  |

And the following code shows partial update:

````c#
User u = User.FindById(1);
u.Name = "jerry";
u.Save();
````

We just called the save function in standard way. But the ``Save`` function will generate the SQL such as:

````sql
Update [User] Set [Name]='jerry' Where [Id]=1
````

So, only ``Name`` column appears in the SQL and will be updated.

If we change all the columns, it will update them all.

````c#
User u = User.FindById(1);
u.Name = "jerry";
u.Age = 36;
u.Save();
````

The above ``Save`` function will generate the SQL like:

````sql
Update [User] Set [Name]='jerry',[Age]=36 Where [Id]=1
````

>(In fact, the generated SQLs use parameter mode, the above SQL just for simpleness to show)

How it works
----------

There is a ``Dictionary<string, object>`` in the ``PartialUpdateHelper``. And a ``PartialUpdateHelper`` in ``DbObjectSmartUpdate`` _(DbObjectModel inherits form it)_ if ``PartialUpdate`` set to true _(by default)_ in config file.

The dictionary named ``_LoadedColumns``, the dictionary is used to store all loaded columns. And it will be initialized by the loaded values just after the columns of DbObjectSmartUpdate loaded. 

It will check if the values changed before save. It will not call database actually if nothing changed when we call the ``Save`` function.

>When we use an object which has a lot of columns, by partial update, we can just call ``Save`` function, and only the updated columns will be saved.

>When we use relation objects, they may have the large children list, by partial update, we can just call ``Save`` function, and only the updated items will be saved.

Disable this feature
----------

For some reason, we may want to disable this feature. To do it, add or modify the config item PartialUpdate to false in Leafing.Settings section.

````xml
<add key="PartialUpdate" value="false" />
````
