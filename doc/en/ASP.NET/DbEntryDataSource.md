DbEntryDataSource
==========

By [Working with DataSource], we have a basic imaging of how to create a web application by using DbEntryDataSource. Now, let’s make a little more.

Change ``UserDataSource`` as following:

````c#
public class UserDataSource : DbEntryDataSource<User>
{
    public UserDataSource()
    {
        this.Condition = CK.K["Age"] > 19;
    }
}
````

Run the application. Confirm that the GridView only shows the data which the Age column greeter than 19.

``Condition`` is the public property of DbEntryDataSource. So we can change it in default.aspx.as too:

````c#
protected void Page_Load(object sender, EventArgs e)
{
    mydata.Condition = CK.K["Age"] > 19;
}
````

And we can change it after user input some conditions:

````c#
protected void Page_Load(object sender, EventArgs e)
{
    if (IsPostBack)
    {
        int n;
        if (int.TryParse(TextBox1.Text, out n))
        {
            mydata.Condition = CK.K["Age"] > n;
        }
    }
}
````

But only change it in one class and one function is a good idea.

We can change the behavior of the DbEntryDataSource by override some functions of it:

````c#
public class UserDataSource : DbEntryDataSource<User>
{
    public override List<User> ExecuteSelect(Condition condition, OrderBy order,
        int MaximumRows, int PageIndex, ref int TotalRowCount)
    {
        return base.ExecuteSelect(condition, order, MaximumRows, PageIndex, ref TotalRowCount);
    }

    public override int ExecuteInsert(object obj)
    {
        return base.ExecuteInsert(obj);
    }

    public override int ExecuteUpdate(object obj)
    {
        return base.ExecuteUpdate(obj);
    }

    public override int ExecuteDelete(object Key)
    {
        return base.ExecuteDelete(Key);
    }
}
````

So we have all CRUD in our hands now, it even don’t need to operate database! Change it whatever you want!

There is another function we can override:

````c#
protected override User CreateObject(IDictionary values)
{
    return base.CreateObject(values);
}
````

The values are key value dictionary of the object, the values type are not match of the object fields type. So it's hard to use. But it’s the original data come from GridView. It's a bad news. And the good news is mostly we don’t need to override this function.

