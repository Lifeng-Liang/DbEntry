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

Now, let¡¯s suppose the ``User`` table has one row:

| Id | Name | Age |
| -- | ---- | --- |
| 1  | tom  | 18  |

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

So, only ``Name`` column will be updated.

If we change all the columns, it will update them all.

````c#
User u = User.FindById(1);
u.Name = "jerry";
u.Age = 36;
u.Save();
````

The above ``Save`` function will generate the SQL such as:

````sql
Update [User] Set [Name]='jerry',[Age]=36 Where [Id]=1
````

>(In fact, the generated SQL is use parameter mode, the above SQL just for simpleness to show)

How it works
----------

There is a ``Dictionary<string, object>`` in the ``DbObjectSmartUpdate`` _(DbObjectModel inherits form it)_ named ``m_UpdateColumns``, the dictionary is used to store all updated column names.

And there is a function named ``m_InitUpdateColumns`` to initialize the dictionary. Before it executed, the feature will not be active.

And there is a function named ``m_ColumnUpdated`` to set items to the dictionary. It should be called when the field updated.

The MSBuild task of DbEntry will search all the classes which implements from IDbObject and change the constructor(s) to call  ``m_InitUpdateColumns`` by the end of them. And change the properties which is compiler generated in the classes to call ``m_ColumnUpdated`` in the ``Property set`` function if the value changed. So, when we change property of the object in constructor, it doesn¡¯t add anything to the dictionary, but when we change the property of the object out of the constructor, it will know which columns should be update.

It doesn¡¯t call database actually if nothing changed when we call the ``Save`` function.

>When we use an object which has a lot of columns, by partial update, we can just call ``Save`` function, and only the updated columns will be saved.

>When we use relation objects, they may have the large children list, by partial update, we can just call ``Save`` function, and only the updated items will be saved.
