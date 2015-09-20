Configuration
==========

Common:
----------

DbEntry mainly use ``App.config/Web.config`` to set the configure information of databases. Firstly, it has a section part to define the new section ``Leafing.Settings``:

````xml
<configSections>
  <section name="Leafing.Settings" type="Leafing.Core.Setting.NameValueSectionHandler, Leafing.Core" />
</configSections>
````

It is fixed, so we don't need to change it at all, just copy it to the ``App.config/Web.config``.

And then, we need define the section ``Leafing.Settings``, it is the same format as ``appSettings``:

````xml
<Leafing.Settings>
  <add key="DataBase" value="@Access : @~test.mdb" />
</Leafing.Settings>
````

The above defined the default context to using ``Access`` and the short connection string is ``~test.mdb``.

The configure information also can store as a embedded resource xml in the assembly, it is the same format as it in ``App.config/Web.config``, if the setting information is not find in ``App.config/Web.config``, it will search for assemblies to get it, if all failed, it will raise an exception.

The file name of this embedded resource file must have the postfix ".config.xml". For example:

````
MyConfig.config.xml
````

This is an embedded resource xml configuration example:

````xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <Leafing.Settings>
    <add key="DataBase" value="@Access : @~test.mdb" />
  </Leafing.Settings>
</configuration>
````

The embedded resource xml configuration could be used sometimes we can not set the configuration in ``App.config/Web.config``, for example, I use it in the unit test project, because it is a library and don't know who will load and run it.

There are some items which do not usually used in configuretion: ``SqlTimeOut``, ``TimeConsumingSqlTimeOut``, ``MaxRecords``, ``Orm.UsingParamter``.

The ``SqlTimeOut`` and ``TimeConsumingSqlTimeOut`` are set for normal SQL calling from ORM layer, the timeout of ``SELECT`` SQL will set as ``TimeConsumingSqlTimeOut``, and others will set as ``SqlTimeOut``. The default value of ``SqlTimeOut`` is 30. The default value of ``TimeConsumingSqlTimeOut`` is 60.

````xml
<Leafing.Settings>
  <add key="TimeConsumingSqlTimeOut" value="3600" />
  <add key="DataBase" value="@Access : @~test.mdb" />
</Leafing.Settings>
````

There is a hidden mode of settings to set the mapped table name as another name. If we add the setting item and the ``key`` is start with ``@``, it will be a re-map setting. The value will be the final table name. Sometimes I use it to change the table name of the inner classes of DbEntry like ``Leafing_Enum`` and ``LogItem``.

````xml
<Leafing.Settings>
  <add key="@Leafing_Enum" value="MyEnums" />
</Leafing.Settings>
````

If we use SQL directly, the timeout will be 30 and we can change it manually.

````c#
var sql = new SqlStatement("select count(*) from [User]");
sql.SqlTimeOut = 180;
DbEntry.Provider.ExecuteScalar(sql);
````

The ``MaxRecords`` is set for ``SELECT`` SQL the max records count we will load, if this value is 0, it means unlimited, the default value is 0. This setting item could be used in some special case like ``SQL QUERYER``, the SQL is input by end user, and we don't want the result will be unlimited to delay our database response performance. Otherwise, let it be 0.

The ``Orm.UsingParamter`` is set for how to give the information by clauses. By default, this value is true. It means it will use SQL Parameter to send the information to database. If the value is false, it will replace the SQL Statement parts as the information, but it is not fully tested, and doesn't support all types. I use this setting only if I want see the whole SQL and don't mind the results.

Working with databases:
----------

The following is how to set database context in config file.

``DataBase`` joins with two part, first is ``Dialect``, last is ``ConnectionString``, split with ":". If the first of char of ``Dialect`` is @, it's a short style, and means it is a inner ``Dialect``, ``ConnectionString`` for ``Access``, ``Excel``, ``SQLite``, if the first char is @, it's a short style, just put the file name after @.

