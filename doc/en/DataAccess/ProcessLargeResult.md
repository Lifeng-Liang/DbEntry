Process Large Result
==========

DbEntry Query Syntax allows multiple methods to get the result. 

By default, DbEntry add the items to a list. This is use a class named ``ListInserter`` to implements. It implements the interface ``IProcessor``. In DbEntry, all classes which implement this interface could be used to get the query result.

If we have a large result and it's too big to add them all into the memory. We can just use the following method to avoid add the items to a list.

````c#
public class ItemOutputer : IProcessor
{
    public bool Process(object obj)
    {
        Console.WriteLine( obj );
        return true;
    }
}

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Console.WriteLine("Process large results:\n");
        var ctx = ModelContext.GetInstance(typeof(SampleData));
        ctx.Operator.DataLoad(new ItemOutputer(), typeof(SampleData), 
            null, null, new OrderBy("Id"), null, false, false);
        Console.ReadLine();
    }
}
````

The parameters of method ``DataLoad`` are the condition, orderby and range etc.

Attention that in the whole operation the connection of database is always opening. If the operation is taking hours, we need keep it open and connectable in the whole time. 

So use this method if it is very necessary.
