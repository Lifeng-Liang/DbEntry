
DbEntry.Net (Leafing Framework)
==========

[![Build Status](https://travis-ci.org/Lifeng-Liang/DbEntry.svg?branch=master)](https://travis-ci.org/Lifeng-Liang/DbEntry)
[![codecov](https://codecov.io/gh/Lifeng-Liang/DbEntry/branch/master/graph/badge.svg)](https://codecov.io/gh/Lifeng-Liang/DbEntry)

DbEntry.Net (Leafing Framework) is a lightweight, high performance ORM compnent for .Net Core 2.0. It has clearly and easily programing interface. It based on ADO.NET, and supported C#, Visual Basic, ASP.NET Core etc.

The samples of the release package all TESTED on Sql Server 2005 Express, Sql Server 2008, MySql 5.0, SQLite 3, Access 2003, Firebird 2.1.0, PostgreSQL 8.3.3 and Oracle 10g express.
Features:

*   Linq support
*   FluentInterface query syntax
*   RoR ActiveRecord style syntax
*   Ambient transaction
*   Dynamic object
*   Partial update
*   1:1 1:Many Many:Many relations
*   Auto create table
*   Anti sql injection
*   Multiple data source
*   Object validation
*   Paged selector and collection
*   Nullable support
*   DbEntryDataSource
*   ASP.NET 2.0 Membership support
*   Built-in Cache Support
*   Ruby On Rails style MVC framework
*   Simple IoC framework
*   High performance, almost same as using ADO.NET directly
*   Lightwight, the binary file only about 360KB

First sight:
----------

```c#
public class User : DbObjectModel<User>
{
	public string Name { get; set; }
	public int Age { get; set; }
	public bool Actived { get; set; }
	public DateTime Birthday { get; set; }
}

static void Main()
{
	// Create
	var u = new User { Name = "tom", Age = 18, Actived = true, Birthday = DateTime.Now };
	u.Save();
	// Read
	var u1 = User.FindById(u.Id);
	// Update
	u1.Name = "jerry";
	u1.Save();
	// Delete
	u1.Delete();
	// Query
	var ids = from p in User.Table where p.Age > 15 select new {p.Id};
	var l1 = from p in User.Table where p.Age > 15 && p.Actived select p;
	var l2 = User.Find(p => p.Age > 15 && p.Actived); // another style of linq
	var l3 = User.FindBySql("Select * From [User] Where [Age] > 15 And [Actived] = true");
}
```

Documentation
----------

### [Documentation Index](doc/en/Index.md)

Links
----------

For MySql, SQLite and Firebird, please install the .net driver first, to unpack 7z archieve, please install 7-Zip:

*   MySql: http://dev.mysql.com/downloads/connector/net/5.1.html
*   SQLite: http://system.data.sqlite.org/
*   Firebird: http://www.firebirdsql.org/index.php?op=files&id=netprovider
*   7-Zip: http://www.7-zip.org
*   Team Explorer: http://www.microsoft.com/downloads/details.aspx?FamilyID=0ed12659-3d41-4420-bbb0-a46e51bfca86&DisplayLang=en
*   VisualSvn: http://visualsvn.com/
*   SvnBridge: http://www.codeplex.com/SvnBridge
*   TestDriven.Net http://www.testdriven.net

Examples:
----------

*   Blog: https://github.com/Lifeng-Liang/Blog
*   DbEntry.Asp and Duwamish: http://sourceforge.net/project/showfiles.php?group_id=124033

My Homepage, Blog(Chinese) and email:
----------

*   Homepage: http://llf.hanzify.org
*   Blog: http://llf.javaeye.com
*   Blog: http://www.cnblogs.com/lephone/
*   Email: lifeng.liang(at)gmail.com
*   Email2: lifeng.liang(at)qq.com (In case gmail blocked by some unknown force again)

Other resources:
----------

*   QQ群：11387761
*   在我的博客园的Blog上，写了几篇角度不同的介绍DbEntry的文章，也可以作为一个补充：http://www.cnblogs.com/lephone/
*   网友Elephant正在翻译DbEntry.Net的文档，在翻译中，他也增加了一些内容，这里是链接：http://www.blogjava.net/enzosoft/category/38039.html
*   这是网友吴晓阳写的一些项目
	*	使用firebird2.1与dbEntry.net做的设备报修小程序
	*	从数据库生成领域类的CodeSmith模板
	*	http://baoming.codeplex.com/
	*	http://ct4dbentry.codeplex.com/ 
*  这是网友 吴鑫宇（wuxinyu） 用 WCF+DbEntry 写的例子
	*	Wcf+dbentry架构

Donate 捐款:
----------

*   [Details 细节](Donate.md)
*   Donate ways 捐款方式:
	*	支付宝: 
		* ![支付宝](doc/imgs/alipay.png)
	*	微信:
		* ![微信](doc/imgs/wechat.png)
	*	Paypal: lifeng.liang(at)gmail.com
	*	Bitcoin: 16ypFeJGaqyQywu9ra1AQw6UaNWP4RoPBf

