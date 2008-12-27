using System.Linq;
using Lephone.Data.Definition;
using Lephone.Linq;
using Lephone.MockSql.Recorder;
using NUnit.Framework;

namespace Lephone.UnitTest.Linq
{
    [TestFixture]
    public class RealOperateTest
    {
        [DbTable("People")]
        public abstract class Person : LinqObjectModel<Person>
        {
            [DbColumn("Name")]
            public abstract string FirstName { get; set; }
        }

        #region Init

        [SetUp]
        public void SetUp()
        {
            InitHelper.Init();
            StaticRecorder.ClearMessages();
        }

        [TearDown]
        public void TearDown()
        {
            InitHelper.Clear();
        }

        #endregion

        [Test]
        public void Test15()
        {
            var list = from s in Person.Table select s;
            Assert.AreEqual(3, list.ToArray().Length);
        }

        [Test]
        public void Test16()
        {
            var list = from s in Person.Table where s.Id == 1 select s;
            Assert.AreEqual(1, list.ToArray().Length);
            Assert.AreEqual("Tom", list.ToArray()[0].FirstName);
        }

        [Test]
        public void Test3()
        {
            var list = Person.Find(p => p.FirstName.Contains("T"));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Tom", list[0].FirstName);
        }
    }
}
