Object Definition
==========

Object definition of standard way:

````c#
public class User : DbObjectModel<User>
{
    public string Name { get; set; }
}
````

This will create an object model, and the object has a primary key named "Id", and some functions like ``Save``, ``Delete``, it also has some static functions like ``Find``, ``FindById``, ``FindBySql`` etc.

The default type of primary key "Id" is long, but we can define it as int or Guid:

````c#
public class User : DbObjectModel<User, int>
{
    public string Name { get; set; }
}

public class Room : DbObjectModel<Room, Guid>
{
    public string Name { get; set; }
}
````

Attention for this mode, the generic argument of ``DbObjectModel`` must same as defined class, otherwise it will cause something we do not expect.

If an object inherits from ``DbObjectModel``, it provides partial update for the object, in partial update mode, the ``Update`` function only update the columns which changed, it doesn't call database if nothing changed. 

````c#
var u = new User {Name = "tom"};
u.Save();
var u1 = User.FindById(u.Id);
````

For decimal type of field, we can define the precision of it by attribute, the default precision is (16,2):

````c#
public class Product : DbObjectModel<Product>
{
    [Precision(10,2)]
    public decimal Price;
}
````

Also we can create an object inherits from ``DbObject``, and doesn't use property style:

````c#
public class User : DbObject
{
    public string Name;
}
````

In this way, it doesn't has extra functions in it, ``DbObject`` only provides an primary key column named "Id", so it means in this object it has two columns: ``Name`` and ``Id``, the ``Id`` column type is long, and it's aoto increments primary key of this table.

And we need to use ``DbEntry`` to operate it:

````c#
var u = new User();
u.Name = "tom";
DbEntry.Save(u);
var u1 = DbEntry.GetObject<User>(u.Id);
````

Also we can create an object inherits from ``System.Object`` by implements the interface IDbObject:

````c#
public class User : IDbObject
{
    public string Name;
}
````

In this way, it only has one column named "Name", nothing else. This type of objects can operate like other objects inherits from ``DbObject``, only it don't has the primary key "Id", but you can define another primary key or multi-key for it, and this is the only way to define primary key by ourself in DbEntry.

For example, if we want the ``Name`` column to be the primary key of this table, just change the object model as following:

````c#
public class User : IDbObject
{
    [DbKey(IsDbGenerate=false), Length(50)]
    public string Name;
}
````

It is almost like the other base class ``NamedDbObject``, so if we want use string type ``Name`` column as primary key, we can inherits from ``NamedDbObject``, the little difference is the max length of ``Name`` column is 255 in ``NamedDbObject``:

````c#
public class User : NamedDbObject
{
}
````

But in this case, the object does not have a system generated primary key, so the ``Save`` function do not work, we should use ``Insert`` or ``Update`` function by ourself.

If we want to define the auto increments primary key as other name, we can difine it as following:

````c#
public class User : IDbObject
{
    [DbKey]
    public long userID;
 
    public string Name;
}
````

The ``UnsavedValue`` is set to tell DbEntry how to judge the object is saved or not. In this case, if the ``userID`` equals 0, it means it is a new object, it should use ``INSERT`` sql to operate, otherwise, it means it is a saved object, it should use ``UPDATE`` sql to operate. This is how the ``Save`` function works.

The multi-key example:

````c#
public class MKey : IDbObject
{
    [DbKey(IsDbGenerate = false), Length(50)]
    public string Name;
 
    [DbKey(IsDbGenerate = false)]
    public int Age;
 
    public bool Actived;
}
````

All the object we just show, all using the default information by itself like class name, field name and property name, so we don't need to defind other information for it, but if we need, we can map it as another name:

````c#
[DbTable("User")]
public class MyUser : IDbObject
{
    [DbColumn("Name")]
    public string theName;
}
````

This class also defined to operate same table in database, but in C#, it has a different name. it could used when we need to change table or column name but do not want change the C# code name. Or it can use for some column name which is not a legal C# identity:

````c#
public class User : IDbObject
{
    [DbColumn("User Name")]
    public string Name;
}
````

There are some other attributes we can use:

````c#
public class User : IDbObject
{
    [Length(50), AllowNull]
    public string Name;
 
    [StringColumn(IsUnicode=true, Regular=CommonRegular.EmailRegular)]
    public string Email;
}
````

The ``Length`` attribute defined the max length of the string field, it will use for generate create table sql statement and use for validate function. ``Length`` only works for string field, don't define it to other type field.

If a string field is not defined by ``Length`` attribute, it means it has unlimited size, the mapped database type is "text" or "ntext" (in sql server).

The ``AllowNull`` attribute defines the field which allows null value, in DbEntry, it also use for genernate create table sql and for validate function. In DbEntry, the field which is defined ``AllowNull`` attribute or ``Nullable`` type field will be deemed as allow null field. You don't need to define this attribute to ``Nuallable`` field, ``AllowNull`` attribute only works for string field.