If there is ``{BaseDirectory``} or ``~`` in short style ``ConnectionString``, it will be replaced to current directory of this application (or web), so please ensure there is no ``{BaseDirectory``} or ``~`` in ``ConnectionString`` if you don't mean it.

For ``SQLite``, the short style set cache to 100K and does not use system ``Flush`` function. _(The origin provider set cache to 2K and use system Flush function.)_

The ``UnitTest`` is using ``SQLite``, to run it, you should install the ``SQLite`` provider first.

``DbProviderFactory`` should set ``DbProviderFactory`` fullname, for ``@SmartDbFactory``, it joins with two parts, last is Assembly fullname, ``SmartDbFactory`` will search driver automatically, it could be used for suport the driver of ADO.Net 1.x , and, it also provides the way to search Stored Procedure's paramters.

Assembly's fullname could be get by ``AssemblyNameGetter.exe`` in the tools.

The sample of config:

Access 2003:

````xml
<Leafing.Settings>
  <add key="DataBase" value="@Access : @~test.mdb" />
</Leafing.Settings>
````

Access 2007:

````xml
<Leafing.Settings>
  <add key="DataBase" value="@Access2007 : @~test.accdb" />
</Leafing.Settings>
````

Excel:

````xml
<Leafing.Settings>
  <add key="DataBase" value="@Excel : @~test.xls" />
</Leafing.Settings>
````

Excel 2007:

````xml
<Leafing.Settings>
  <add key="DataBase" value="@Excel2007 : @~test.xlsx" />
</Leafing.Settings>
````

Sql Server 2000:

````xml
<Leafing.Settings>
  <add key="DataBase" value="@SqlServer2000 : data source=wms;initial catalog=wms;user id=sa;password=1" />
</Leafing.Settings>
````

Sql Server 2005:

````xml
<Leafing.Settings>
  <add key="DataBase" value="@SqlServer2005 : data source=wms;initial catalog=wms;Integrated Security=SSPI;" />
</Leafing.Settings>
````

