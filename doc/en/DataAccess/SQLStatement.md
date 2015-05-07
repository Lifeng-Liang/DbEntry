SQL statement
==========

Most time we just need to use [Query Syntax](QuerySyntax.md) in DbEntry. It supports anti SQL injection. And it's unrelated to the database, so we can use same syntax on different databases, and change database freely.

But sometimes the problem we have is more complex than that, in this case, we can use SQL directly.

Invoke SQL is the feature of ``DataProvider``:

````c#
DbEntry.Provider.ExecuteNonQuery("Update [User] Set [Age] = 18");
````

The ``DbObjectModel`` also supports SQL for query:

````c#
List<SampleData> ls = SampleData.FindBySql(
    "Select * From [SampleData] Where [Enabled]=true");
````

If we want replace ``*`` as column names, make sure they are same as the mapped names in class.

The same query by using ``ExecuteList`` is:

````c#
List<SampleData> ls = DbEntry.ExecuteList<SampleData>(
    "Select * From [SampleData] Where [Enabled]=true");
````

If we just want to get one value from database, it will like:

````c#
object n = DbEntry.Provider. ExecuteScalar(
    "Select count(*) From [User] Where [Age]>18");
````

Or if we want to call stored procedure, it will like:

````c#
DataSet ds = DbEntry.Provider.ExecuteDataset("GetDataByPage", 2, 10);
````

It just like we call a SQL, we don¡¯t need to specify it as a stored procedure because of DbEntry will check the text, if it is a identity, it will be considered as a stored procedure. And for stored procedure, DbEntry will get the parameters and types from database automatically, so we don¡¯t need to specify them anymore.

SqlStatement
----------

All above are using string as SQL statement to invoke, but in fact, it converted to ``SqlStatement`` before it to be invoked.

We can use ``SqlStatement`` to do the same query like this:

````c#
SqlStatement Sql = new SqlStatement(
    "Select * From [SampleData] Where [Enabled]=true");
List<SampleData> ls = SampleData.FindBySql(Sql);
````

Like we just disscussed, DbEntry will check the text to judge if it is a stored procedure by check if it is an identity.

An identity means start with English character or underline, follow English character or number of 0 to 9 or underline. And it allows space or tab in the begin or end. Otherwise it is not an identity.

If you find the auto check doesn¡¯t work well, you can specify it in code like this:

````c#
SqlStatement Sql = new SqlStatement(
    "Select * From [SampleData] Where [Enabled]=true");
Sql.SqlCommandType = CommandType.Text;
List<SampleData> ls = SampleData.FindBySql(Sql);
````

And the timeout could be defined in the ``SqlStatement`` too:

````c#
SqlStatement Sql = new SqlStatement(
    "Select * From [SampleData] Where [Enabled]=true");
Sql.SqlTimeOut = 3600;
List<SampleData> ls = SampleData.FindBySql(Sql);
````

The stored procedure¡¯s parameters could be get from database so we don¡¯t need to specify them, but the SQL parameter need us to specify the name at least:

````c#
SqlStatement Sql = new SqlStatement(
    "SELECT TOP 50 * FROM [User] WHERE [id] > @id AND [Enabled] = @Enabled",
    new DataParamter("@id", 5),
    new DataParamter("@Enabled", false)
    );
````

The type of parameter doesn¡¯t need to specify, but if we want, we can specify it as well. The default direction of parameter is IN. We can specify it as OUT or RETURN too:

````c#
SqlStatement Sql = new SqlStatement("llf_GetOutParam",
    new DataParamter("@InParam", 12),
    new DataParamter("@OutParam", 0, typeof(int), ParameterDirection.Output),
    new DataParamter("@Ret", 0, typeof(int), ParameterDirection.ReturnValue)
);
````

By this way, we can get the out or return value after we called the stored procedure.

DataSet
----------

SqlStatement can work together with ``DataSet`` too:

````c#
SqlStatement Sql = new SqlStatement(
    "SELECT TOP 50 * FROM [User] WHERE [id] > @id AND [Enabled] = @Enabled",
    new DataParamter("@id", 5),
    new DataParamter("@Enabled", false)
    );
DataSet ds = DbEntry.Provider.ExecuteDataset(Sql);
````

And, we can update ``DataSet`` easily:

````c#
DbEntry.Provider.UpdateDataset(Sql, ds);
````

The ``typed DataSet`` is also supported:

````c#
MyDataSet ds = new MyDataSet();
DbEntry.Provider.ExecuteDataset(sql, ds);
````

Dynamic query
----------

For .Net 4.0, DbEntry supports dynamic object as well. Use ExecuteDynamicXXXX functions to do dynamic query without define db objects :

````c#
dynamic row = DbEntry.Provider.ExecuteDynamicRow("Select * From People Where Id = 1");
Assert.AreEqual("Tom", row.Name);

dynamic list = DbEntry.Provider.ExecuteDynamicList("Select * From People Order By Id");
Assert.AreEqual(3, list.Count);
Assert.AreEqual(1, list[0].Id);
Assert.AreEqual("Tom", list[0].Name);
Assert.AreEqual(2, list[1].Id);
Assert.AreEqual("Jerry", list[1].Name);
Assert.AreEqual(3, list[2].Id);
Assert.AreEqual("Mike", list[2].Name);

dynamic table = DbEntry.Provider.ExecuteDynamicTable("Select * From People Order By Id");
Assert.AreEqual(3, table.Count);
Assert.AreEqual(1, table[0].Id);
Assert.AreEqual("Tom", table[0].Name);
Assert.AreEqual(2, table[1].Id);
Assert.AreEqual("Jerry", table[1].Name);
Assert.AreEqual(3, table[2].Id);
Assert.AreEqual("Mike", table[2].Name);

dynamic set = DbEntry.Provider.ExecuteDynamicSet(@"Select * From People Order By Id;
Select * From PCs Order By Id;");
Assert.AreEqual(2, set.Count);
Assert.AreEqual(3, set[0].Count);
Assert.AreEqual(3, set[1].Count);
Assert.AreEqual("Tom", set[0][0].Name);
Assert.AreEqual("IBM", set[1][0].Name);
````

>So, almost everything we can get in ADO.NET, we can get it in DbEntry too. And it's more easy and safe than use ADO.NET directly.