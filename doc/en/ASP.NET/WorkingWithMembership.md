Working with Membership
==========

>DbEntry 4.2 doesn't include Membership dll by mistake, download [Leafing.Membership.dll.zip](http://download-codeplex.sec.s-msft.com/Download?ProjectName=dbentry&DownloadId=900402) and upzip it into *DbEntryPath* will be OK.

1\. Create new web site in vs2010, add a web.config and change it as following:

````xml
<?xml version="1.0"?>
<configuration>
  <configSections>
    <section name="Leafing.Settings"
      type="Leafing.Core.Setting.NameValueSectionHandler, Leafing.Core" />
  </configSections>
 
  <Leafing.Settings>
    <add key="AutoCreateTable" value="true" />
    <add key="DataBase" value="@SQLite : @~Test.db" />
    <add key="DbProviderFactory"
value="System.Data.SQLite.SQLiteFactory, System.Data.SQLite, Version=1.0.66.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139" />
  </Leafing.Settings>
  
  <appSettings/>
  <connectionStrings/>
  <system.web>
    <compilation debug="true"/>
 
    <membership defaultProvider="DbEntryMembershipProvider">
      <providers>
        <clear />
        <add name="DbEntryMembershipProvider" type="Leafing.Web.DbEntryMembershipProvider, Leafing.Web"/>
      </providers>
    </membership>
 
    <roleManager defaultProvider="DbEntryRole">
      <providers>
        <clear />
        <add name="DbEntryRole" type="Leafing.Web.DbEntryRoleProvider, Leafing.Web"/>
      </providers>
    </roleManager>
 
    <authentication mode="Forms">
      <forms name=".ADUAUTH" loginUrl="MyLogin.aspx" defaultUrl="Default.aspx" protection="All" />
    </authentication>
 
    <authorization>
      <deny users="?" />
      <allow users="*" />
    </authorization>
  </system.web>
</configuration>
````

2\. Add the references of "Leafing.Core", "Leafing.Data" and "Leafing.Web"

3\. Create a new page named "MyLogin.aspx"

4\. Drag a CreateUserWizard control to MyLogin.aspx

5\. Run this web site and create a user named "tom" and set the password as "123456"

6\. Remove the CreateUserWizard control from MyLogin.aspx

7\. Drag a Login control to MyLogin.aspx

8\. Drag a LoginName control to Default.aspx

9\. Run this web site and use "tom/123456" to login

10\. Use SqlQuerier to check the "Test.db" in web folder, ensure the table already created, and the row of "tom" already created
