Mapping relations
==========

>It only shows the mapping for SqlServer, other database please find it by yourself.

| C# Property Defination                                         | Sql Server Column Type |
| -------------------------------------------------------------- | ---------------------- |
| ``int Column``                                                 | int                    |
| ``long Column``                                                | bigint                 |
| ``short Column``                                               | smallint               |
| ``byte Column``                                                | tinyint                |
| ``Guid Column``                                                | uniqueidentifier       |
| ``bool Column``                                                | bit                    |
| ``DateTime Column``                                            | datetime               |
| ``decimal Column``                                             | decimal                |
| ``float Column``                                               | real                   |
| ``double Column``                                              | float                  |
| ``byte[] Column``                                              | binary                 |
| ``string Column``                                              | ntext                  |
| ``[StringColumn(IsUnicode = false)]string Column``             | text/varchar(max)      |
| ``[Length(30)]string Column``                                  | nvarchar(30)           |
| ``[StringColumn(IsUnicode = false), Length(30)]string Column`` | varchar(30)            |
