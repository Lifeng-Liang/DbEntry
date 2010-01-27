using System;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using Lephone.UnitTest.Data;
using NUnit.Framework;

namespace Lephone.UnitTest.Linq
{
    [TestFixture]
    public class CommonTest : DataTestBase
    {
        public int nnn = 30;

        [DbTable("People")]
        public abstract class Person : DbObjectModel<Person>
        {
            [DbColumn("Name")]
            public abstract string FirstName { get; set; }
        }

        public enum MyEnum
        {
            Test1,
            Test2,
        }

        public abstract class EnumTest : DbObjectModel<EnumTest>
        {
            [DbColumn("ccc")]
            public abstract MyEnum Abc { get; set; }
        }

        [Test]
        public void Test1()
        {
            sqlite.From<Person>()
                .Where(p => p.FirstName.StartsWith("T") && (p.Id >= 1 || p.Id == 15) && p.FirstName != null && p.FirstName == p.FirstName)
                .OrderBy(p => p.Id)
                .Select();

            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE ((([Name] LIKE @Name_0) AND (([Id] >= @Id_1) OR ([Id] = @Id_2))) AND ([Name] IS NOT NULL)) AND ([Name] = [Name]) ORDER BY [Id] ASC;\n<Text><60>(@Name_0=T%:String,@Id_1=1:Int64,@Id_2=15:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test2()
        {
            sqlite.From<Person>()
                .Where(p => p.FirstName.EndsWith("T")).Select();
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Name] LIKE @Name_0;\n<Text><60>(@Name_0=%T:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test3()
        {
            sqlite.From<Person>()
                .Where(p => p.FirstName.Contains("T")).Select();
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Name] LIKE @Name_0;\n<Text><60>(@Name_0=%T%:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test4()
        {
            sqlite.GetObject<Person>(p => p.FirstName == "Tom");
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Name] = @Name_0;\n<Text><60>(@Name_0=Tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test5()
        {
            sqlite.From<Person>().Where(Condition.Empty).OrderBy(p => p.FirstName).ThenBy(p => p.Id).Select();
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] ORDER BY [Name] ASC,[Id] ASC;\n<Text><60>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test6()
        {
            sqlite.From<Person>().Where(Condition.Empty).OrderByDescending(p => p.FirstName).ThenBy(p => p.Id).Select();
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] ORDER BY [Name] DESC,[Id] ASC;\n<Text><60>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test7()
        {
            const int id1 = 1;
            const int id2 = 15;

            sqlite.From<Person>()
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
            sqlite.From<Person>()
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
            sqlite.From<Person>()
                .Where(p => p.Id >= id1 + 2)
                .OrderBy(p => p.Id)
                .Select();
        }

        [Test]
        public void Test10()
        {
            TestMember3(sqlite, 3, 15);
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Id] >= @Id_0 ORDER BY [Id] ASC;\n<Text><60>(@Id_0=3:Int64)", StaticRecorder.LastMessage);
        }

        private static void TestMember3(DbContext de, int id1, int id2)
        {
            de.From<Person>()
                .Where(p => p.Id >= id1)
                .OrderBy(p => p.Id)
                .Select();
        }

        [Test]
        public void Test11()
        {
            TestMember4(sqlite, this, 3);
            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Id] >= @Id_0 ORDER BY [Id] ASC;\n<Text><60>(@Id_0=27:Int64)", StaticRecorder.LastMessage);
        }

        private static void TestMember4(DbContext de, CommonTest tt, int id1)
        {
            de.From<Person>()
                .Where(p => p.Id >= tt.nnn - id1)
                .OrderBy(p => p.Id)
                .Select();
        }

        [Test]
        public void Test12()
        {
            const string name1 = "tom";

            sqlite.From<Person>()
                .Where(p => p.FirstName.StartsWith(name1))
                .OrderBy(p => p.Id)
                .Select();

            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Name] LIKE @Name_0 ORDER BY [Id] ASC;\n<Text><60>(@Name_0=tom%:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test13()
        {
            const string name1 = "tom";

            sqlite.From<Person>()
                .Where(p => p.FirstName != name1)
                .OrderBy(p => p.Id)
                .Select();

            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Name] <> @Name_0 ORDER BY [Id] ASC;\n<Text><60>(@Name_0=tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test14()
        {
            const string name1 = "tom";

            sqlite.From<Person>()
                .Where(p => p.FirstName != name1 + " cat")
                .OrderBy(p => p.Id)
                .Select();

            Assert.AreEqual("SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE [Name] <> @Name_0 ORDER BY [Id] ASC;\n<Text><60>(@Name_0=tom cat:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test15()
        {
            string[] ss = null;
            string s = ss.First();
            Assert.IsNull(s);

            s = ss.Last();
            Assert.IsNull(s);

            ss = new[]{"1st", "2nd", "3rd"};
            Assert.AreEqual("1st", ss.First());
            Assert.AreEqual("3rd", ss.Last());

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
            string s = list.Last();
            Assert.IsNull(s);

            s = list.First();
            Assert.IsNull(s);

            list = new List<string> {"1st", "2nd", "3rd"};
            Assert.AreEqual("1st", list.First());
            Assert.AreEqual("3rd", list.Last());

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
            sqlite.From<Person>().Where(p => p.FirstName.ToLower() == "tom").Select();
            AssertSql(@"SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE LOWER([Name]) = @Name_0;
<Text><60>(@Name_0=tom:String)");
        }

        [Test]
        public void TestLowerForLike()
        {
            sqlite.From<Person>().Where(p => p.FirstName.ToLower().Contains("tom")).Select();
            AssertSql(@"SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE LOWER([Name]) LIKE @Name_0;
<Text><60>(@Name_0=%tom%:String)");
        }

        [Test]
        public void TestUpperFunction()
        {
            sqlite.From<Person>().Where(p => p.FirstName.ToUpper() == "tom").Select();
            AssertSql(@"SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE UPPER([Name]) = @Name_0;
<Text><60>(@Name_0=tom:String)");
        }

        [Test]
        public void TestUpperForLike()
        {
            sqlite.From<Person>().Where(p => p.FirstName.ToUpper().Contains("tom")).Select();
            AssertSql(@"SELECT [Id],[Name] AS [FirstName] FROM [People] WHERE UPPER([Name]) LIKE @Name_0;
<Text><60>(@Name_0=%tom%:String)");
        }

        [Test]
        public void TestMax()
        {
            var n = DbEntry.From<Person>().Where(Condition.Empty).GetMax(p => p.Id);
            Assert.AreEqual(3, n);

            n = Person.GetMax(null, p => p.Id);
            Assert.AreEqual(3, n);

            var x = Person.GetMax(p => p.Id > 100, p => p.Id);
            Assert.IsNull(x);
        }

        [Test]
        public void TestMin()
        {
            var n = DbEntry.From<Person>().Where(Condition.Empty).GetMin(p => p.Id);
            Assert.AreEqual(1, n);

            n = Person.GetMin(null, p => p.Id);
            Assert.AreEqual(1, n);

            var x = Person.GetMin(p => p.Id > 100, p => p.Id);
            Assert.IsNull(x);
        }

        [Test]
        public void TestMaxDate()
        {
            var n = DbEntry.From<DateAndTime>().Where(Condition.Empty).GetMaxDate(p => p.dtValue);
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), n);

            n = DateAndTime.GetMaxDate(null, p => p.dtValue);
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), n);

            var x = DateAndTime.GetMaxDate(p => p.Id > 100, p => p.dtValue);
            Assert.IsNull(x);
        }

        [Test]
        public void TestMinDate()
        {
            var n = DbEntry.From<DateAndTime>().Where(Condition.Empty).GetMinDate(p => p.dtValue);
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), n);

            n = DateAndTime.GetMinDate(null, p => p.dtValue);
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), n);

            var x = DateAndTime.GetMinDate(p => p.Id > 100, p => p.dtValue);
            Assert.IsNull(x);
        }

        [Test]
        public void TestSum()
        {
            var n = DbEntry.From<Person>().Where(Condition.Empty).GetSum(p => p.Id);
            Assert.AreEqual(6, n);

            n = Person.GetSum(null, p => p.Id);
            Assert.AreEqual(6, n);

            var x = Person.GetSum(p => p.Id > 100, p => p.Id);
            Assert.IsNull(x);
        }

        [Test]
        public void TestDelete()
        {
            sqlite.Delete<Person>(p => p.FirstName.EndsWith("T"));
            Assert.AreEqual("DELETE FROM [People] WHERE [Name] LIKE @Name_0;\n<Text><30>(@Name_0=%T:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestEnum()
        {
            sqlite.From<EnumTest>().Where(p => p.Abc == MyEnum.Test1).Select();
            AssertSql("SELECT [Id],[ccc] AS [Abc] FROM [Enum_Test] WHERE [ccc] = @ccc_0;\n<Text><60>(@ccc_0=0:Int32)");
        }
    }
}
