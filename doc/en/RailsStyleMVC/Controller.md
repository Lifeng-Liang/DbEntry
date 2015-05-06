Controller
==========

Every controller should inherit from ControllerBase:

````c#
public class BookController : ControllerBase
{
}
````

The communication between controller and viewer is use bag or flash.

To use bag should use indexer of controller, so we can define the action of it like:

````c#
public class BookController : ControllerBase
{
    public void Test()
    {
        this["Item"] = "Hello, world!!!";
    }
}
````

Then we should add a aspx view ``appname/Views/Book/Test.aspx`` and output the this["Item"] in it:

````
<%= this["Item"] %>
````

Now, run the web site and navigate to ``http://host/appname/book/test`` will show the string "Hello, world!!!".

We can also use the bag variables by just define a public variable in the page with the same name as it in the bag like following:

````html
<%@ Page Title="" Language="C#" MasterPageFile="~/User.master" %>

<script runat="server">
    public Book Item;
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    Name: <%=  Item.Name %><br/>
    ISBN: <%=  Item.ISBN %><br/>
    Category: <%=  Item.Category %><br/>
</asp:Content>
````

Paramters
----------

We can pass parameter in URL and we can define the type of it in action.

Change the test action to:

````c#
public void Test(int number)
{
    this["Item"] = "Hello, the number is: " + number.ToString();
}
````

Input the URL like ``http://host/appname/book/test/23``, the view page will shows:

````
Hello, the number is: 23
````

DbEntry also allow multiple parameters:

````c#
public void Test(DateTime dt, int days)
{
    this["Item"] = "Hello, the date is: " + dt.AddDays(days).ToString();
}
````

Input the URL like ``http://host/appname/book/test/2007-8-9/10``, the view page will shows:

````
Hello, the date is: 2007-8-19 0:0:0
````

The http GET also allowed so we can use it freely.

flash
----------

Mainly we use bag to pass items to view. Sometimes we use flash.

The bag only works for current action.

Flash just like session so it can pass to another action. The difference between flash and session is the value of flash only support read one time. Once it is read the object will be deleted.

Mostly we use flash to pass information to show the current action done. If we have an action will re-direct to another action and want to show something we can use it.

