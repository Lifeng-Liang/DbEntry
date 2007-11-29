
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
        public abstract class Person : DbObjectModel<Person>
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
    }
}