MySql: [Download Link](http://dev.mysql.com/downloads/connector/net/5.1.html)

````xml
<Leafing.Settings>
  <add key="DataBase" value="@MySql : server=localhost;user id=root; password=1; database=wms; pooling=false" />
  <add key="DbProviderFactory" value="@SmartDbFactory : MySql.Data, Version=5.1.2.2, Culture=neutral, PublicKeyToken=c5687fc88969c44d" />
</Leafing.Settings>
````

SQLite: [Download Link](http://system.data.sqlite.org/)

````xml
<Leafing.Settings>
  <add key="DataBase" value="@SQLite : @~Test.db" />
  <add key="DbProviderFactory" value="System.Data.SQLite.SQLiteFactory, System.Data.SQLite, Version=1.0.94.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139" />
</Leafing.Settings>
````

Firebird: [Download Link](http://www.firebirdsql.org/index.php?op=files&id=netprovider)

````xml
<Leafing.Settings>
  <add key="DataBase" value="@Firebird : User=SYSDBA;Password=masterkey;Database=c:\mytest.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=UNICODE_FSS;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0" />
  <add key="DbProviderFactory" value="@SmartDbFactory : FirebirdSql.Data.FirebirdClient, Version=2.5.1.0, Culture=neutral, PublicKeyToken=3750abcc3150b00c" />
</Leafing.Settings>
````

Oracle:

````xml
<Leafing.Settings>
  <add key="DataBase" value="@Oracle : Data Source=localhost; User Id=llf; Password=123" />
  <add key="DbProviderFactory" value="@SmartDbFactory : System.Data.OracleClient, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
</Leafing.Settings>
````

PostgreSQL:[Download Link](http://pgfoundry.org/projects/npgsql)

````xml
<Leafing.Settings>
  <add key="DataBase" value="@PostgreSQL : Server=127.0.0.1;Port=5432;Database=postgres;Userid=sa;password=1234;Encoding=UNICODE;" />
  <add key="DbProviderFactory" value="@SmartDbFactory : Npgsql, Version=1.98.4.0, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7" />
</Leafing.Settings>
````

>*Attention:* The version of database provider is based on my installed version, if you installed a newer version of the provider, please change the version part. If it did not installed to the GAC, please do it by yourself.

AutoScheme:
----------

Another setting ``AutoScheme`` is set to tell DbEntry to fix the scheme before run sql on it. The default value is None. There are four value could be set to it :

1. None
2. CreateTable
3. AddColumns
4. RemoveColumns

AddColumns also includes CreateTable feature, and RemoveColumns includes AddColumns and CreateTable.

````xml
<Leafing.Settings>
  <add key="AutoScheme" value="CreateTable" />
  <add key="DataBase" value="@Access : @~Test.mdb" />
  <add key="Access2007.AutoCreateTable" value="true" />
  <add key="Access2007.DataBase" value="@Access : @~Test.mdb" />
</Leafing.Settings>
````

If it set to CreateTable, when some code want access database by using object model, DbEntry will try to create the table first if it doesn't exist.

If it set to AddColumns, when some code want access database by using object model, DbEntry will try to create the table if the table doesn't exist or add columns if the columns doesn't exist.

If it set to RemoveColumns, it will remove columns too if there is(are) column(s) exist in database but doesn't exist in our code.

This feature only works for ORM functions, execute SQL directly doesn't raise it.

This feature will help us to implements application prototype faster, it should only used in development stage or test stage.

When the application deployed to the working environment, change the value as None or delete this line in config file.

>*Attention:* MySql and Firebird don't have Unicode type of string, so if you specify the string column as Unicode type in object model, the created column size maybe not be what we thought.

Default Context
----------

DbEntry.Net allows user pre-setup some database connections and set one of them as the default context.

The following config file shows 3 database settings, and the current selected is ``development``:

````xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="Leafing.Settings" type="Leafing.Core.Setting.NameValueSectionHandler, Leafing.Core" />
  </configSections>

  <Leafing.Settings>
    <add key="DefaultContext" value="development" />

    <add key="test.DataBase" value="@Access : @~WMStest.mdb" />
    <add key="development.DataBase" value="@Access : @~WMSdevelp.mdb" />
    <add key="SqlServer.DataBase" value="@SqlServer2005 : data source=wms;initial catalog=WMS;user id=sa;password=1" />
  </Leafing.Settings>
</configuration>
````

The following config file stored some databases connections infomation in it, and the current context is ``Access``:

````xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="Leafing.Settings" type="Leafing.Core.Setting.NameValueSectionHandler, Leafing.Core" />
  </configSections>

  <Leafing.Settings>
    <add key="DefaultContext" value="Access" />
    <add key="Access.DataBase" value="@Access : @~test.mdb" />
    <add key="Excel.DataBase" value="@Excel : @~test.xls" />
    <add key="SqlServer.DataBase" value="@SqlServer2005 : data source=wms;initial catalog=WMS;user id=sa;password=1" />
    <add key="MySql.DataBase" value="@MySql : server=localhost;user id=root; password=123; database=mytest; pooling=false" />
    <add key="MySql.DbProviderFactory" value="@SmartDbFactory : MySql.Data, Version=5.1.2.2, Culture=neutral, PublicKeyToken=c5687fc88969c44d" />
  </Leafing.Settings>
</configuration>
````

Encrypt ConnectionString
--------

There's a way to encrypt the connection string of the databases.

Add a class and make it inherits from Leafing.Data.Dialect.ConnectionStringCoder and override the function ``Decode`` like :

````c#
[Implementation(2)]
public class MyCsCoder : ConnectionStringCoder
{
	public override string Decode(string source) {
		return MyDecodeMethod(source);
	}
  ......
}
````

We can define our own decode method in it.

The parameter of the attribute Implementation should be a integer and should equels or more than 2. It will make sure it will be the loaded ConnectionStringCoder instance rather than the original one.