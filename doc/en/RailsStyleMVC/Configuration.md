Configuration
==========

The model configuration please visit [Data Configuration](../DataAccess/Configuration.md).

The other configuration for Mvc, please use:

````xml
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
    <add path="*.xml" verb="GET,HEAD" type="System.Web.StaticFileHandler" validate="true" />
    <add path="*" verb="*" type="Leafing.Web.MvcDispatcher, Leafing.Web" validate="true" />
  </httpHandlers>

  <pages pageBaseType="Leafing.Web.Mvc.PageBase, Leafing.Web" />
</system.web>
````

It's a little long, but alway be the same. You can just copy it to your web.config.

There is another setting item to help deployment:

````xml
<Leafing.Settings>
  <add key="MvcPostfix" value=".aspx" />
<Leafing.Settings>
````

If it set to ".aspx", every URL will have a ".aspx" postfix so IIS will pass it to our code without any IIS configuration (need uncheck "Verify file exists"). And we can change the value to ".html" if we want, don't forget to remove ".html" from HttpHandler section of web.config and it needs to set IIS to pass all html to our application.

And we can define the scaffolding views to use a master page as well:

````xml
<Leafing.Settings>
  <add key="ScaffoldingMasterPage" value="~/main.master" />
<Leafing.Settings>
````

We also could use ContentPlaceHolderHead and ContentPlaceHolderBody to define the PlaceHolderId it will use. Mostly we don't need do it because the default value of them are same as default values in master page.

Normally, DbEntry will search all loaded assemblies to find controllers, but we can specify it as well:

````xml
<Leafing.Settings>
  <add key="ControllerAssembly" value="MyApp.Biz" />
<Leafing.Settings>
````
