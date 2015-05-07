NameMapper
==========

For some databases, the table name or/and column name need set as Upper or lower, it's easy to compare, but is not good to read.

For example, if we have a table name that is ``SystemLog``, and the name in database will be ``SYSTEMLOG``. DbEntry provides a mapping from code class name to database table name, with it, the table name will be ``SYSTEM_LOG``.

This is for the class which is not define the ``DbTable`` or ``JoinOn``.

And there is a prefix for many-to-many cross table name is ``"R_"``. So the cross table of ``Article`` and ``User`` will be ``R_ARTICLE_USER``.

This is the default configure by now. But if we have some old code and don¡¯t want to change the database table name, we can define it in App/Web.config:

````xml
<add key="NameMapper" value="Leafing.Core.Text.NameMapper, Leafing.Core" />
````

There is another NameMapper will help us to map table name pluralize. It means if we have a class model named ``"Book"``, the mapped table name will be ``"Books"``, the ``"Category"`` will be ``"Categories"``, and ``"Person"`` will be ``"People"``. The config of it is:

````xml
<add key="NameMapper" value="Leafing.Core.Text.InflectionNameMapper, Leafing.Core" />
````
