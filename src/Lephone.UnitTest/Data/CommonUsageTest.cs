using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Data.Model;
using Lephone.Data.SqlEntry;
using Lephone.MockSql.Recorder;
using Lephone.UnitTest.Data.CreateTable;
using Lephone.UnitTest.Data.Objects;
using Lephone.Core.Logging;
using Lephone.Extra;
using Lephone.Extra.Logging;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    #region Objects

    [DbTable("People")]
    public class SavePeople : DbObjectModel<SavePeople>
    {
        public string Name { get; set; }

        [Exclude]
        public UniquePerson ExcludeColumn { get; set; }
    }

    [DbTable("File")]
    public class DistinctTest : IDbObject
    {
        [DbColumn("BelongsTo_Id")] public long? N;
    }

    [DbTable("People")]
    public class SinglePerson : DbObject
    {
        public string Name;
    }

    [DbTable("People"), DbContext("SQLite")]
    public class SinglePersonSqlite : DbObject
    {
        public string Name;
    }

    [DbTable("People")]
    public class UniquePerson : DbObjectModel<UniquePerson>
    {
        [Index(UNIQUE = true)]
        public string Name { get; set; }
    }

    public class CountTable : DbObject
    {
        [SpecialName]
        public int Count;
    }

    [DbContext("SqlServerMock")]
    public class CountTableSql : DbObject
    {
        [SpecialName]
        public int Count;
    }

    public class CountTable2 : DbObjectModel<CountTable2>
    {
        public string Name { get; set; }

        [SpecialName]
        public int Count { get; set; }
    }

    [DbContext("SqlServerMock")]
    public class CountTable2Sql : DbObjectModel<CountTable2Sql>
    {
        public string Name { get; set; }

        [SpecialName]
        public int Count { get; set; }
    }

    [DbTable("People")]
    public class FieldPerson : DbObjectModel<FieldPerson>
    {
        [DbColumn("Name")]
        public string TheName { get; set; }

        public static FieldPerson FindByName(string name)
        {
            return FindOne(p => p.TheName == name);
        }
    }

    [DbTable("People"), DbContext("SqlServerMock")]
    public class FieldPersonSql : DbObjectModel<FieldPersonSql>
    {
        [DbColumn("Name")]
        public string TheName { get; set; }

        public static FieldPersonSql FindByName(string name)
        {
            return FindOne(p => p.TheName == name);
        }
    }

    [DbTable("LockVersionTest")]
    public class LockVersionTest : DbObjectModel<LockVersionTest>
    {
        public string Name { get; set; }

        [SpecialName]
        public int LockVersion { get; set; }
    }

    public class Mkey : IDbObject
    {
        [DbKey(IsDbGenerate = false)]
        public string FirstName;

        [DbKey(IsDbGenerate = false)]
        public string LastName;

        public int Age;
    }

    [DbContext("SQLite")]
    public class Mkey2 : IDbObject
    {
        [DbKey(IsDbGenerate = false)]
        public string FirstName;

        [DbKey(IsDbGenerate = false)]
        public string LastName;

        public int Age;
    }

    [DbContext("SQLite")]
    public class FindByModel : DbObjectModel<FindByModel>
    {
        [DbColumn("FirstName")]
        public string Name { get; set; }
        public int Age { get; set; }
    }

    [Serializable]
    public abstract class Contentable<T> : DbObjectModel<T> where T : Contentable<T>, new()
    {
        [DbColumn("Content"), LazyLoad]
        public string ItemContent { get; set; }
    }

    [DbContext("SQLite")]
    public class WithContent : Contentable<WithContent>
    {
        public string Name { get; set; }
    }

    #endregion

    [TestFixture]
    public class CommonUsageTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            var p = new SinglePerson {Name = "abc"};
            Assert.AreEqual(0, p.Id);

            DbEntry.Save(p);
            Assert.IsTrue(0 != p.Id);
            var p1 = DbEntry.GetObject<SinglePerson>(p.Id);
            Assert.AreEqual(p.Name, p1.Name);

            p.Name = "xyz";
            DbEntry.Save(p);
            Assert.AreEqual(p.Id, p1.Id);

            p1 = DbEntry.GetObject<SinglePerson>(p.Id);
            Assert.AreEqual("xyz", p1.Name);

            long id = p.Id;
            DbEntry.Delete(p);
            Assert.AreEqual(0, p.Id);
            p1 = DbEntry.GetObject<SinglePerson>(id);
            Assert.IsNull(p1);
        }

        [Test]
        public void Test2()
        {
            List<SinglePerson> l = DbEntry
                .From<SinglePerson>()
                .Where(Condition.Empty)
                .OrderBy("Id")
                .Range(1, 1)
                .Select();

            Assert.AreEqual(1, l.Count);
            Assert.AreEqual(1, l[0].Id);
            Assert.AreEqual("Tom", l[0].Name);

            l = DbEntry
                .From<SinglePerson>()
                .Where(Condition.Empty)
                .OrderBy("Id")
                .Range(2, 2)
                .Select();

            Assert.AreEqual(1, l.Count);
            Assert.AreEqual(2, l[0].Id);
            Assert.AreEqual("Jerry", l[0].Name);

            l = DbEntry
                .From<SinglePerson>()
                .Where(Condition.Empty)
                .OrderBy("Id")
                .Range(3, 5)
                .Select();

            Assert.AreEqual(1, l.Count);
            Assert.AreEqual(3, l[0].Id);
            Assert.AreEqual("Mike", l[0].Name);

            l = DbEntry
                .From<SinglePerson>()
                .Where(Condition.Empty)
                .OrderBy((DESC)"Id")
                .Range(3, 5)
                .Select();

            Assert.AreEqual(1, l.Count);
            Assert.AreEqual(1, l[0].Id);
            Assert.AreEqual("Tom", l[0].Name);
        }

        [Test]
        public void Test3()
        {
            Assert.AreEqual(3, DbEntry.From<Category>().Where(Condition.Empty).GetCount());
            Assert.AreEqual(5, DbEntry.From<Book>().Where(Condition.Empty).GetCount());
            Assert.AreEqual(2, DbEntry.From<Book>().Where(CK.K["Category_Id"] == 3).GetCount());
        }

        [Test]
        public void Test4()
        {
            List<GroupByObject<long>> l = DbEntry
                .From<Book>()
                .Where(Condition.Empty)
                .OrderBy((DESC)DbEntry.CountColumn)
                .GroupBy<long>("Category_Id");

            Assert.AreEqual(2, l[0].Column);
            Assert.AreEqual(3, l[0].Count);

            Assert.AreEqual(3, l[1].Column);
            Assert.AreEqual(2, l[1].Count);
        }

        [Test]
        public void Test5()
        {
            IList l = DbEntry
                .From<Book>()
                .Where(Condition.Empty)
                .GroupBy<string>("Name");

            Assert.AreEqual(5, l.Count);

            l = DbEntry
                .From<Book>()
                .Where(CK.K["Id"] > 2)
                .GroupBy<string>("Name");

            Assert.AreEqual(3, l.Count);

            List<GroupByObject<string>> ll = DbEntry
                .From<Book>()
                .Where(CK.K["Id"] > 2)
                .OrderBy("Name")
                .GroupBy<string>("Name");

            Assert.AreEqual(3, ll.Count);
            Assert.AreEqual("Pal95", ll[0].Column);
            Assert.AreEqual("Shanghai", ll[1].Column);
            Assert.AreEqual("Wow", ll[2].Column);
        }

        [Test]
        public void TestPeopleModel()
        {
            PCs.DeleteAll(); // avoid FK error.
            List<PeopleModel> l = PeopleModel.FindAll();
            Assert.AreEqual(3, l.Count);
            Assert.AreEqual("Tom", l[0].Name);

            PeopleModel p = PeopleModel.FindByName("Jerry");
            Assert.AreEqual(2, p.Id);
            Assert.IsTrue(p.IsValid());

            p.Name = "llf";
            Assert.IsTrue(p.IsValid());
            p.Save();

            PeopleModel p1 = PeopleModel.FindById(2);
            Assert.AreEqual("llf", p1.Name);

            p.Delete();
            p1 = PeopleModel.FindById(2);
            Assert.IsNull(p1);

            p = new PeopleModel {Name = "123456"};
            Assert.IsFalse(p.IsValid());

            Assert.AreEqual(1, PeopleModel.CountName("Tom"));
            Assert.AreEqual(0, PeopleModel.CountName("xyz"));
        }

        [Test]
        public void TestSql()
        {
            PeopleModel p1 = DbEntry.ExecuteList<PeopleModel>("Select [Id],[Name] From [People] Where [Id] = 2")[0];
            Assert.AreEqual("Jerry", p1.Name);
            p1 = DbEntry.ExecuteList<PeopleModel>(new SqlStatement("Select [Name],[Id] From [People] Where [Id] = 1"))[0];
            Assert.AreEqual("Tom", p1.Name);
            p1 = PeopleModel.FindBySql("Select [Id],[Name] From [People] Where [Id] = 2")[0];
            Assert.AreEqual("Jerry", p1.Name);
            p1 = PeopleModel.FindBySql(new SqlStatement("Select [Name],[Id] From [People] Where [Id] = 3"))[0];
            Assert.AreEqual("Mike", p1.Name);
        }

        [Test]
        public void ToStringTest()
        {
            var p = new ImpPeople {Name = "tom"};
            Assert.AreEqual("{ Id = 0, Name = tom }", p.ToString());

            var a = new DArticle {Name = "long"};
            Assert.AreEqual("{ Id = 0, Name = long }", a.ToString());

            var c = new ImpPCs {Name = "HP"};
            Assert.AreEqual("{ Id = 0, Name = HP, Person_Id = <NULL> }", c.ToString());
        }

        [Test]
        public void TestColumnCompColumn()
        {
            //Condition c = CK.K["Age"] > CK.K["Count"];
            var c = CK.K["Age"].Gt(CK.K["Count"]);
            var dpc = new DataParameterCollection();
            string s = c.ToSqlText(dpc, DbEntry.Provider.Dialect);
            Assert.AreEqual(0, dpc.Count);
            Assert.AreEqual("[Age] > [Count]", s);
        }

        [Test]
        public void TestColumnCompColumn2()
        {
            var c = CK.K["Age"] > CK.K["Count"];
            var dpc = new DataParameterCollection();
            string s = c.ToSqlText(dpc, DbEntry.Provider.Dialect);
            Assert.AreEqual(0, dpc.Count);
            Assert.AreEqual("[Age] > [Count]", s);
        }

        [Test]
        public void TestColumnCompColumn3()
        {
            var c = CK.K["Age"] > CK.K["Count"] && CK.K["Name"] == CK.K["theName"] || CK.K["Age"] <= CK.K["Num"];
            var dpc = new DataParameterCollection();
            string s = c.ToSqlText(dpc, DbEntry.Provider.Dialect);
            Assert.AreEqual(0, dpc.Count);
            Assert.AreEqual("(([Age] > [Count]) AND ([Name] = [theName])) OR ([Age] <= [Num])", s);
        }

        [Test]
        public void TestGetSqlStetement()
        {
            SqlStatement sql = DbEntry.Provider.GetSqlStatement("SELECT * FROM User WHERE Age > ? AND Age < ?", 18, 23);
            Assert.AreEqual("SELECT * FROM User WHERE Age > @p0 AND Age < @p1", sql.SqlCommandText);
            Assert.AreEqual("@p0", sql.Parameters[0].Key);
            Assert.AreEqual(18, sql.Parameters[0].Value);
            Assert.AreEqual("@p1", sql.Parameters[1].Key);
            Assert.AreEqual(23, sql.Parameters[1].Value);
        }

        [Test]
        public void TestGetSqlStetement2()
        {
            SqlStatement sql = DbEntry.Provider.GetSqlStatement("SELECT * FROM User WHERE Id = ? Name LIKE '%?%' Age > ? AND Age < ? ", 1, 18, 23);
            Assert.AreEqual("SELECT * FROM User WHERE Id = @p0 Name LIKE '%?%' Age > @p1 AND Age < @p2 ", sql.SqlCommandText);
            Assert.AreEqual("@p0", sql.Parameters[0].Key);
            Assert.AreEqual(1, sql.Parameters[0].Value);
            Assert.AreEqual("@p1", sql.Parameters[1].Key);
            Assert.AreEqual(18, sql.Parameters[1].Value);
            Assert.AreEqual("@p2", sql.Parameters[2].Key);
            Assert.AreEqual(23, sql.Parameters[2].Value);
        }

        [Test]
        public void TestGetSqlStetementByExecuteList()
        {
            List<Person> ls = DbEntry.ExecuteList<Person>("SELECT * FROM [People] WHERE Id > ? AND Id < ?", 1, 3);
            Assert.AreEqual(1, ls.Count);
            Assert.AreEqual("Jerry", ls[0].Name);
        }

        [Test]
        public void TestGuidKey()
        {
            var o = new GuidKey();
            Assert.IsTrue(Guid.Empty == o.Id);

            o.Name = "guid";
            o.Save();

            Assert.IsFalse(Guid.Empty == o.Id);

            GuidKey o1 = GuidKey.FindById(o.Id);
            Assert.AreEqual("guid", o1.Name);

            o.Name = "test";
            o.Save();

            GuidKey o2 = GuidKey.FindById(o.Id);
            Assert.AreEqual("test", o2.Name);

            o2.Delete();
            GuidKey o3 = GuidKey.FindById(o.Id);
            Assert.IsNull(o3);
        }

        [Test]
        public void TestGuidColumn()
        {
            var g = Guid.NewGuid();
            var o = new GuidColumn {TheGuid = g};
            o.Save();

            var o1 = GuidColumn.FindById(o.Id);
            Assert.IsNotNull(o1);
            Assert.AreEqual(g, o1.TheGuid);

            var g1 = Guid.NewGuid();
            o1.TheGuid = g1;
            o1.Save();

            Assert.IsFalse(g == g1);

            var o2 = GuidColumn.FindById(o.Id);
            Assert.IsTrue(g1 == o2.TheGuid);
        }

        [Test]
        public void TestUniqueValidate()
        {
            var u = new UniquePerson {Name = "test"};
            var vh = new ValidateHandler();
            vh.ValidateObject(u);
            Assert.IsTrue(vh.IsValid);

            u.Name = "Tom";
            vh = new ValidateHandler();
            vh.ValidateObject(u);
            Assert.IsFalse(vh.IsValid);
            Assert.AreEqual("Invalid Field Name Should be UNIQUED.", vh.ErrorMessages["Name"]);

            // smart validate
            var p = DbEntry.GetObject<UniquePerson>(1);
            var n = ConsoleMessageLogRecorder.Count;
            Assert.IsTrue(p.IsValid());
            Assert.AreEqual(n, ConsoleMessageLogRecorder.Count);
            p.Name = "Jerry";
            Assert.IsFalse(p.IsValid());
            Assert.AreEqual(n + 1, ConsoleMessageLogRecorder.Count);
        }

        [Test]
        public void TestFindOneWithSqlServer2005()
        {
            var p = DbEntry.GetObject<Person>(o => o.Name == "test");
            Assert.IsNull(p);
        }

        [Test]
        public void Test2NdPageWithSqlserver2005()
        {
            StaticRecorder.ClearMessages();
            DbEntry.From<PersonSql>().Where(CK.K["Age"] > 18).OrderBy("Id").Range(3, 5).Select();
            Assert.AreEqual("SELECT [Id],[Name] FROM (SELECT [Id],[Name], ROW_NUMBER() OVER ( ORDER BY [Id] ASC) AS __rownumber__ FROM [People]  WHERE [Age] > @Age_0) AS T WHERE T.__rownumber__ >= 3 AND T.__rownumber__ <= 5;\n<Text><60>(@Age_0=18:Int32)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test2NdPageWithSqlserver2005WithAlias()
        {
            StaticRecorder.ClearMessages();
            DbEntry.From<FieldPersonSql>().Where(CK.K["Age"] > 18).OrderBy("Id").Range(3, 5).Select();
            Assert.AreEqual("SELECT [Id],[TheName] FROM (SELECT [Id],[Name] AS [TheName], ROW_NUMBER() OVER ( ORDER BY [Id] ASC) AS __rownumber__ FROM [People]  WHERE [Age] > @Age_0) AS T WHERE T.__rownumber__ >= 3 AND T.__rownumber__ <= 5;\n<Text><60>(@Age_0=18:Int32)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestTableNameMapOfConfig()
        {
            ObjectInfo oi = ModelContext.GetInstance(typeof(LephoneLog)).Info;
            Assert.AreEqual("System_Log", oi.From.MainTableName);

            oi = ModelContext.GetInstance(typeof(LephoneEnum)).Info;
            Assert.AreEqual("Lephone_Enum", oi.From.MainTableName);
        }

        //[Test]
        //public void Test_CK_Field()
        //{
        //    var de = new DbContext("SqlServerMock");
        //    StaticRecorder.ClearMessages();
        //    de.From<PropertyClassWithDbColumn>().Where(CK<PropertyClassWithDbColumn>.Field["TheName"] == "tom").Select();
        //    Assert.AreEqual("SELECT [Id],[Name] AS [TheName] FROM [People] WHERE [Name] = @Name_0;\n<Text><60>(@Name_0=tom:String)", StaticRecorder.LastMessage);
        //}

        [Test]
        public void TestNull()
        {
            StaticRecorder.ClearMessages();
            DbEntry.From<PropertyClassWithDbColumnSql>().Where(CK.K["Name"] == null).Select();
            Assert.AreEqual("SELECT [Id],[Name] AS [TheName] FROM [People] WHERE [Name] IS NULL;\n<Text><60>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestNotNull()
        {
            StaticRecorder.ClearMessages();
            DbEntry.From<PropertyClassWithDbColumnSql>().Where(CK.K["Name"] != null).Select();
            Assert.AreEqual("SELECT [Id],[Name] AS [TheName] FROM [People] WHERE [Name] IS NOT NULL;\n<Text><60>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCountTable()
        {
            StaticRecorder.ClearMessages();
            var ct = new CountTableSql {Id = 1};
            DbEntry.Save(ct);
            Assert.AreEqual("UPDATE [Count_Table_Sql] SET [Count]=[Count]+1  WHERE [Id] = @Id_0;\n<Text><30>(@Id_0=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCountTable2()
        {
            StaticRecorder.ClearMessages();
            var ct = new CountTable2Sql {Id = 1, Name = "tom"};
            DbEntry.Save(ct);
            Assert.AreEqual("UPDATE [Count_Table2Sql] SET [Name]=@Name_0,[Count]=[Count]+1  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestFieldNameMapper()
        {
            FieldPerson p = FieldPerson.FindByName("Jerry");
            Assert.AreEqual(2, p.Id);
        }

        [Test]
        public void TestLockVersion()
        {
            var item = LockVersionTest.FindById(1);
            Assert.AreEqual(1, item.LockVersion);
            item.Name = "jerry";
            item.Save();

            var item0 = LockVersionTest.FindById(1);
            Assert.AreEqual(2, item0.LockVersion);
        }

        [Test, ExpectedException(typeof(DataException))]
        public void TestLockVersionException()
        {
            var item = LockVersionTest.FindById(1);
            var item2 = LockVersionTest.FindById(1);

            item.Name = "jerry";
            item.Save();

            item2.Name = "mike";
            item2.Save();
        }

        [Test]
        public void TestDefineCrossTableName3()
        {
            var b = new crxBook1 {Name = "test"};

            var c = new crxCategory1 {Name = "math"};

            c.Books.Add(b);

            c.Save();

            var c1 = crxCategory1.FindById(c.Id);
            Assert.AreEqual("math", c1.Name);
            Assert.AreEqual(1, c1.Books.Count);
            Assert.AreEqual("test", c1.Books[0].Name);
        }

        [Test]
        public void TestMkey()
        {
            DbEntry.Create(typeof(Mkey));

            var p1 = new Mkey {FirstName = "test", LastName = "next", Age = 11};
            DbEntry.Insert(p1);

            var p2 = DbEntry.From<Mkey>().Where(p => p.FirstName == "test" && p.LastName == "next").Select()[0];
            Assert.AreEqual(11, p2.Age);

            p2.Age = 18;
            DbEntry.Update(p2);

            var p3 = DbEntry.From<Mkey>().Where(p => p.FirstName == "test" && p.LastName == "next").Select()[0];
            Assert.AreEqual(18, p3.Age);
        }

        [Test]
        public void TestMkeyForUpdate()
        {
            var p = new Mkey2 { FirstName = "test", LastName = "next", Age = 11 };
            DbEntry.Update(p);
            AssertSql(@"UPDATE [Mkey2] SET [Age]=@Age_0  WHERE ([FirstName] = @FirstName_1) AND ([LastName] = @LastName_2);
<Text><30>(@Age_0=11:Int32,@FirstName_1=test:String,@LastName_2=next:String)");
        }

        [Test]
        public void TestLowerFunction()
        {
            DbEntry.From<SinglePersonSqlite>().Where(CK.K["Name"].ToLower() == "tom").Select();
            AssertSql(@"SELECT [Id],[Name] FROM [People] WHERE LOWER([Name]) = @Name_0;
<Text><60>(@Name_0=tom:String)");
        }

        [Test]
        public void TestLowerForLike()
        {
            DbEntry.From<SinglePersonSqlite>().Where(CK.K["Name"].ToLower().Like("%tom%")).Select();
            AssertSql(@"SELECT [Id],[Name] FROM [People] WHERE LOWER([Name]) LIKE @Name_0;
<Text><60>(@Name_0=%tom%:String)");
        }

        [Test]
        public void TestUpperFunction()
        {
            DbEntry.From<SinglePersonSqlite>().Where(CK.K["Name"].ToUpper() == "tom").Select();
            AssertSql(@"SELECT [Id],[Name] FROM [People] WHERE UPPER([Name]) = @Name_0;
<Text><60>(@Name_0=tom:String)");
        }

        [Test]
        public void TestUpperForLike()
        {
            DbEntry.From<SinglePersonSqlite>().Where(CK.K["Name"].ToUpper().Like("%tom%")).Select();
            AssertSql(@"SELECT [Id],[Name] FROM [People] WHERE UPPER([Name]) LIKE @Name_0;
<Text><60>(@Name_0=%tom%:String)");
        }

        [Test]
        public void TestMax()
        {
            DbEntry.From<SinglePersonSqlite>().Where(Condition.Empty).GetMax("Id");
            AssertSql(@"SELECT MAX([Id]) AS [Id] FROM [People];
<Text><60>()");

            var n = DbEntry.From<SinglePerson>().Where(Condition.Empty).GetMax("Id");
            Assert.AreEqual(3, n);

            n = FieldPerson.GetMax(null, "Id");
            Assert.AreEqual(3, n);
        }

        [Test]
        public void TestMin()
        {
            DbEntry.From<SinglePersonSqlite>().Where(Condition.Empty).GetMin("Id");
            AssertSql(@"SELECT MIN([Id]) AS [Id] FROM [People];
<Text><60>()");

            var n = DbEntry.From<SinglePerson>().Where(Condition.Empty).GetMin("Id");
            Assert.AreEqual(1, n);

            n = FieldPerson.GetMin(null, "Id");
            Assert.AreEqual(1, n);
        }

        [Test]
        public void TestMaxDate()
        {
            StaticRecorder.CurRow.Add(new RowInfo(new DateTime()));
            DbEntry.From<DateAndTimeSqlite>().Where(Condition.Empty).GetMaxDate("dtValue");
            AssertSql(@"SELECT MAX([dtValue]) AS [dtValue] FROM [DateAndTime];
<Text><60>()");

            var n = DbEntry.From<DateAndTime>().Where(Condition.Empty).GetMaxDate("dtValue");
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), n);

            n = DateAndTime.GetMaxDate(null, "dtValue");
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), n);
        }

        [Test]
        public void TestMinDate()
        {
            StaticRecorder.CurRow.Add(new RowInfo(new DateTime()));
            DbEntry.From<DateAndTimeSqlite>().Where(Condition.Empty).GetMinDate("dtValue");
            AssertSql(@"SELECT MIN([dtValue]) AS [dtValue] FROM [DateAndTime];
<Text><60>()");

            var n = DbEntry.From<DateAndTime>().Where(Condition.Empty).GetMinDate("dtValue");
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), n);

            n = DateAndTime.GetMinDate(null, "dtValue");
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), n);
        }

        [Test]
        public void TestSum()
        {
            DbEntry.From<SinglePersonSqlite>().Where(Condition.Empty).GetSum("Id");
            AssertSql(@"SELECT SUM([Id]) AS [Id] FROM [People];
<Text><60>()");

            var n = DbEntry.From<SinglePerson>().Where(Condition.Empty).GetSum("Id");
            Assert.AreEqual(6, n);

            n = FieldPerson.GetSum(null, "Id");
            Assert.AreEqual(6, n);
        }

        [Test]
        public void TestDistinct()
        {
            var list = DbEntry.From<DistinctTest>().Where(Condition.Empty).OrderBy(p => p.N).SelectDistinct();
            Assert.AreEqual(9, list.Count);
            var exps = new[] {0, 1, 2, 3, 4, 9, 11, 15, 16};
            for(int i = 1; i < 9; i++)
            {
                Assert.AreEqual(exps[i], list[i].N);
            }
        }

        [Test]
        public void TestDistinctPagedSelector()
        {
            var query = DbEntry.From<DistinctTest>().Where(Condition.Empty).OrderBy(p => p.N).PageSize(3).GetDistinctPagedSelector();
            Assert.AreEqual(8, query.GetResultCount());
            var list = (List<DistinctTest>)query.GetCurrentPage(1);
            Assert.AreEqual(3, list.Count);
            var exps = new[] { 3, 4, 9 };
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(exps[i], list[i].N);
            }
        }

        [Test]
        public void TestWhereFunctionOfDbObjectModel()
        {
            var list = UniquePerson.Where(CK.K["Name"] == "Tom").Select();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(1, list[0].Id);
        }

        [Test]
        public void TestGroupbySum()
        {
            DbEntry.From<SinglePersonSqlite>().Where(Condition.Empty).GroupBySum<string, long>("Name", "Id");
            AssertSql(@"SELECT [Name],SUM([Id]) AS [Id] FROM [People] GROUP BY [Name];
<Text><60>()");

            var list = DbEntry.From<Book>().Where(Condition.Empty).GroupBySum<long, long>("Category_Id", "Id");
            var sorted = (from o in list orderby o.Column select o).ToList();

            Assert.AreEqual(2, sorted[0].Column);
            Assert.AreEqual(10, sorted[0].Sum);

            Assert.AreEqual(3, sorted[1].Column);
            Assert.AreEqual(5, sorted[1].Sum);
        }

        [Test]
        public void TestInClause()
        {
            DbEntry.From<SinglePersonSqlite>().Where(CK.K["Id"].In(1, 3, 5, 7)).Select();
            AssertSql(@"SELECT [Id],[Name] FROM [People] WHERE [Id] IN (@in_0,@in_1,@in_2,@in_3);
<Text><60>(@in_0=1:Int32,@in_1=3:Int32,@in_2=5:Int32,@in_3=7:Int32)");
        }

        [Test]
        public void TestInSql()
        {
            DbEntry.From<SinglePersonSqlite>().Where(CK.K["Id"].InSql("Select Id From Others")).Select();
            AssertSql(@"SELECT [Id],[Name] FROM [People] WHERE [Id] IN (Select Id From Others);
<Text><60>()");
        }

        [Test]
        public void TestFindBy()
        {
            FindByModel.FindBy.NameAndAge("tom", 18);
            AssertSql(@"SELECT [Id],[FirstName] AS [Name],[Age] FROM [Find_By_Model] WHERE ([FirstName] = @FirstName_0) AND ([Age] = @Age_1);
<Text><60>(@FirstName_0=tom:String,@Age_1=18:Int32)");
        }

        [Test]
        public void TestFindBy2()
        {
            FindByModel.FindBy.Name("tom");
            AssertSql(@"SELECT [Id],[FirstName] AS [Name],[Age] FROM [Find_By_Model] WHERE [FirstName] = @FirstName_0;
<Text><60>(@FirstName_0=tom:String)");
        }

        [Test, ExpectedException(typeof(DataException))]
        public void TestFindBy3()
        {
            FindByModel.FindBy.NameAge("tom", 18);
        }

        [Test, ExpectedException(typeof(DataException))]
        public void TestFindBy4()
        {
            FindByModel.FindBy.Name("tom", 18);
        }

        [Test]
        public void TestExecuteDynamicTable()
        {
            dynamic table = DbEntry.Provider.ExecuteDynamicTable("Select * From People Order By Id");
            Assert.AreEqual(3, table.Count);
            Assert.AreEqual(1, table[0].Id);
            Assert.AreEqual("Tom", table[0].Name);
            Assert.AreEqual(2, table[1].Id);
            Assert.AreEqual("Jerry", table[1].Name);
            Assert.AreEqual(3, table[2].Id);
            Assert.AreEqual("Mike", table[2].Name);
        }

        [Test]
        public void TestExecuteDynamicList()
        {
            dynamic list = DbEntry.Provider.ExecuteDynamicList("Select * From People Order By Id");
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0].Id);
            Assert.AreEqual("Tom", list[0].Name);
            Assert.AreEqual(2, list[1].Id);
            Assert.AreEqual("Jerry", list[1].Name);
            Assert.AreEqual(3, list[2].Id);
            Assert.AreEqual("Mike", list[2].Name);
        }

        [Test]
        public void TestExecuteDynamicRow()
        {
            dynamic row = DbEntry.Provider.ExecuteDynamicRow("Select * From People Where Id = 1");
            Assert.AreEqual("Tom", row.Name);
        }

        [Test]
        public void TestExecuteDynamicRowForNull()
        {
            dynamic row = DbEntry.Provider.ExecuteDynamicRow("Select * From NullTest Where Id = 3");
            Assert.AreEqual(3, row.Id);
            Assert.IsNull(row.Name);
            Assert.IsNull(row.MyInt);
            Assert.IsNull(row.MyBool);
        }

        [Test]
        public void TestExecuteDynamicSet()
        {
            dynamic set = DbEntry.Provider.ExecuteDynamicSet(@"Select * From People Order By Id;
Select * From PCs Order By Id;");
            Assert.AreEqual(2, set.Count);
            Assert.AreEqual(3, set[0].Count);
            Assert.AreEqual(3, set[1].Count);
            Assert.AreEqual("Tom", set[0][0].Name);
            Assert.AreEqual("IBM", set[1][0].Name);
        }

        [Test]
        public void TestWithContent()
        {
            var c = new WithContent {Name = "tom", ItemContent = "test"};
            c.Save();
            AssertSql(@"INSERT INTO [With_Content] ([Name],[Content]) VALUES (@Name_0,@Content_1);
SELECT LAST_INSERT_ROWID();
<Text><30>(@Name_0=tom:String,@Content_1=test:String)");

            c.Name = "jerry";
            c.Save();
            AssertSql(string.Format(@"UPDATE [With_Content] SET [Name]=@Name_0  WHERE [Id] = @Id_1;
<Text><30>(@Name_0=jerry:String,@Id_1={0}:Int64)", c.Id));

            c.ItemContent = "update";
            c.Save();
            AssertSql(string.Format(@"UPDATE [With_Content] SET [Content]=@Content_0  WHERE [Id] = @Id_1;
<Text><30>(@Content_0=update:String,@Id_1={0}:Int64)", c.Id));
        }

        [Test]
        public void TestWithContentSelect()
        {
            StaticRecorder.CurRow.Add(new RowInfo("Id", 1L));
            StaticRecorder.CurRow.Add(new RowInfo("Name", "tom"));
            var c = WithContent.FindById(1);
            Assert.AreEqual("tom", c.Name);
            StaticRecorder.CurRow.Add(new RowInfo("Content", "test"));
            Assert.AreEqual("test", c.ItemContent);
            AssertSql(@"SELECT [Content] FROM [With_Content] WHERE [Id] = @Id_0;
<Text><60>(@Id_0=1:Int64)");
        }

        [Test]
        public void TestInCluse()
        {
            var smt = DbEntry.From<PCs>().Where(p => p.Id >= 2).GetStatement(p => p.Id);
            var list = DbEntry.From<Person>().Where(CK.K["Id"].InStatement(smt)).OrderBy(p => p.Id).Select();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Jerry", list[0].Name);
            Assert.AreEqual("Mike", list[1].Name);
        }

        [Test]
        public void TestInCluse2()
        {
            var smt = DbEntry.From<PCs>().Where(p => p.Id >= 2).GetStatement("Id");
            var list = DbEntry.From<Person>().Where(CK.K["Id"].InStatement(smt)).OrderBy(p => p.Id).Select();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Jerry", list[0].Name);
            Assert.AreEqual("Mike", list[1].Name);
        }
    }
}
