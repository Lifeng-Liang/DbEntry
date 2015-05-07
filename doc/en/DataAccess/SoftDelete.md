Soft Delete
==========

In DbEntry, we can define the class as soft delete. It means there is a column named "IsDeleted" would be used to judge is the row deleted or not.

````c#
[SoftDelete]
public class User : DbObjectModel<User>
{
    public string Name { get; set; }
}
````

This class maps to a table which has a column "IsDeleted" with bool type.

We can define another name to this column:

````c#
[SoftDelete(ColumnName = "IsDel")]
public class User : DbObjectModel<User>
{
    public string Name { get; set; }
}
````

Now, we can use it in our code:

````c#
User o = User.FindById(1);
o.Name = "test";
o.Save();

o = User.New;
o.Name = "tt";
o.Save();

o.Delete();
````

>*Attention:* the cross-table of many to many relations doesn¡¯t support soft delete. This table is automatically use and always use the real delete.
