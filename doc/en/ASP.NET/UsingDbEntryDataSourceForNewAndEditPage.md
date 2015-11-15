Using DbEntryDataSource for New and Edit Page
==========

Sample
----------

Create the data model in an library assembly named "Libs" like:

````c#
public enum Category
{
    Story,
    Nature,
    Class,
}

public abstract class Book : DbObjectModel<Book>
{
    [Length(10)] public abstract string Name { get; set; }
    public abstract float Price { get; set; }
    public abstract bool Read { get; set; }
    public abstract Category Category { get; set; }
}

public class BookDataSource : DbEntryDataSource<Book>
{
}
````

Create a web site and add a GridView to default.aspx

Create a web page named BookEdit.aspx

Change the Web.config to:

````xml
<?xml version="1.0"?>
<configuration>
  <configSections>
    <section name="Leafing.Settings" type="Leafing.Core.Setting.NameValueSectionHandler, Leafing.Core"/>
  </configSections>
  <Leafing.Settings>
    <add key="AutoScheme" value="CreateTable" />
    
    <add key="DataBase" value="@SQLite : @c:\ttt.db"/>
    <add key="DbProviderFactory" value="System.Data.SQLite.SQLiteFactory, System.Data.SQLite, Version=1.0.47.2, Culture=neutral, PublicKeyToken=db937bc2d44ff139"/>
  </Leafing.Settings>
  <appSettings/>
  <connectionStrings/>
  <system.web>
    <pages>
      <controls>
        <add assembly="Libs" namespace="Libs" tagPrefix="biz"/>
      </controls>
    </pages>
    <compilation debug="true"/>
    <authentication mode="Windows"/>
  </system.web>
</configuration>
````

Add the BookDataSource to default.aspx:

````html
<biz:BookDataSource ID="BookDataSource1" runat="server" />
````

Set the GridView to bind the DataSource:

````
DataSourceID="BookDataSource1" DataKeyNames="Id"
````

Add a column to the GridView:

````html
<Columns>
    <asp:HyperLinkField Text="Edit" DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/BookEdit.aspx?Id={0}" />
</Columns>
````

Add a hyperlink to default.aspx:

````html
<a href="BookEdit.aspx">New</a>
````

Add label "title" and "msg" to BookEdit.aspx, and add style sheet to it:

````css
<style type="text/css">
.Warning
{
  color : red;
}
.Notice
{
  color : yellow;
}
</style>
````

Add button "Save" and "Delete" to BookEdit.aspx

````html
<asp:Button ID="Save" runat="server" Text="Save" />
<asp:Button ID="Delete" runat="server" Text="Delete" />
````

Add a BookDataSource to BookEdit.aspx:

````html
<biz:BookDataSource ID="BookDataSource1" runat="server" />
````

Change the BookDataSource in BookEdit.aspx with the following:

````
ContentTitleID="title"
NoticeMessageID="msg"
SaveButtonID="Save"
DeleteButtonID="Delete"
OnObjectDeleted="BookDataSource1_OnObjectDeleted"
````

In BookDataSource1_OnObjectDeleted, add :

````c#
Response.Redirect("Default.aspx");
````

Use TemplateBuilder in [Tools] to generate the html snippet and copy them to BookEdit.aspx:

````html
<table border="1">
<tr><td class="FieldTitle">Book Name:</td><td class="FieldControl"><asp:TextBox ID="Book_Name" runat="server" MaxLength="50" Columns="50" /></td></tr>
<tr><td class="FieldTitle">Book Price:</td><td class="FieldControl"><asp:TextBox ID="Book_Price" runat="server" MaxLength="20" Columns="20" /></td></tr>
<tr><td class="FieldTitle">Book Read:</td><td class="FieldControl"><asp:CheckBox ID="Book_Read" runat="server" /></td></tr>
<tr><td class="FieldTitle">Book Category:</td><td class="FieldControl"><asp:DropDownList ID="Book_Category" runat="server"><asp:ListItem Text="Story" Value="Story" /><asp:ListItem Text="Nature" Value="Nature" /><asp:ListItem Text="Class" Value="Class" /></asp:DropDownList></td></tr>
</table>
````

Add a hyperlink "Back" to BookEdit.aspx:

````html
<a href="Default.aspx">Back</a>
````

Run this web site, in default page

Click new to navigate to BookEdit.aspx, ensure the title is "New Book" and input something and click Save.

Click "Back" to navigate to Dafault.aspx to view the Book List

Click "Edit" in GridView to navigate to BookEdit.aspx, ensure the title is "Book Edit".

Change something and click Save button.

Click "Back" to navigate to default.aspx to view the changes.

How it works
----------

DbEntryDataSource works with the controls which "ID" is the fixed format "ClassName_FieldName" in "new" and "edit" mode, the controls could created by TemplateBuilder.

And DbEntryDataSource can bind the control label to show title or message, and it can bind the control button to handle save or delete.

DbEntryDataSource also provide some events with this mode such as "OnObjectDeleted" etc.

DbEntryDataSource use the http parameter named "id" to judge if the current visit is "new" or "edit".

