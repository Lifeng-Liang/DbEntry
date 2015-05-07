Performance Test
==========

The following test shows the performance of DbEntry, ADO.NET directly and use DataSet.

All tests runs twice. The first time is let it do the initialize. _(For example, DbEntry will analyze object model in the first time.)_

The tests read the table with 50000 rows. And it was using SQL Server 2005 Express.

The whole code is in [Performance Test Code](PerformanceTestCode.md).

The result is not stable, this is one of them:

````
Run ADO.NET with GetXXX:
Time span is 360.5184 ms.

Run ADO.NET with GetXXX:
Time span is 320.4608 ms.

Run ADO.NET with index:
Time span is 310.4464 ms.

Run ADO.NET with index:
Time span is 280.4032 ms.

Run ADO.NET with name:
Time span is 460.6624 ms.

Run ADO.NET with name:
Time span is 380.5472 ms.

Run ADO.NET with name don't insert to list:
Time span is 380.5472 ms.

Run ADO.NET with name don't insert to list:
Time span is 260.3744 ms.

Run DbEntry:
Time span is 360.5184 ms.

Run DbEntry:
Time span is 260.3744 ms.

Run DbEntry with SQL:
Time span is 450.648 ms.

Run DbEntry with SQL:
Time span is 410.5904 ms.

Run DataSet:
Time span is 610.8784 ms.

Run DataSet:
Time span is 520.7488 ms.

All end.
````

So we will find the performance of DbEntry almost same as use ADO.NET directly. _(Use Emit mode. It is the default mode in DbEntry.Net v0.32. If using Reflection mode, the spend time of DbEntry will more than DataSet.)_

