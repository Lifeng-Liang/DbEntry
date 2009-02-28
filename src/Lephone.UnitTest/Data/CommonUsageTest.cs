using System;
using System.Collections;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;
using Lephone.Linq;
using Lephone.MockSql.Recorder;
using Lephone.UnitTest.Data.CreateTable;
using Lephone.UnitTest.Data.Objects;
using Lephone.Util.Logging;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    #region Objects

    public abstract class t_user : LinqObjectModel<t_user>
    {
        [Length(40)]
        public abstract string mc { get; set; }

        [SpecialName, LazyLoad]
        public abstract DateTime CreatedOn { get; set; }
    }

    [DbTable("People")]
    class SinglePerson : DbObject
    {
        public string Name;
    }

    [DbTable("People")]
    public abstract class UniquePerson : DbObjectModel<UniquePerson>
    {
        [Index(UNIQUE = true)]
        public abstract string Name { get; set; }
    }

    public class CountTable : DbObject
    {
        [SpecialName]
        public int Count;
    }

    public abstract class CountTable2 : DbObjectModel<CountTable2>
    {
        public abstract string Name { get; set; }

        [SpecialName]
        public abstract int Count { get; set; }
    }

    [DbTable("People")]
    public abstract class FieldPerson : DbObjectModel<FieldPerson>
    {
        [DbColumn("Name")]
        public abstract string theName { get; set; }

        public static FieldPerson FindByName(string Name)
        {
            return FindOne(Field["theName"] == Name);
        }
    }

    [DbTable("LockVersionTest")]
    public abstract class LockVersionTest : DbObjectModel<LockVersionTest>
    {
        public abstract string Name { get; set; }

        [SpecialName]
        public abstract int LockVersion { get; set; }
    }

    public class MKEY : IDbObject
    {
        [DbKey(IsDbGenerate = false)]
        public string FirstName;

        [DbKey(IsDbGenerate = false)]
        public string LastName;

        public int Age;
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
                .Where(WhereCondition.EmptyCondition)
                .OrderBy("Id")
                .Range(1, 1)
                .Select();

            Assert.AreEqual(1, l.Count);
            Assert.AreEqual(1, l[0].Id);
            Assert.AreEqual("Tom", l[0].Name);

            l = DbEntry
                .From<SinglePerson>()
                .Where(WhereCondition.EmptyCondition)
                .OrderBy("Id")
                .Range(2, 2)
                .Select();

            Assert.AreEqual(1, l.Count);
            Assert.AreEqual(2, l[0].Id);
            Assert.AreEqual("Jerry", l[0].Name);

            l = DbEntry
                .From<SinglePerson>()
                .Where(WhereCondition.EmptyCondition)
                .OrderBy("Id")
                .Range(3, 5)
                .Select();

            Assert.AreEqual(1, l.Count);
            Assert.AreEqual(3, l[0].Id);
            Assert.AreEqual("Mike", l[0].Name);

            l = DbEntry
                .From<SinglePerson>()
                .Where(WhereCondition.EmptyCondition)
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
            Assert.AreEqual(3, DbEntry.From<Category>().Where(WhereCondition.EmptyCondition).GetCount());
            Assert.AreEqual(5, DbEntry.From<Book>().Where(WhereCondition.EmptyCondition).GetCount());
            Assert.AreEqual(2, DbEntry.From<Book>().Where(CK.K["Category_Id"] == 3).GetCount());
        }

        [Test]
        public void Test4()
        {
            List<GroupByObject<long>> l = DbEntry
                .From<Book>()
                .Where(WhereCondition.EmptyCondition)
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
                .Where(WhereCondition.EmptyCondition)
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

            p = PeopleModel.New();
            p.Name = "123456";
            Assert.IsFalse(p.IsValid());

            Assert.AreEqual(1, PeopleModel.CountName("Tom"));
            Assert.AreEqual(0, PeopleModel.CountName("xyz"));
        }

        [Test]
        public void TestSql()
        {
            PeopleModel p1 = DbEntry.Context.ExecuteList<PeopleModel>("Select [Id],[Name] From [People] Where [Id] = 2")[0];
            Assert.AreEqual("Jerry", p1.Name);
            p1 = DbEntry.Context.ExecuteList<PeopleModel>(new SqlStatement("Select [Name],[Id] From [People] Where [Id] = 1"))[0];
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

            DArticle a = DArticle.New();
            a.Name = "long";
            Assert.AreEqual("{ Id = 0, Name = long }", a.ToString());

            var c = new ImpPCs {Name = "HP"};
            Assert.AreEqual("{ Id = 0, Name = HP, Person_Id = 0 }", c.ToString());
        }

        [Test]
        public void TestColumnCompColumn()
        {
            //WhereCondition c = CK.K["Age"] > CK.K["Count"];
            var c = CK.K["Age"].Gt(CK.K["Count"]);
            var dpc = new DataParamterCollection();
            string s = c.ToSqlText(dpc, DbEntry.Context.Dialect);
            Assert.AreEqual(0, dpc.Count);
            Assert.AreEqual("[Age] > [Count]", s);
        }

        [Test]
        public void TestColumnCompColumn2()
        {
            var c = CK.K["Age"] > CK.K["Count"];
            var dpc = new DataParamterCollection();
            string s = c.ToSqlText(dpc, DbEntry.Context.Dialect);
            Assert.AreEqual(0, dpc.Count);
            Assert.AreEqual("[Age] > [Count]", s);
        }

        [Test]
        public void TestColumnCompColumn3()
        {
            var c = CK.K["Age"] > CK.K["Count"] && CK.K["Name"] == CK.K["theName"] || CK.K["Age"] <= CK.K["Num"];
            var dpc = new DataParamterCollection();
            string s = c.ToSqlText(dpc, DbEntry.Context.Dialect);
            Assert.AreEqual(0, dpc.Count);
            Assert.AreEqual("(([Age] > [Count]) And ([Name] = [theName])) Or ([Age] <= [Num])", s);
        }

        [Test]
        public void TestGetSqlStetement()
        {
            SqlStatement sql = DbEntry.Context.GetSqlStatement("select * from User where Age > ? And Age < ?", 18, 23);
            Assert.AreEqual("select * from User where Age > @p0 And Age < @p1", sql.SqlCommandText);
            Assert.AreEqual("@p0", sql.Paramters[0].Key);
            Assert.AreEqual(18, sql.Paramters[0].Value);
            Assert.AreEqual("@p1", sql.Paramters[1].Key);
            Assert.AreEqual(23, sql.Paramters[1].Value);
        }

        [Test]
        public void TestGetSqlStetement2()
        {
            SqlStatement sql = DbEntry.Context.GetSqlStatement("Select * from User where Id = ? Name Like '%?%' Age > ? And Age < ? ", 1, 18, 23);
            Assert.AreEqual("Select * from User where Id = @p0 Name Like '%?%' Age > @p1 And Age < @p2 ", sql.SqlCommandText);
            Assert.AreEqual("@p0", sql.Paramters[0].Key);
            Assert.AreEqual(1, sql.Paramters[0].Value);
            Assert.AreEqual("@p1", sql.Paramters[1].Key);
            Assert.AreEqual(18, sql.Paramters[1].Value);
            Assert.AreEqual("@p2", sql.Paramters[2].Key);
            Assert.AreEqual(23, sql.Paramters[2].Value);
        }

        [Test]
        public void TestGetSqlStetementByExecuteList()
        {
            List<Person> ls = DbEntry.Context.ExecuteList<Person>("select * from [People] where Id > ? And Id < ?", 1, 3);
            Assert.AreEqual(1, ls.Count);
            Assert.AreEqual("Jerry", ls[0].Name);
        }

        [Test]
        public void TestGuidKey()
        {
            GuidKey o = GuidKey.New();
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
        public void TestUniqueValidate()
        {
            var u = UniquePerson.New();
            u.Name = "test";
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
            var n = ConsoleMessageRecorder.Count;
            Assert.IsTrue(p.IsValid());
            Assert.AreEqual(n, ConsoleMessageRecorder.Count);
            p.Name = "Jerry";
            Assert.IsFalse(p.IsValid());
            Assert.AreEqual(n + 1, ConsoleMessageRecorder.Count);
        }

        [Test]
        public void TestFindOneWithSqlServer2005()
        {
            var de = new DbContext("SqlServerMock");
            var p = de.GetObject<Person>(CK.K["Name"] == "test", null);
            Assert.IsNull(p);
        }

        [Test]
        public void Test2ndPageWithSqlserver2005()
        {
            var de = new DbContext("SqlServerMock");
            StaticRecorder.ClearMessages();
            de.From<Person>().Where(CK.K["Age"] > 18).OrderBy("Id").Range(3, 5).Select();
            Assert.AreEqual("select [Id],[Name] from (select [Id],[Name], ROW_NUMBER() OVER ( Order By [Id] ASC) as __rownumber__ From [People]  Where [Age] > @Age_0) as T Where T.__rownumber__ >= 3 and T.__rownumber__ <= 5;\n<Text><60>(@Age_0=18:Int32)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestTableNameMapOfConfig()
        {
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(Lephone.Data.Logging.LephoneLog));
            Assert.AreEqual("System_Log", oi.From.GetMainTableName());

            oi = ObjectInfo.GetInstance(typeof(LephoneEnum));
            Assert.AreEqual("Lephone_Enum", oi.From.GetMainTableName());
        }

        [Test]
        public void Test_CK_Field()
        {
            var de = new DbContext("SqlServerMock");
            StaticRecorder.ClearMessages();
            de.From<PropertyClassWithDbColumn>().Where(CK<PropertyClassWithDbColumn>.Field["TheName"] == "tom").Select();
            Assert.AreEqual("Select [Id],[Name] From [People] Where [Name] = @Name_0;\n<Text><60>(@Name_0=tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestNull()
        {
            var de = new DbContext("SqlServerMock");
            StaticRecorder.ClearMessages();
            de.From<PropertyClassWithDbColumn>().Where(CK.K["Name"] == null).Select();
            Assert.AreEqual("Select [Id],[Name] From [People] Where [Name] Is NULL;\n<Text><60>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestNotNull()
        {
            var de = new DbContext("SqlServerMock");
            StaticRecorder.ClearMessages();
            de.From<PropertyClassWithDbColumn>().Where(CK.K["Name"] != null).Select();
            Assert.AreEqual("Select [Id],[Name] From [People] Where [Name] Is Not NULL;\n<Text><60>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCountTable()
        {
            var de = new DbContext("SqlServerMock");
            StaticRecorder.ClearMessages();
            var ct = new CountTable {Id = 1};
            de.Save(ct);
            Assert.AreEqual("Update [Count_Table] Set [Count]=[Count]+1  Where [Id] = @Id_0;\n<Text><30>(@Id_0=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCountTable2()
        {
            var de = new DbContext("SqlServerMock");
            StaticRecorder.ClearMessages();
            CountTable2 ct = CountTable2.New();
            ct.Id = 1;
            ct.Name = "tom";
            de.Save(ct);
            Assert.AreEqual("Update [Count_Table2] Set [Name]=@Name_0,[Count]=[Count]+1  Where [Id] = @Id_1;\n<Text><30>(@Name_0=tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
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
            var b = crxBook1.New();
            b.Name = "test";

            var c = crxCategory1.New();
            c.Name = "math";

            c.Books.Add(b);

            c.Save();

            var c1 = crxCategory1.FindById(c.Id);
            Assert.AreEqual("math", c1.Name);
            Assert.AreEqual(1, c1.Books.Count);
            Assert.AreEqual("test", c1.Books[0].Name);
        }

        [Test]
        public void TestMKEY()
        {
            DbEntry.Context.Create(typeof(MKEY));

            var p1 = new MKEY {FirstName = "test", LastName = "next", Age = 11};
            DbEntry.Insert(p1);

            var p2 = DbEntry.From<MKEY>().Where(p => p.FirstName == "test" && p.LastName == "next").Select()[0];
            Assert.AreEqual(11, p2.Age);

            p2.Age = 18;
            DbEntry.Update(p2);

            var p3 = DbEntry.From<MKEY>().Where(p => p.FirstName == "test" && p.LastName == "next").Select()[0];
            Assert.AreEqual(18, p3.Age);
        }

        [Test]
        public void TestLazyLoadCreatedOn()
        {
            try
            {
                var d = t_user.New();
                d.mc = "张三";
                d.Save();
            }
            catch(Exception ex)
            {
                Assert.AreEqual("SpecialName colomn could not be LazyLoad", ex.Message);
            }
        }

        [Test]
        public void TestMKEYForUpdate()
        {
            var p = new MKEY { FirstName = "test", LastName = "next", Age = 11 };
            sqlite.Update(p);
            AssertSql(@"Update [MKEY] Set [Age]=@Age_0  Where ([FirstName] = @FirstName_1) And ([LastName] = @LastName_2);
<Text><30>(@Age_0=11:Int32,@FirstName_1=test:String,@LastName_2=next:String)");
        }

        [Test]
        public void TestLowerFunction()
        {
            sqlite.From<SinglePerson>().Where(CK.K["Name"].ToLower() == "tom").Select();
            AssertSql(@"Select [Id],[Name] From [People] Where lower([Name]) = @Name_0;
<Text><60>(@Name_0=tom:String)");
        }

        [Test]
        public void TestLowerForLike()
        {
            sqlite.From<SinglePerson>().Where(CK.K["Name"].ToLower().Like("%tom%")).Select();
            AssertSql(@"Select [Id],[Name] From [People] Where lower([Name]) Like @Name_0;
<Text><60>(@Name_0=%tom%:String)");
        }
    }
}
