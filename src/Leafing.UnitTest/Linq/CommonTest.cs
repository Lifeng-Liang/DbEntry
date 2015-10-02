using System;
using System.Collections.Generic;
using System.Linq;
using Leafing.Data;
using Leafing.Data.Definition;
using Leafing.Data.Model.Linq;
using Leafing.MockSql.Recorder;
using Leafing.UnitTest.Data;
using NUnit.Framework;

namespace Leafing.UnitTest.Linq
{
    [TestFixture]
    public class CommonTest : DataTestBase
    {
        public int nnn = 30;

        [DbTable("People")]
        public class Person : DbObjectModel<Person>
        {
            [DbColumn("Name")]
            public string FirstName { get; set; }
        }

        [DbTable("People"), DbContext("SQLite")]
        public class PersonSqlite : DbObjectModel<PersonSqlite>
        {
            [DbColumn("Name")]
            public string FirstName { get; set; }
        }

        public enum MyEnum
        {
            Test1,
            Test2,
        }

        [DbContext("SQLite")]
        public class EnumTest : DbObjectModel<EnumTest>
        {
            [DbColumn("ccc")]
            public MyEnum Abc { get; set; }
        }

        [DbContext("SQLite")]
        public class BoolTest : DbObjectModel<BoolTest>
        {
            public string Name { get; set; }
            public bool Available { get; set; }
        }

        [DbContext("SQLite")]
        public class Nolazy : DbObjectModel<Nolazy>
        {
            public int No { get; set; }

			public LazyLoad<string> Content { get; set; }

            [Length(10)]
            public string Name { get; set; }

			public Nolazy ()
			{
				Content = new LazyLoad<string>(this, "Content");
			}
        }

        [DbContext("SQLite")]
        public class User : DbObjectModel<User>
        {
            public string Name { get; set; }

			public HasMany<Article> Articles { get; private set; }

			public User ()
			{
				Articles = new HasMany<Article>(this, "Id", "User_Id");
			}
        }

        [DbContext("SQLite")]
        public class Article : DbObjectModel<Article>
        {
            public string Title { get; set; }

			public BelongsTo<User, long> Writer { get; set; }

			// get foreign key without read writer.
            public long WriterId
			{
				get
				{
					return (long)Writer.ForeignKey;
				}
			}

			public Article ()
			{
				Writer = new BelongsTo<User, long>(this, "User_Id");
			}
        }

        [DbContext("SQLite")]
        public class TestClass : IDbObject
        {
            public string Name;
            public int Age;
            public bool Gender;
        }

