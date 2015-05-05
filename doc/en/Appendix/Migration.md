Migration
==========

Basic migration suggestion of v4.2 to recent check-in
----------

# Nothing yet.

Basic migration suggestion of v4.1 to v4.2
----------

# Replace "Lephone" to "Leafing" in all *.csproj files.
# Remove all files in the projects folder which the name has "Lephone" in it.
# Open the solution and replace "Lephone" to "Leafing" in the whole solution.

Basic migration suggestion of v4.0 to v4.1
----------

# Change FindAll() to Find(Condition.Empty).
# Change DeleteAll() to DeleteBy(Condition.Empty).
# Change the events name to the one without "On" in DbEntryDataSource.

Basic migration suggestion of v3.9 to v4.0
----------

# Replace "Lephone.Util" to "Lephone.Core".
# Replace "IoC" to "Ioc".
# Change the models to remove all the abstract.
# Use a text editor to add the [DbEntry MSBuild Section] to csproj file which the models in.
# A little of foreign key‘s name need to change.
# Replace "Lephone.Data.Linq" to "Lephone.Data.Model.Linq".

Basic migration suggestion of v3.6 to v3.9
----------

# If you are using JoinOn, you need use new parameters of it, and CreateTableList has been removed.
# If you are using Linq, Now it moved to other assemlbies. Replace LinqObjectModel to DbObjectModel. If you are using null as parameter of Where function, use Condition.Empty instead. Remove reference of Lephone.Linq and clean you solutions.
# The change set 62160 is the last change set which still supports .net 2.0, use it if you need work under .net 2.0.
# If you are using mvc, replace "ctx." as "Ctx.".
# If you are using mvc, change LinkTo/UrlTo to fluent interface mode.
# If you are using mvc, replace "bag" as "this".
# If you are using mvc, change bag key to upper case such as item to Item, list_count to ListCount etc.
# If you are using mvc, replace "Rails" as "Mvc".
# Replace WhereCondition to Condition, replace EmptyCondition to Empty, replace TrueCondition to True, replace FalseCondition to False.
# Change UsingAspxPostfix to RailsPostfix and value to ".aspx" if you use it.
# If you are using CK<T> and FieldNameGetter<T>, use Linq instead.
# Replace "new DbContext(" to "DbEntry.GetContext("

Basic migration suggestion of v3.5 to v3.6
----------

# Change Label to NoticeLabel if use DbEntryDataSource to handle error message.
# Replace function Create_ManyToManyMediTable to CreateCrossTable if you used it.
# Replace function Create_DeleteToTable to CreateDeleteToTable if you used it.
# "AutoCreateTable" now related with DbContext, so in web.config/app.config you should use the right prefix for it.
# Replace "Paramter" to "Parameter".
# Replace ".New()" to ".New" and change some back to ".New()" if it is XmlBuilder.New().
# Change constructors with parameters to abstract Init function if you like.
# Change Init functions to abstract if you like.

Basic migration suggestion of v0.33 to v3.5
----------

# If overrided ExecuteDelete of DbEntryDataSource before, the paramter is the object not the key now. So you need change the code of it.
# Replace "UsingTransaction" to "NewTransation".
# Replace "UsingExistedTransaction" to "UsingTransation".
# Replace "UsingConnection" to "NewConnection".
# Replace "UsingExistedConnection" to "UsingConnection".
# If you use "EnumTable" before, replace it to "LephoneEnum".
# If you use "LogTable" before, replace it to "LephoneLog".
# If you use "HexStringCoding" before, it now in "Lephone.Util.Text".

Basic migration suggestion of v0.32 to v0.33
----------

# Open *.project file by text editor, replace all "org.hanzify.llf.util" to "Lephone.Util"
# Then replace all "org.hanzify.llf" to "Lephone"
# Open solution, replace all "org.hanzify.llf.util" to "Lephone.Util" with whole solution
# Then replace all "org.hanzify.llf" to "Lephone" with whole solution
# Then replace all "DbEntryException" to "DataException" with whole solution
# If there is also have missing liberay problem, re-select them.
# If used DbEntryDataSource, add the reference of "Lephone.Web" change the namespace to "Lephone.Web"
# If used DbEntryDataSource work with GridView, add the property "DataKeyNames" values "Id" to the GridView.
# If used UrlBuilder, add the reference of "Lephone.Web" change the namespace to "Lephone.Web"
# If used MaxLengthAttribute like "MaxLength(50)", change it to "Length(50)"
# If used "CreateManyToManyMediTable" function, replace it to "Create_ManyToManyMediTable"
# Replace snippets if you imported them before.
# Add IDbObject interface to the data model which is inherits from System.Object.

Basic migration suggestion of v0.31 to v0.32
----------

# DbEntry.RequeredTransaction should rename as DbEntry.UsingExistedTransaction
# HasManyAndBelongsTo should rename as HasAndBelongsToMany
# If it is not dynamic object, and it is relation object, the relation fields should be initialized in constructors
# OrderByAttribute changed to abstract, change OrderBy as paramter with attributes such as HasOne, HasMany, HasAndBelongsToMany
# In DbKeyAttribute, IsSystemGeneration should rename as IsDbGenerate.
# If use Access, add ``<add key="ObjectHandlerType" value="Reflection" />`` to config file.
