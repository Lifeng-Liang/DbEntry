Transaction
==========

Use transaction is very easy too. Just call ``UsingTransaction`` by using ``Anonymous Method`` mode:

````c#
DbEntry.UsingTransaction(delegate
{
    SampleData.New("aa", UserRole.Manager, DateTime.Now, true).Save();
    SampleData.New("bb", UserRole.Client, DateTime.Now, false).Save();
    DbEntry.Provider.ExecuteNonQuery("Update [SampleData] Set [Name]='tom' Where [Id]=1");
});
````

All above code include two ``Save`` calls and one ``ExecuteNonQuery`` call are in the same transaction. We don't need to call ``Commit`` by ourselves, it will commit the changes by the end of the scope if all the code executes success. It will call the rollback function by the end of the scope if any exception occurs. The connection will close by the end of the scope too.

So, we don't need worry about commit, rollback or close connection, just put the code into this scope, they all will in the transaction.

We can specify isolation level as well:

````c#
DbEntry.UsingTransaction(IsolationLevel.RepeatableRead, delegate
{
    SampleData.New("aa", UserRole.Manager, DateTime.Now, true).Save();
    SampleData.New("bb", UserRole.Client, DateTime.Now, false).Save();
});
````

``UsingTransaction`` will use current transaction if it exists. we can use ``NewTransaction`` to call a nested transaction if we want to define a new transation to avoid use the existed one:

````c#
DbEntry.UsingTransaction(delegate
{
    SampleData.New("aa", UserRole.Manager, DateTime.Now, true).Save();
    SampleData.New("bb", UserRole.Client, DateTime.Now, false).Save();
    DbEntry.NewTransaction(delegate
    {
        new User{Name = "tom"}.Save();
        new User{Name = "jerry"}.Save();
    });
});
````

The above two transactions don't disturb each other. But it's not recommended to use.

We can specify the isolation level in ``NewTransaction`` too.

The isolation level in ``UsingTransaction`` means if the isolation level of current transaction is not equals the specified isolation level it will start a new transaction with the specified isolation level.

When we call relation objects to save or delete, it will use ``UsingTransaction`` as well, so all the operations are in one transaction. And if we define a new transaction around it, the operate in the scope will in the same transaction.

The ``UsingTransaction`` funciton opens the database connection before. But for now it will wait till someone execute any SQL. So it will use the first call's connection information instead of itself's. So it only in DbEntry class now.  Therefore the nested transaction to simulate cross-database transaction could be used as following:

````c#
DbEntry.UsingTransaction(delegate
{
    SampleData.New("aa", UserRole.Manager, DateTime.Now, true).Save();
    SampleData.New("bb", UserRole.Client, DateTime.Now, false).Save();
    DbEntry.UsingTransaction(delegate
    {
        new MySqlUser{Name = "tom"}.Save();
        new MySqlUser{Name = "jerry"}.Save();
    });
});
````

Of course, it isn't the real cross-database transaction, We can use ``TransactionScope`` to get the real cross-database transaction by using DbEntry as well.

Cross-database transaction
----------

``TransactionScope`` gives us real cross-database transaction. Inside it, the MSDTC is use for manager transactions between databases. MSDTC isn't 100% ensure way to implements cross-database transaction, but it gives us a better solution than nested transaction like above code shows.

The following code shows how to use it in DbEntry:

````c#
public class test : DbObjectModel<test>
{
    public string Name { get; set; }
}

[DbContext("sql2")]
public class trTest : DbObjectModel<trTest>
{
    public string Name { get; set; }
}
 
class Program
{
    static void Main(string[] args)
    {
        using (var ts = new TransactionScope())
        {
            DbEntry.UsingTransaction(delegate
            {
                new test{Name = "t1"}.Save();
                new test{Name = "t2"}.Save();
            });
            new trTest{Name = "t3").Save();
            int n = 0;
            n = 5 / n; // emulate exception.
            new trTest{Name = "t4").Save();
            ts.Complete();
        }
    }
}
````

In above code, the exception will caused at ``new trTest().Save`` line if we don't have MSDTC installed.

Check the database, the ``t1, t2`` are not saved to ``test`` table yet. So we can say it works in DbEntry.

Because it based on MSDTC, so the database instances must allow MSDTC to manage. It means not all supported databases by DbEntry could use this feture. More information about it, please visit [Microsoft](http://www.microsoft.com/) to find out.

Based on [an article on internet (Chinese version)](http://www.cnblogs.com/cn_wpf/archive/2007/08/06/844766.html), use ``TransactionScope`` needs to install the following hot-fixes first:

| Number   | Link                                                                                         |
| -------- | -------------------------------------------------------------------------------------------- |
| kb916002 | [http://support.microsoft.com/kb/916002/en-us](http://support.microsoft.com/kb/916002/en-us) |
| kb929246 | [http://support.microsoft.com/kb/929246/en-us](http://support.microsoft.com/kb/929246/en-us) |
| kb936983 | [http://support.microsoft.com/kb/936983/en-us](http://support.microsoft.com/kb/936983/en-us) |
