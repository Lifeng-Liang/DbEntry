
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Linq;

using NUnit.Framework;
using Lephone.MockSql.Recorder;

namespace Lephone.UnitTest.Linq
{
    [TestFixture]
    public class CommonTest
    {
        public int nnn = 30;

        public abstract class Person : LinqObjectModel<Person>
        {
            [DbColumn("Name")]
            public abstract string FirstName { get; set; }
        }

        private DbContext de = new DbContext("SQLite");

        [SetUp]
        public void SetUp()
        {
            StaticRecorder.ClearMessages();
        }

        [Test]
        public void Test1()
        {
            var list = de.From<Person>()
                .Where(p => p.FirstName.StartsWith("T") && (p.Id >= 1 || p.Id == 15) && p.FirstName != null && p.FirstName == p.FirstName)
                .OrderBy(p => p.Id)
                .Select();

            Assert.AreEqual("Select [Id],[Name] From [Person] Where ((([Name] Like @Name_0) And (([Id] >= @Id_1) Or ([Id] = @Id_2))) And ([Name] Is Not NULL)) And ([Name] = [Name]) Order By [Id] ASC;\n<Text><60>(@Name_0=T%:String,@Id_1=1:Int64,@Id_2=15:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test2()
        {
            var list = de.From<Person>()
                .Where(p => p.FirstName.EndsWith("T")).Select();
            Assert.AreEqual("Select [Id],[Name] From [Person] Where [Name] Like @Name_0;\n<Text><60>(@Name_0=%T:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test3()
        {
            var list = de.From<Person>()
                .Where(p => p.FirstName.Contains("T")).Select();
            Assert.AreEqual("Select [Id],[Name] From [Person] Where [Name] Like @Name_0;\n<Text><60>(@Name_0=%T%:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test4()
        {
            var item = de.GetObject<Person>(p => p.FirstName == "Tom");
            Assert.AreEqual("Select [Id],[Name] From [Person] Where [Name] = @Name_0;\n<Text><60>(@Name_0=Tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test5()
        {
            de.From<Person>().Where(WhereCondition.EmptyCondition).OrderBy(p => p.FirstName).ThenBy(p => p.Id).Select();
            Assert.AreEqual("Select [Id],[Name] From [Person] Order By [Name] ASC,[Id] ASC;\n<Text><60>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test6()
        {
            de.From<Person>().Where(WhereCondition.EmptyCondition).OrderByDescending(p => p.FirstName).ThenBy(p => p.Id).Select();
            Assert.AreEqual("Select [Id],[Name] From [Person] Order By [Name] DESC,[Id] ASC;\n<Text><60>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test7()
        {
            int id1 = 1;
            int id2 = 15;

            var list = de.From<Person>()
                .Where(p => p.FirstName.StartsWith("T") && (p.Id >= id1 || p.Id == id2) && p.FirstName != null && p.FirstName == p.FirstName)
                .OrderBy(p => p.Id)
                .Select();

            Assert.AreEqual("Select [Id],[Name] From [Person] Where ((([Name] Like @Name_0) And (([Id] >= @Id_1) Or ([Id] = @Id_2))) And ([Name] Is Not NULL)) And ([Name] = [Name]) Order By [Id] ASC;\n<Text><60>(@Name_0=T%:String,@Id_1=1:Int64,@Id_2=15:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test8()
        {
            TestMember(1, 15);
            Assert.AreEqual("Select [Id],[Name] From [Person] Where ((([Name] Like @Name_0) And (([Id] >= @Id_1) Or ([Id] = @Id_2))) And ([Name] Is Not NULL)) And ([Name] = [Name]) Order By [Id] ASC;\n<Text><60>(@Name_0=T%:String,@Id_1=1:Int64,@Id_2=15:Int64)", StaticRecorder.LastMessage);
        }

        private void TestMember(int id1, int id2)
        {
            var list = de.From<Person>()
                .Where(p => p.FirstName.StartsWith("T") && (p.Id >= id1 || p.Id == id2) && p.FirstName != null && p.FirstName == p.FirstName)
                .OrderBy(p => p.Id)
                .Select();
        }

        [Test]
        public void Test9()
        {
            TestMember2(1, 15);
            Assert.AreEqual("Select [Id],[Name] From [Person] Where [Id] >= @Id_0 Order By [Id] ASC;\n<Text><60>(@Id_0=3:Int64)", StaticRecorder.LastMessage);
        }

        private void TestMember2(int id1, int id2)
        {
            var list = de.From<Person>()
                .Where(p => p.Id >= id1 + 2)
                .OrderBy(p => p.Id)
                .Select();
        }

        [Test]
        public void Test10()
        {
            TestMember3(de, 3, 15);
            Assert.AreEqual("Select [Id],[Name] From [Person] Where [Id] >= @Id_0 Order By [Id] ASC;\n<Text><60>(@Id_0=3:Int64)", StaticRecorder.LastMessage);
        }

        private static void TestMember3(DbContext de, int id1, int id2)
        {
            var list = de.From<Person>()
                .Where(p => p.Id >= id1)
                .OrderBy(p => p.Id)
                .Select();
        }

        [Test]
        public void Test11()
        {
            TestMember4(de, this, 3);
            Assert.AreEqual("Select [Id],[Name] From [Person] Where [Id] >= @Id_0 Order By [Id] ASC;\n<Text><60>(@Id_0=27:Int64)", StaticRecorder.LastMessage);
        }

        private static void TestMember4(DbContext de, CommonTest tt, int id1)
        {
            var list = de.From<Person>()
                .Where(p => p.Id >= tt.nnn - id1)
                .OrderBy(p => p.Id)
                .Select();
        }

        [Test]
        public void Test12()
        {
            string name1 = "tom";

            var list = de.From<Person>()
                .Where(p => p.FirstName.StartsWith(name1))
                .OrderBy(p => p.Id)
                .Select();

            Assert.AreEqual("Select [Id],[Name] From [Person] Where [Name] Like @Name_0 Order By [Id] ASC;\n<Text><60>(@Name_0=tom%:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test13()
        {
            string name1 = "tom";

            var list = de.From<Person>()
                .Where(p => p.FirstName != name1)
                .OrderBy(p => p.Id)
                .Select();

            Assert.AreEqual("Select [Id],[Name] From [Person] Where [Name] <> @Name_0 Order By [Id] ASC;\n<Text><60>(@Name_0=tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test14()
        {
            string name1 = "tom";

            var list = de.From<Person>()
                .Where(p => p.FirstName != name1 + " cat")
                .OrderBy(p => p.Id)
                .Select();

            Assert.AreEqual("Select [Id],[Name] From [Person] Where [Name] <> @Name_0 Order By [Id] ASC;\n<Text><60>(@Name_0=tom cat:String)", StaticRecorder.LastMessage);
        }
    }
}