        [Test]
        public void Test1()
        {
            DbEntry.From<PersonSqlite>()
                .Where(p => p.FirstName.StartsWith("T") && (p.Id >= 1 || p.Id == 15) && p.FirstName != null && p.FirstName == p.FirstName)
                .OrderBy(p => p.Id)
                .Select();

            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE ((([Name] LIKE @Name_0) AND (([Id] >= @Id_1) OR ([Id] = @Id_2))) AND ([Name] IS NOT NULL)) AND ([Name] = [Name]) ORDER BY [Id] ASC;\n<Text><60>(@Name_0=T%:String,@Id_1=1:Int64,@Id_2=15:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test2()
        {
            DbEntry.From<PersonSqlite>()
                .Where(p => p.FirstName.EndsWith("T")).Select();
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Name] LIKE @Name_0;\n<Text><60>(@Name_0=%T:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test3()
        {
            DbEntry.From<PersonSqlite>()
                .Where(p => p.FirstName.Contains("T")).Select();
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Name] LIKE @Name_0;\n<Text><60>(@Name_0=%T%:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test4()
        {
            DbEntry.GetObject<PersonSqlite>(p => p.FirstName == "Tom");
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Name] = @Name_0;\n<Text><60>(@Name_0=Tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test5()
        {
            DbEntry.From<PersonSqlite>().Where(Condition.Empty).OrderBy(p => p.FirstName).ThenBy(p => p.Id).Select();
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] ORDER BY [Name] ASC,[Id] ASC;\n<Text><60>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test6()
        {
            DbEntry.From<PersonSqlite>().Where(Condition.Empty).OrderByDescending(p => p.FirstName).ThenBy(p => p.Id).Select();
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] ORDER BY [Name] DESC,[Id] ASC;\n<Text><60>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test7()
        {
            const int id1 = 1;
            const int id2 = 15;

            DbEntry.From<PersonSqlite>()
                .Where(p => p.FirstName.StartsWith("T") && (p.Id >= id1 || p.Id == id2) && p.FirstName != null && p.FirstName == p.FirstName)
                .OrderBy(p => p.Id)
                .Select();

            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE ((([Name] LIKE @Name_0) AND (([Id] >= @Id_1) OR ([Id] = @Id_2))) AND ([Name] IS NOT NULL)) AND ([Name] = [Name]) ORDER BY [Id] ASC;\n<Text><60>(@Name_0=T%:String,@Id_1=1:Int64,@Id_2=15:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test8()
        {
            TestMember(1, 15);
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE ((([Name] LIKE @Name_0) AND (([Id] >= @Id_1) OR ([Id] = @Id_2))) AND ([Name] IS NOT NULL)) AND ([Name] = [Name]) ORDER BY [Id] ASC;\n<Text><60>(@Name_0=T%:String,@Id_1=1:Int64,@Id_2=15:Int64)", StaticRecorder.LastMessage);
        }

        private void TestMember(int id1, int id2)
        {
            DbEntry.From<PersonSqlite>()
                .Where(p => p.FirstName.StartsWith("T") && (p.Id >= id1 || p.Id == id2) && p.FirstName != null && p.FirstName == p.FirstName)
                .OrderBy(p => p.Id)
                .Select();
        }

        [Test]
        public void Test9()
        {
            TestMember2(1, 15);
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Id] >= @Id_0 ORDER BY [Id] ASC;\n<Text><60>(@Id_0=3:Int64)", StaticRecorder.LastMessage);
        }

        private void TestMember2(int id1, int id2)
        {
            DbEntry.From<PersonSqlite>()
                .Where(p => p.Id >= id1 + 2)
                .OrderBy(p => p.Id)
                .Select();
        }

        [Test]
        public void Test10()
        {
            TestMember3(3, 15);
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Id] >= @Id_0 ORDER BY [Id] ASC;\n<Text><60>(@Id_0=3:Int64)", StaticRecorder.LastMessage);
        }

        private static void TestMember3(int id1, int id2)
        {
            DbEntry.From<PersonSqlite>()
                .Where(p => p.Id >= id1)
                .OrderBy(p => p.Id)
                .Select();
        }

        [Test]
        public void Test11()
        {
            TestMember4(this, 3);
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Id] >= @Id_0 ORDER BY [Id] ASC;\n<Text><60>(@Id_0=27:Int64)", StaticRecorder.LastMessage);
        }

        private static void TestMember4(CommonTest tt, int id1)
        {
            DbEntry.From<PersonSqlite>()
                .Where(p => p.Id >= tt.nnn - id1)
                .OrderBy(p => p.Id)
                .Select();
        }

        [Test]
        public void Test12()
        {
            const string name1 = "tom";

            DbEntry.From<PersonSqlite>()
                .Where(p => p.FirstName.StartsWith(name1))
                .OrderBy(p => p.Id)
                .Select();

            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Name] LIKE @Name_0 ORDER BY [Id] ASC;\n<Text><60>(@Name_0=tom%:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test13()
        {
            const string name1 = "tom";

            DbEntry.From<PersonSqlite>()
                .Where(p => p.FirstName != name1)
                .OrderBy(p => p.Id)
                .Select();

            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Name] <> @Name_0 ORDER BY [Id] ASC;\n<Text><60>(@Name_0=tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test14()
        {
            const string name1 = "tom";

            DbEntry.From<PersonSqlite>()
                .Where(p => p.FirstName != name1 + " cat")
                .OrderBy(p => p.Id)
                .Select();

            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Name] <> @Name_0 ORDER BY [Id] ASC;\n<Text><60>(@Name_0=tom cat:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test15()
        {
            string[] ss = null;
            string s = ss.FirstItem();
            Assert.IsNull(s);

            s = ss.LastItem();
            Assert.IsNull(s);

            ss = new[]{"1st", "2nd", "3rd"};
            Assert.AreEqual("1st", ss.FirstItem());
            Assert.AreEqual("3rd", ss.LastItem());

            ss = ss.RemoveFirst();
            Assert.AreEqual(2, ss.Length);
            Assert.AreEqual("2nd", ss[0]);
            Assert.AreEqual("3rd", ss[1]);

            ss = ss.RemoveLast();
            Assert.AreEqual(1, ss.Length);
            Assert.AreEqual("2nd", ss[0]);
        }

        [Test]
        public void Test16()
        {
            List<string> list = null;
            string s = list.LastItem();
            Assert.IsNull(s);

            s = list.FirstItem();
            Assert.IsNull(s);

            list = new List<string> {"1st", "2nd", "3rd"};
            Assert.AreEqual("1st", list.FirstItem());
            Assert.AreEqual("3rd", list.LastItem());

            list.RemoveFirst();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("2nd", list[0]);
            Assert.AreEqual("3rd", list[1]);

            list.RemoveLast();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("2nd", list[0]);
        }

        [Test]
        public void TestLowerFunction()
        {
            DbEntry.From<PersonSqlite>().Where(p => p.FirstName.ToLower() == "tom").Select();
            AssertSql(@"SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE LOWER([Name]) = @Name_0;
<Text><60>(@Name_0=tom:String)");
        }

        [Test]
        public void TestLowerForLike()
        {
            DbEntry.From<PersonSqlite>().Where(p => p.FirstName.ToLower().Contains("tom")).Select();
            AssertSql(@"SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE LOWER([Name]) LIKE @Name_0;
<Text><60>(@Name_0=%tom%:String)");
        }

        [Test]
        public void TestUpperFunction()
        {
            DbEntry.From<PersonSqlite>().Where(p => p.FirstName.ToUpper() == "tom").Select();
            AssertSql(@"SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE UPPER([Name]) = @Name_0;
<Text><60>(@Name_0=tom:String)");
        }

        [Test]
        public void TestUpperForLike()
        {
            DbEntry.From<PersonSqlite>().Where(p => p.FirstName.ToUpper().Contains("tom")).Select();
            AssertSql(@"SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE UPPER([Name]) LIKE @Name_0;
<Text><60>(@Name_0=%tom%:String)");
        }

        [Test]
        public void TestMax()
        {
            var n = DbEntry.From<Person>().Where(Condition.Empty).GetMax(p => p.Id);
            Assert.AreEqual(3, n);

            n = Person.GetMax(Condition.Empty, p => p.Id);
            Assert.AreEqual(3, n);

            var x = Person.GetMax(p => p.Id > 100, p => p.Id);
            Assert.IsNull(x);
        }

        [Test]
        public void TestMin()
        {
            var n = DbEntry.From<Person>().Where(Condition.Empty).GetMin(p => p.Id);
            Assert.AreEqual(1, n);

            n = Person.GetMin(Condition.Empty, p => p.Id);
            Assert.AreEqual(1, n);

            var x = Person.GetMin(p => p.Id > 100, p => p.Id);
            Assert.IsNull(x);
        }

        [Test]
        public void TestMaxDate()
        {
            var n = DbEntry.From<DateAndTime>().Where(Condition.Empty).GetMaxDate(p => p.dtValue);
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), n);

            n = DateAndTime.GetMaxDate(Condition.Empty, p => p.dtValue);
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), n);

            var x = DateAndTime.GetMaxDate(p => p.Id > 100, p => p.dtValue);
            Assert.IsNull(x);
        }

        [Test]
        public void TestMinDate()
        {
            var n = DbEntry.From<DateAndTime>().Where(Condition.Empty).GetMinDate(p => p.dtValue);
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), n);

            n = DateAndTime.GetMinDate(Condition.Empty, p => p.dtValue);
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), n);

            var x = DateAndTime.GetMinDate(p => p.Id > 100, p => p.dtValue);
            Assert.IsNull(x);
        }

        [Test]
        public void TestSum()
        {
            var n = DbEntry.From<Person>().Where(Condition.Empty).GetSum(p => p.Id);
            Assert.AreEqual(6, n);

            n = Person.GetSum(Condition.Empty, p => p.Id);
            Assert.AreEqual(6, n);

            var x = Person.GetSum(p => p.Id > 100, p => p.Id);
			Assert.IsTrue(x == null || x == 0m);
        }

        [Test]
        public void TestDelete()
        {
            DbEntry.DeleteBy<PersonSqlite>(p => p.FirstName.EndsWith("T"));
            Assert.AreEqual("DELETE FROM [People] WHERE [Name] LIKE @Name_0;\n<Text><30>(@Name_0=%T:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestEnum()
        {
            DbEntry.From<EnumTest>().Where(p => p.Abc == MyEnum.Test1).Select();
            AssertSql("SELECT [Id],[ccc] AS [Abc] FROM [Enum_Test] WHERE [ccc] = @ccc_0;\n<Text><60>(@ccc_0=0:Int32)");
        }

        [Test]
        public void TestBoolOfDefault()
        {
            DbEntry.From<BoolTest>().Where(p => p.Available).Select();
            AssertSql("SELECT [Id],[Name],[Available] FROM [Bool_Test] WHERE [Available] = @Available_0;\n<Text><60>(@Available_0=True:Boolean)");
        }

        [Test]
        public void TestBoolOfDefault2()
        {
            DbEntry.From<BoolTest>().Where(p => p.Available && p.Name == "tom").Select();
            AssertSql("SELECT [Id],[Name],[Available] FROM [Bool_Test] WHERE ([Available] = @Available_0) AND ([Name] = @Name_1);\n<Text><60>(@Available_0=True:Boolean,@Name_1=tom:String)");
        }

        [Test]
        public void TestInClause()
        {
            DbEntry.From<BoolTest>().Where(p => p.Id.In(1, 3, 5, 7)).Select();
            AssertSql("SELECT [Id],[Name],[Available] FROM [Bool_Test] WHERE [Id] IN (@in_0,@in_1,@in_2,@in_3);\n<Text><60>(@in_0=1:Int64,@in_1=3:Int64,@in_2=5:Int64,@in_3=7:Int64)");
        }

        [Test]
        public void TestInClause2()
        {
            var list = new long[] { 1, 3, 5, 7 };
            DbEntry.From<BoolTest>().Where(p => p.Id.In(list)).Select();
            AssertSql("SELECT [Id],[Name],[Available] FROM [Bool_Test] WHERE [Id] IN (@in_0,@in_1,@in_2,@in_3);\n<Text><60>(@in_0=1:Int64,@in_1=3:Int64,@in_2=5:Int64,@in_3=7:Int64)");
        }

        [Test]
        public void TestInClause3()
        {
            BoolTest.Where(p => p.Id.InStatement(BoolTest.Where(x => x.Id > 10).GetStatement(x => x.Id))).Select();
            AssertSql("SELECT [Id],[Name],[Available] FROM [Bool_Test] WHERE [Id] IN (SELECT [Id] FROM [Bool_Test] WHERE [Id] > @Id_0);\n<Text><60>(@Id_0=10:Int64)");
        }

        [Test]
        public void TestInClause4()
        {
            BoolTest.Where(p => p.Id.InStatement(EnumTest.Where(x => x.Id >= 5).GetStatement(x => x.Id))).Select();
            AssertSql("SELECT [Id],[Name],[Available] FROM [Bool_Test] WHERE [Id] IN (SELECT [Id] FROM [Enum_Test] WHERE [Id] >= @Id_0);\n<Text><60>(@Id_0=5:Int64)");
        }

        [Test]
        public void TestInClause5()
        {
			User.Where(p => p.Id.InStatement(Article.Where(x => x.Id >= 5).GetStatement(x => x.Writer.Value.Id))).Select();
            AssertSql("SELECT [Id],[Name] FROM [User] WHERE [Id] IN (SELECT [User_Id] FROM [Article] WHERE [Id] >= @Id_0);\n<Text><60>(@Id_0=5:Int64)");
        }

        [Test]
        public void TestOrderByFk()
        {
			Article.Where(x => x.Id >= 5).OrderBy(x => x.Writer.Value.Id).Select();
            AssertSql("SELECT [Id],[Title],[User_Id] AS [Writer] FROM [Article] WHERE [Id] >= @Id_0 ORDER BY [User_Id] ASC;\n<Text><60>(@Id_0=5:Int64)");
        }

        [Test]
        public void TestNoLazy()
        {
            Nolazy.Where(Condition.Empty).Select();
            AssertSql("SELECT [Id],[No],[Name] FROM [Nolazy];\n<Text><60>()");

            Nolazy.Where(Condition.Empty).SelectDistinct();
            AssertSql("SELECT DISTINCT [Id],[No],[Name] FROM [Nolazy];\n<Text><60>()");

            Nolazy.Where(Condition.Empty).SelectNoLazy();
            AssertSql("SELECT [Id],[No],[Name],[Content] FROM [Nolazy];\n<Text><60>()");

            Nolazy.Where(Condition.Empty).SelectDistinctNoLazy();
            AssertSql("SELECT DISTINCT [Id],[No],[Name],[Content] FROM [Nolazy];\n<Text><60>()");
        }

        [Test]
        public void TestNoLazy2()
        {
            StaticRecorder.CurRow.Add(new RowInfo("Id", 1L));
            StaticRecorder.CurRow.Add(new RowInfo("No", 2));
            StaticRecorder.CurRow.Add(new RowInfo("Name", "aha"));
            StaticRecorder.CurRow.Add(new RowInfo("Content", "I'm here"));

            var list = Nolazy.Where(Condition.Empty).SelectNoLazy();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(2, list[0].No);
            Assert.AreEqual("aha", list[0].Name);
			Assert.AreEqual("I'm here", list[0].Content.Value);
            AssertSql("SELECT [Id],[No],[Name],[Content] FROM [Nolazy];\n<Text><60>()");
        }

        [Test]
        public void TestSpecialFk()
        {
            StaticRecorder.CurRow.Add(new RowInfo("Id", 1L));
            StaticRecorder.CurRow.Add(new RowInfo("Title", "tom"));
            StaticRecorder.CurRow.Add(new RowInfo("$Writer", 8L));
            var obj = Article.FindById(1);
            AssertSql(@"SELECT [Id],[Title],[User_Id] AS [Writer] FROM [Article] WHERE [Id] = @Id_0;
<Text><60>(@Id_0=1:Int64)");
            Assert.AreEqual("tom", obj.Title);
            Assert.AreEqual(8, obj.WriterId);
            AssertSql(string.Empty);
        }

        [Test]
        public void TestSimpleClass()
        {
            StaticRecorder.CurRow.Add(new RowInfo("Name", "jerry"));
            var list = (from obj in new LinqQueryProvider<TestClass, TestClass>(null) 
                        where obj.Age > 10 select new { obj.Name }).ToList();
            AssertSql(@"SELECT [Name] FROM [Test_Class] WHERE [Age] > @Age_0;
<Text><60>(@Age_0=10:Int32)");
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("jerry", list[0].Name);
        }

        [Test]
        public void TestIsNull()
        {
			Article.Find(p => p.Writer.Value.Id.IsNull());
            AssertSql(@"SELECT [Id],[Title],[User_Id] AS [Writer] FROM [Article] WHERE [User_Id] IS NULL;
<Text><60>()");

        }

        [Test]
        public void TestIsNull1()
        {
			Article.Find(p => p.Writer.Value.Id.IsNotNull());
            AssertSql(@"SELECT [Id],[Title],[User_Id] AS [Writer] FROM [Article] WHERE [User_Id] IS NOT NULL;
<Text><60>()");

        }

        [Test]
        public void TestIsNull2()
        {
			Article.Find(p => p.Writer.Value.Id.IsNull());
            AssertSql(@"SELECT [Id],[Title],[User_Id] AS [Writer] FROM [Article] WHERE [User_Id] IS NULL;
<Text><60>()");
        }

        [Test]
        public void TestIsNull3()
        {
			Article.Find(p => p.Writer.Value.Id.IsNotNull());
            AssertSql(@"SELECT [Id],[Title],[User_Id] AS [Writer] FROM [Article] WHERE [User_Id] IS NOT NULL;
<Text><60>()");
        }

        [Test]
        public void TestIsNull4()
        {
            Article.Find(p => p.Writer == null);
            AssertSql(@"SELECT [Id],[Title],[User_Id] AS [Writer] FROM [Article] WHERE [User_Id] IS NULL;
<Text><60>()");

        }

        [Test]
        public void TestIsNull5()
        {
            Article.Find(p => p.Writer != null);
            AssertSql(@"SELECT [Id],[Title],[User_Id] AS [Writer] FROM [Article] WHERE [User_Id] IS NOT NULL;
<Text><60>()");
        }

        [Test]
        public void TestNotIn()
        {
            DbEntry.From<BoolTest>().Where(p => p.Id.NotIn(1, 3, 5, 7)).Select();
            AssertSql("SELECT [Id],[Name],[Available] FROM [Bool_Test] WHERE [Id] NOT IN (@in_0,@in_1,@in_2,@in_3);\n<Text><60>(@in_0=1:Int64,@in_1=3:Int64,@in_2=5:Int64,@in_3=7:Int64)");
        }

        [Test]
        public void TestNotIn1()
        {
            DbEntry.From<BoolTest>().Where(p => p.Name.NotIn("Tom", "Jerry")).Select();
            AssertSql("SELECT [Id],[Name],[Available] FROM [Bool_Test] WHERE [Name] NOT IN (@in_0,@in_1);\n<Text><60>(@in_0=Tom:String,@in_1=Jerry:String)");
        }

        [Test]
        public void TestNotIn2()
        {
            DbEntry.From<BoolTest>().Where(CK.K["Id"].NotIn(1, 3, 5, 7)).Select();
            AssertSql("SELECT [Id],[Name],[Available] FROM [Bool_Test] WHERE [Id] NOT IN (@in_0,@in_1,@in_2,@in_3);\n<Text><60>(@in_0=1:Int32,@in_1=3:Int32,@in_2=5:Int32,@in_3=7:Int32)");
        }

        [Test]
        public void TestNotIn3()
        {
            DbEntry.From<BoolTest>().Where(CK.K["Name"].NotIn("Tom", "Jerry")).Select();
            AssertSql("SELECT [Id],[Name],[Available] FROM [Bool_Test] WHERE [Name] NOT IN (@in_0,@in_1);\n<Text><60>(@in_0=Tom:String,@in_1=Jerry:String)");
        }

        [Test]
        public void TestUpdateBy0()
        {
            BoolTest.UpdateBy(p => p.Available, new { Name = "aa" });
            AssertSql(@"UPDATE [Bool_Test] SET [Name]=@Name_0  WHERE [Available] = @Available_1;
<Text><30>(@Name_0=aa:String,@Available_1=True:Boolean)");
        }

        [Test]
        public void TestUpdateBy1()
        {
            BoolTest.UpdateBy(p => p.Available, new { Name = "aa", Available = false});
            AssertSql(@"UPDATE [Bool_Test] SET [Name]=@Name_0,[Available]=@Available_1  WHERE [Available] = @Available_2;
<Text><30>(@Name_0=aa:String,@Available_1=False:Boolean,@Available_2=True:Boolean)");
        }

        [Test]
        public void TestUpdateBy2()
        {
            DbEntry.UpdateBy<TestClass>(p => p.Name == "abc", new { Age = 3L });
            AssertSql(@"UPDATE [Test_Class] SET [Age]=@Age_0  WHERE [Name] = @Name_1;
<Text><30>(@Age_0=3:Int64,@Name_1=abc:String)");
        }

        [Test]
        public void TestUpdateBy3()
        {
            DbEntry.UpdateBy<TestClass>(p => p.Name == "abc", new { Age = 3L, Gender = true });
            AssertSql(@"UPDATE [Test_Class] SET [Age]=@Age_0,[Gender]=@Gender_1  WHERE [Name] = @Name_2;
<Text><30>(@Age_0=3:Int64,@Gender_1=True:Boolean,@Name_2=abc:String)");
        }

        [Test]
        public void TestUpdateBy4()
        {
            DbEntry.UpdateBy<TestClass>(CK.K["Name"] == "abc", new { Age = 3L, Gender = true });
            AssertSql(@"UPDATE [Test_Class] SET [Age]=@Age_0,[Gender]=@Gender_1  WHERE [Name] = @Name_2;
<Text><30>(@Age_0=3:Int64,@Gender_1=True:Boolean,@Name_2=abc:String)");
        }

        [Test]
        public void TestDeleteBy0()
        {
            DbEntry.DeleteBy<TestClass>(CK.K["Name"] == "abc");
            AssertSql(@"DELETE FROM [Test_Class] WHERE [Name] = @Name_0;
<Text><30>(@Name_0=abc:String)");
        }

        [Test]
        public void TestDeleteBy1()
        {
            DbEntry.DeleteBy<TestClass>(p => p.Name == "abc");
            AssertSql(@"DELETE FROM [Test_Class] WHERE [Name] = @Name_0;
<Text><30>(@Name_0=abc:String)");
        }

        [Test]
        public void TestUpdate1()
        {
            DbEntry.From<TestClass>().Where(p => p.Name == "abc")
                .Add("Age", 2).Update();
            AssertSql(@"UPDATE [Test_Class] SET [Age]=[Age]+(2)  WHERE [Name] = @Name_0;
<Text><30>(@Name_0=abc:String)");
        }

        [Test]
        public void TestUpdate1L()
        {
            DbEntry.From<TestClass>().Where(p => p.Name == "abc")
                .Add(p => p.Age, 2).Update();
            AssertSql(@"UPDATE [Test_Class] SET [Age]=[Age]+(2)  WHERE [Name] = @Name_0;
<Text><30>(@Name_0=abc:String)");
        }

        [Test]
        public void TestUpdate2()
        {
            DbEntry.From<TestClass>().Where(p => p.Name == "abc")
                .Sub("Age", 4).Update();
            AssertSql(@"UPDATE [Test_Class] SET [Age]=[Age]-(4)  WHERE [Name] = @Name_0;
<Text><30>(@Name_0=abc:String)");
        }

        [Test]
        public void TestUpdate2L()
        {
            DbEntry.From<TestClass>().Where(p => p.Name == "abc")
                .Sub(p => p.Age, 4).Update();
            AssertSql(@"UPDATE [Test_Class] SET [Age]=[Age]-(4)  WHERE [Name] = @Name_0;
<Text><30>(@Name_0=abc:String)");
        }

        [Test]
        public void TestUpdate3()
        {
            DbEntry.From<TestClass>().Where(p => p.Name == "abc")
                .Set("Gender", true).Update();
            AssertSql(@"UPDATE [Test_Class] SET [Gender]=@Gender_0  WHERE [Name] = @Name_1;
<Text><30>(@Gender_0=True:Boolean,@Name_1=abc:String)");
        }

        [Test]
        public void TestUpdate3L()
        {
            DbEntry.From<TestClass>().Where(p => p.Name == "abc")
                .Set(p => p.Gender, true).Update();
            AssertSql(@"UPDATE [Test_Class] SET [Gender]=@Gender_0  WHERE [Name] = @Name_1;
<Text><30>(@Gender_0=True:Boolean,@Name_1=abc:String)");
        }

        [Test]
        public void TestUpdate4()
        {
            DbEntry.From<TestClass>().Where(p => p.Name == "abc")
                .Set("Gender", true).Add("Age", 15).Update();
            AssertSql(@"UPDATE [Test_Class] SET [Gender]=@Gender_0,[Age]=[Age]+(15)  WHERE [Name] = @Name_1;
<Text><30>(@Gender_0=True:Boolean,@Name_1=abc:String)");
        }

        [Test]
        public void TestUpdate4L()
        {
            DbEntry.From<TestClass>().Where(p => p.Name == "abc")
                .Set(p => p.Gender, true).Add("Age", 15).Update();
            AssertSql(@"UPDATE [Test_Class] SET [Gender]=@Gender_0,[Age]=[Age]+(15)  WHERE [Name] = @Name_1;
<Text><30>(@Gender_0=True:Boolean,@Name_1=abc:String)");
        }

        [Test]
        public void TestDelete1()
        {
            DbEntry.From<TestClass>().Where(p => p.Name == "abc").Delete();
            AssertSql(@"DELETE FROM [Test_Class] WHERE [Name] = @Name_0;
<Text><30>(@Name_0=abc:String)");
        }
    }
}
