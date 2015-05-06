!!! Viewer

In DbEntry, the view engine is aspx. The first thing we need know is the view aspx must inherit from ``Leafing.Web.Mvc.PageBase``. If not, we will get a exception message about it when we visit the page.

We can just define it in Web.config like:

````xml
<system.web>
  <pages pageBaseType="Leafing.Web.Mvc.PageBase, Leafing.Web" />
</system.web>
````

It's easy and works for all pages, so I recommended it. But it only works for the page who used inline code mode. If you use code behide or code file mode, you need change the base class to ``Leafing.Web.Mvc.PageBase``.

Another thing about view aspx is we shouldn't use postback control or view-state in it _(we can use the control but don't use the postback event, but it's not recommanded too.)_. In MVC, the message flow just from controller to view, no postback.

Except that, we can use it freely, just like code-behind or not, use C# or VB, use stand-along page or use master-page like:

````html
<%@ Page Language="VB" MasterPageFile="~/MasterPage.master" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <p><%= this("Item") %></p>
</asp:Content>
````

We can also use the bag variables by just define a public variable in the page with the same name as it in the bag like following:

````html
<%@ Page Title="" Language="C#" MasterPageFile="~/User.master" %>

<script runat="server">
    public Book Item;
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    Name: <%= Item.Name %><br/>
    ISBN: <%= Item.ISBN %><br/>
    Category: <%= Item.Category %><br/>
</asp:Content>
````

In view page, we should to get the variables just like above. And if there is a message from another ``Action`` we can use ``flash`` to get it:

````html
<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="Show User" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <p style="color: Green"><%= Flash["Notice"] %></p>
</asp:Content>
````

Flash has 3 pre defined variable could be used to make things easily: Tip, Notice and Warning. eg:

````c#
public void Test()
{
    Flash.Notice = "all things done.";
}
````

````html
  <p style="color: Green"><%= Flash.Notice %></p>
````

In view page, we should use ``LinkTo`` method to compose hyper link for us like:

````c#
<%= LinkTo.Title("Back").Action("list"), null) %>
````

Or ``UrlTo`` like:

````c#
<%= UrlTo.Title("Back").Action("list").Parameters(nextPage) %>
````

The point to use it is not only for easy, but also for deployment. When we deploy the web site we have two choices: have ".aspx" postfix or not. The ``LinkTo`` method will choose it automatically by configuration.