``StringColumnAttribute`` also works for create table and validate function. ``IsUnicode`` tells DbEntry if the column type is unicode, by default, this argument is true, and the ``Regular`` tell the validate function if it need check the field by using a regular expression. ``CommonRegular`` provides two common regulars: Email and Url.

By now, all fields we defined are mapped to a table column. In DbEntry, it works for field and property which has both get and set. And it only works for public and protected member. So if we want a member does not map to a column, we can set it as private. Also, there is an attribute named ``ExcludeAttribute``, the field or property which defined by this attribute will not mapped by DbEntry too:

````c#
public class User : IDbObject
{
    public string Name;
 
    [Exclude]
    public bool isUserInput;
 
    private int times; // exclude too
 
    public int code { get { return 0; } } // exclude too
}
````

``IndexAttribute`` is set to tell DbEntry create index in create table function. It has 3 fields. ``ASC`` is bool type to tell DbEntry which sort type of this index. ``UNIQUE`` is bool type to tell DbEntry if this index is unique. ``IndexName`` is string type to tell DbEntry what name of this index, if this argument is not set, it will use ¡°IX_¡± plus table name plus column name as the index name. And if two or more columns set as the same index name, it means it is a composed index.

The following code shows a composed index ``Name_Age`` with ``DESC``, ``UNIQUE`` mode:

````c#
class MyTest : IDbObject
{
    [DbKey]
    public long Id = 0;
 
    [Index("Name_Age", ASC = false, UNIQUE = true)]
    public string Name = null;
 
    [Index("Name_Age", ASC = false, UNIQUE = true)]
    public int Age = 0;
}
````

The join table syntax is not in query syntax, it is an attribute set on object definition.

The following code shows two tables ``SampleData`` and ``TheAge`` join on ``SampleData.Id`` equals ``TheAge.Id``:

````c#
[JoinOn(0, typeof(SampleData), "Id", typeof(TheAge), "Id")]
public class JoinTable1 : IDbObject
{
    [DbColumn("SampleData.Id")] public long Id;
    public string Name;
    public UserRole Role;
    public DateTime JoinDate;
    public int Age;
}
````

Because we can join more than 2 tables, but we can not ensure the order of the attributes we get by use .net reflection, so it has an order argument to tell DbEntry the order of join syntax.

The following code shows 3 tables join by using ``JoinOnAttribute``:

````c#
[JoinOn(0, typeof(SampleData), "Id", typeof(TheAge), "Id", CompareOpration.Equal, JoinMode.Inner)]
[JoinOn(1, typeof(SampleData), "Id", typeof(LeafingEnum), "Id", CompareOpration.Equal, JoinMode.Inner)]
public class JoinTable2 : IDbObject
{
    [DbColumn("SampleData.Id")] public long Id;
    [DbColumn("SampleData.Name")] public string Name;
    public UserRole Role;
    public DateTime JoinDate;
    public int Age;
    [DbColumn("LeafingEnum.Name")] public string EnumName;
}
````

There are 4 relation types -- ``HasOne``, ``HasMany``, ``BelongsTo``, ``HasAndBelongsToMany``. And they only work for the classes which inherits from DbObjectModel.

For the model which defined these 4 types, also allowed ``DbColumn`` attribute, if there is no ``DbColumn`` attribute on it, it will use table name plus "_Id" as the column name. And for the property which defined ``HasOne HasMany HasAndBelongsToMany``, the ``OrderBy`` paramter could be used to define order-by clause of relation SQL.

````c#
[DbTable("People")]
public class Person : DbObjectModel<Person>
{
    public string Name { get; set; }

    [OrderBy("Id DESC")]
    public HasOne<PersonalComputer> PC { get; private set; }
}

public class PersonalComputer : DbObjectModel<PersonalComputer>
{
    public string Name { get; set; }

    [DbColumn("Person_Id")]
    public BelongsTo<Person> Owner { get; private set; }
}
````

More details about relation object will be discussed in [Relations](Relations.md).

The following object defined normal fileds, the type of those fields include string, enum, DateTime, bool and Nullable int. So *it will be used for many samples of this tutorials*:

````c#
public enum UserRole
{
    Manager,
    Worker,
    Client
}
 
public class SampleData : DbObjectModel<SampleData>
{
    [Length(50)] public string Name { get; set; }
    public UserRole Role { get; set; }
    public DateTime JoinDate { get; set; }
    public bool Enabled { get; set; }
    public int? NullInt { get; set; }
  
    public static SampleData New(string name, UserRole role,
	   DateTime joinDate, bool enabled, int? nullInt)
    {
        return new SampleData
        {
            Name = name,
            Role = role,
            JoinDate = joinDate,
            Enabled = enabled,
            NullInt = nullInt,
        };
    }
}
````

ComposedOf Field
----------

ComposedOf removed from DbEntry.Net in version 5.0.0 temporarily.