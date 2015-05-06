Mvc Getting Started
==========

DataProvider.dll
----------

Book.cs

````c#
using System;
using Leafing.Data.Definition;

namespace DataProvider.Models
{
    public enum BookCatagory
    {
        Manager,
        Computer,
        Story,
    }

    public abstract class Book : DbObjectModel<Book>
    {
        [Length(30)] public abstract string Name { get; set; }
        public abstract DateTime BuyDate { get; set; }
        public abstract float Price { get; set; }
        public abstract BookCatagory Catagory { get; set; }
        public abstract bool Read { get; set; }
        [SpecialName] public abstract DateTime CreatedOn { get; set; }
        [SpecialName] public abstract DateTime? UpdatedOn { get; set; }
    }
}
````

BookController.cs

````c#
using System;
using Leafing.Web.Mvc;
using DataProvider.Models;

namespace DataProvider.Controllers
{
    public class BookController : ControllerBase<Book>
    {
    }
}
````

WebPortal
----------

Web.config:

````xml
<?xml version="1.0"?>
<configuration>
  <configSections>
    <section name="Leafing.Settings" type="Leafing.Util.Setting.NameValueSectionHandler, Leafing.Util" />
  </configSections>

  <Leafing.Settings>
    <add key="AutoCreateTable" value="true" />
    <add key="DataBase" value="@Access : @~App_Data\test.mdb" />
  </Leafing.Settings>

  <appSettings />
  <connectionStrings />
  <system.web>
  <httpHandlers>
    <add path="*.jpg" verb="GET,HEAD" type="System.Web.StaticFileHandler" validate="true" />
    <add path="*.gif" verb="GET,HEAD" type="System.Web.StaticFileHandler" validate="true" />
    <add path="*.png" verb="GET,HEAD" type="System.Web.StaticFileHandler" validate="true" />
    <add path="*.css" verb="GET,HEAD" type="System.Web.StaticFileHandler" validate="true" />
    <add path="*.js" verb="GET,HEAD" type="System.Web.StaticFileHandler" validate="true" />
    <add path="*.7z" verb="GET,HEAD" type="System.Web.StaticFileHandler" validate="true" />
    <add path="*.zip" verb="GET,HEAD" type="System.Web.StaticFileHandler" validate="true" />
    <add path="*.rar" verb="GET,HEAD" type="System.Web.StaticFileHandler" validate="true" />
    <add path="*.htm" verb="GET,HEAD" type="System.Web.StaticFileHandler" validate="true" />
    <add path="*.html" verb="GET,HEAD" type="System.Web.StaticFileHandler" validate="true" />
    <add path="*" verb="*" type="Leafing.Web.MvcDispatcher, Leafing.Web" validate="true" />
  </httpHandlers>

  <pages pageBaseType="Leafing.Web.Mvc.PageBase, Leafing.Web" />

  <compilation debug="true" />
    <authentication mode="Windows" />
  </system.web>
</configuration>
````

styles/scaffolding.css

````css
body
{
  background-color: #fff;
  color: #333;
}

body, p, ol, ul, td
{
  font-family: verdana, arial, helvetica, sans-serif;
  font-size: 13px;
  line-height: 18px;
}

pre
{
  background-color: #eee;
  padding: 10px;
  font-size: 11px;
}

a
{
  color: #000;
}
a:visited
{
  color: #666;
}
a:hover
{
  color: #fff;
  background-color: #000;
}

.fieldWithErrors
{
  padding: 2px;
  background-color: red;
  display: table;
}

#errorExplanation
{
  width: 400px;
  border: 2px solid red;
  padding: 7px;
  padding-bottom: 12px;
  margin-bottom: 20px;
  background-color: #f0f0f0;
}

#errorExplanation h2
{
  text-align: left;
  font-weight: bold;
  padding: 5px 5px 5px 15px;
  font-size: 12px;
  margin: -7px;
  background-color: #c00;
  color: #fff;
}

#errorExplanation p
{
  color: #333;
  margin-bottom: 0;
  padding: 5px;
}

#errorExplanation ul li
{
  font-size: 12px;
  list-style: square;
}
````

App_Data/test.mdb

>Just a new mdb file with Access 2003 format.

Default.aspx

>Leave the default.aspx when the site be created.

Run it.

The default will show it has a Book controller.

Click "Book" will enter book list page.

Clike "New Book" will enter new book page.

Input something to create one book record. It will return book list page with one row.

Now we can click "Show" to show the book detail.

Or click "Edit" to edit the book information.

Or click "Destory" to destroy this item of book.
