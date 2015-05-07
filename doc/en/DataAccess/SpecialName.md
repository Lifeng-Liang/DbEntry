Special Name
==========

In DbEntry, we can define special name CreatedOn and UpdatedOn. So when we insert it, the CreatedOn column will set as current time. When we update it, the UpdatedOn column will set as current time.

````c#
public class DateTable : DbObjectModel<DateTable>
{
    public string Name { get; set; }

    [SpecialName]
    public DateTime CreatedOn { get; set; }

    [SpecialName]
    public DateTime? UpdatedOn { get; set; }
}
````

The name of them must be CreatedOn or UpdatedOn and the type must be DateTime. And UpdatedOn must be nullable.

The name(s) in our code can't change. But the mapped name could define as another name:

````c#
public class DeletedUser : DbObjectModel<DeletedUser>
{
    [SpecialName, DbColumn("DeletedOn")]
    public DateTime CreatedOn { get; set; }
}
````

There is another special name LockVersion. We can use it for optimistic locking:

````c#
public class TestTable : DbObjectModel<TestTable>
{
    [SpecialName]
    public int LockVersion { get; set; }
}
````

When we updata the record, the LockVersion will be checked and increased. If update statement can't find any matched record, an exception will be raised.

The name of LockVersion in our code can't change. But the mapped name could define as another name:

````c#
public class TestTable : DbObjectModel<TestTable>
{
    [SpecialName, DbColumn("TestVersion")]
    public int LockVersion { get; set; }
}
````

SavedOn
----------

SavedOn works like compose of CreatedOn and Updated. When we insert or update the object, it all set as the current time.

````c#
public class DateTable : DbObjectModel<DateTable>
{
    [SpecialName]
    public DateTime SavedOn { get; set; }
}
````

If we need map it as another name in database, the DbColumn attribute could help us.

Count
----------

Count works as SQL "Count = Count + 1". When we need a counter of saved times, we can use it:

````c#
public class Test : DbObjectModel<Test>
{
    [SpecialName]
    public int Count { get; set; }
}
````

If we need map it as another name in database, the DbColumn attribute could help us.
