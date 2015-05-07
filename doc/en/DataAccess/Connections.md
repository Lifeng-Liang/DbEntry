Connections
==========

In DbEntry, the connection will be created and destroyed in every operation of database. There is no connection pool in it, because ADO.NET supported connection pool by itself. Even we destroy the connection in our code, it also in the ADO.NET connection pool. So the best practice to use connection in ADO.NET is when we need it, create it, when we don¡¯t need it, close and destroy it immediately. 

In DbEntry, use connection is designed as an anonymous method just like use transaction:

````c#
DbEntry.NewConnection(delegate
{
    new User{Name = "tom"}.Save();
    new User{Name = "jerry"}.Save();
});
````

It has a try/catch scope in it, whatever there is an exception or not, it will close the connection either, so, by this way, we will never find unclosed connections in our application.

And there is another function to use existed connection is ``UsingExistedConnection``:

````c#
DbEntry.UsingConnection(delegate
{
    new User{Name = "tom"}.Save();
    new User{Name = "jerry"}.Save();
});
````

It will use the current connection if it exists. Also it is designed to use the connection already began a transaction. The ``ExecuteDataset``, ``UpdateDataset``, ``ExecuteScalar``, ``ExecuteNonQuery`` even ``ExecuteDataReader`` all use ``UsingConnection`` to process. So those functions will use the transaction we defined.

Use ``DataReader`` is an anonymous method too:

````c#
var sql = new SqlStatement("Select * From [User]");
ExecuteDataReader(sql, delegate(IDataReader dr)
{
    // do something with dr
});
````

So we will never find the unclosed data reader in our application too.  :)
