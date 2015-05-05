About Namespaces
==========

The Namespaces of DbEntry is organized by its functionality. It has a common prefix namespace ``Leafing``, it’s my homepage address by reversed.

The ``util`` assembly has some utilities:
# util has the common utilities.
# Coding has coding transfer classes, specify for HexStringCoding.
# Logging for log system, there is another part of it in the Data assembly to provide log to database.
# Setting has setting loader.
# Text has some common functions to use text.
# TimingTask has the timing task component.

The ``Data`` assembly has the core database access component:
# Data has the common ORM classes.
# Builder has the SQL builder in it. Normally we don’t need use it directly.
# Common has the non-sorted classes in it, sometimes we need use it.
# Definition has the classes for object definition.
# Dialect has the databases dialect in it. I don’t think we will use it directly.
# Driver has the database drivers to provide basic ADO.NET components.
# Logging has the part of log system in database.
# QuerySyntax has the query syntax support classes, it not for use directly.
# SqlEntry has the classes we need to call SQL directly.

So, when we define the object, we just need ``Leafing.Data.Definition``.

When we use ORM, we just need ``Leafing.Data``.

When we use SQL, we need ``Leafing.Data.SqlEntry``, maybe ``Leafing.Data`` too.

If we want use some functions in Common, we need ``Leafing.Data.Common``.

How to use the utilities is on your own. :)

