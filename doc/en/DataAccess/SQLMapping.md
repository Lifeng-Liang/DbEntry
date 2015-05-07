SQL Mapping
==========

In DbEntry, read configuration is an easy job. And we can call SQL directly very easy and safe. So we can do the basic SQL Mapping just like iBATIS done.

The setting loader should be defined as:

````c#
public static class SQLs
{
    public static readonly string GetUserByNameAndAge = "";

    static SQLs()
    {
        typeof(SQLs).Initialize();
    }
}
````

And in app/web config file, we need define the SQL:

````xml
<appSettings>
  <add key= "GetUserByNameAndAge"
    value= "Select * From [User] Where [Name]=? And [Age]=?" />
</appSettings>
````

Now, we can use it in our code:

````c#
public class UserController
{
    public List<User> GetUserByNameAndAge(string Name, int Age)
    {
        return DbEntry.ExecuteList<User>(SQLs.GetUserByNameAndAge, Name, Age);
    }
}
````

When we use GetSqlStatement _(ExecuteList use it)_ to create the SqlStatement, the "?" in the SQL will be replaced like "@p0" and the following paramters will be set as the DataParamter(s).

Yes, it's a little complex than iBATIS and may not be powerful like iBATIS, but it's just designed as a plus for ORM in DbEntry.Net when we need define SQLs in the config file.

