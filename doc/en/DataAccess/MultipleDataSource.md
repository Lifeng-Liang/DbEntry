Multiple Data Source
==========

In DbEntry, ``DataProvider`` is the main interface to execute SQL. And ORM layer will call it to access database. From v4.0, The model stuffs moved to a class called ModelContext, all the things about the model such as ObjectInfo, ModelOprator etc, are in this class now. 

If there is ``DefaultContext`` node in ``App.config/Web.config``, it will use this argument to get the default context, otherwise it will use the empty string as the argument to get it.

````xml
<Leafing.Settings>
    <add key="DataBase" value="@Access : @~test.mdb" />
</Leafing.Settings>
````

And the following is same as above:

````xml
<Leafing.Settings>
    <add key="DefaultContext" value="Access" />
    <add key="Access.DataBase" value="@Access : @~test.mdb" />
</Leafing.Settings>
````

More details about config file please read [Configuration](Configuration.md).

And we can use other context(s) to operate other database(s).

````xml
<Leafing.Settings>
    <add key="DataBase" value="@Access : @~test.mdb" />
    <add key="Excel.DataBase" value="@Excel : @~test.xls" />
</Leafing.Settings>
````

And all of the key names of one database setting should have the same prefix:

````xml
<Leafing.Settings>
    <add key="DefaultContext" value="MySql" />

    <add key="MySql.DataBase" value="@MySql : server=localhost;user id=root; password=123; database=mytest; pooling=false" />
    <add key="MySql.DbProviderFactory" value="@SmartDbFactory : MySql.Data, Version=5.1.2.2, Culture=neutral, PublicKeyToken=c5687fc88969c44d" />

    <add key="sqlite.DataBase" value="@SQLite : @~Test.db" />
    <add key="sqlite.DbProviderFactory" value="System.Data.SQLite.SQLiteFactory, System.Data.SQLite, Version=1.0.36.1, Culture=neutral, PublicKeyToken=db937bc2d44ff139" />
</Leafing.Settings>
````

The model should be defined to use the specified context by attribute:

````c#
[DbContext("Excel")]
public User : DbObjectModel<User>
{
    public string Name { get; set; }
}
var l1 = User.FindAll();
var l2 = DbEntry.From<User>().Where(null).Select();
````

And we can use specified ModelContext to get more feature(s) like:

````c#
var ctx = ModelContext.GetInstance(typeof(User));
ctx.Create();
````
