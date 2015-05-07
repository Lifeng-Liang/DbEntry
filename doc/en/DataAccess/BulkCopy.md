Bulk Copy
==========

ADO.Net 2.0 provides a new batch copy feature for SQL Server. It named ``SqlBulkCopy``.

I don't know if the ``SqlBulkCopy`` compose SQL statement or not. But I do know is the DTS in SQL Server is highly faster than SQL statement in our code. In that case of Microsoft said ``SqlBulkCopy`` is very fast, I hope it can do what DTS do. 

Even not, the fast is truth.

I provide the other databases this feature in DbEntry. It uses compose SQL statement to insert the items. So it can't fast like DTS. But from 2 terms it will increase the speed of insert a lot of items.

1. It isn't part of ORM, so it just need do fewer steps to transfer the items.
1. It will use ``BatchSize`` as the item count in one transaction. So it need fewer begin transaction and commit transaction. In most databases, it will increase the speed. And in the file based database like SQLite, the every transaction means open file and close file each time. It is another reason of speed increasiment by fewer transactions.

Of course, we can just use one transaction to insert them all. But if we have a lot of items, maybe we don't want rollback it just because of the last mistake of the list.

It provides progressing notify just like ``SqlBulkCopy``, so we can monitor it and cancel it anytime.

In DbEntry, it provides the common interface of this feature named ``IDbBulkCopy``. And it provides a SqlBulkCopy proxy to implements this interface. We don't need to care about what database it is, if the destination is SQL Server, it will give us SqlBulkCopy.

The usage of it:

````c#
var dc = new DataProvider("SqlServer");
SqlStatement sql = new SqlStatement("select [Id],[Name] from [Books] order by [Id]");
DbEntry.Provider.ExecuteDataReader(sql, delegate(IDataReader dr)
{
    dc.NewConnection(delegate()
    {
        IDbBulkCopy c = dc.GetDbBulkCopy();
        c.BatchSize = 2;
        c.DestinationTableName = "test";
        c.NotifyAfter = 3;
        c.SqlRowsCopied += new SqlRowsCopiedEventHandler(
        	delegate(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.WriteLine("{0} rows copied.", e.RowsCopied);
        });
        c.WriteToServer(dr);
    });
});
````
