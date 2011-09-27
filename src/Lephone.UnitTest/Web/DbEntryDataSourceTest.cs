using System.Collections.Generic;
using Lephone.UnitTest.Data.Objects;
using Lephone.Web;
using Lephone.Web.Common;
using NUnit.Framework;

namespace Lephone.UnitTest.Web
{
    [TestFixture]
    public class DbEntryDataSourceTest : DataTestBase
    {
        public class PersonDataSource : DbEntryDataSource<Person>
        {
        }

        [Test]
        public void Test1()
        {
            var p = Person.FindById(1);
            Assert.AreEqual("Tom", p.Name);

            var ds = (IExcuteableDataSource)new PersonDataSource();
            var keys = new Dictionary<string, object> {{"Id", 1}, {"Name", "123"}};
            var values = new Dictionary<string, object> {{"Name", ""}};
            var oldValues = new Dictionary<string, object> {{"Name", "123"}};
            ds.Update(keys, values, oldValues);

            p = Person.FindById(1);
            Assert.AreEqual("", p.Name);
        }

        [Test]
        public void Test2()
        {
            var p = Person.FindById(1);
            Assert.AreEqual("Tom", p.Name);

            var ds = (IExcuteableDataSource)new PersonDataSource();
            var keys = new Dictionary<string, object> { { "Id", 1 }, { "Name", "123" } };
            var values = new Dictionary<string, object> { { "Name", "aaa" } };
            var oldValues = new Dictionary<string, object> { { "Name", "123" } };
            ds.Update(keys, values, oldValues);

            p = Person.FindById(1);
            Assert.AreEqual("aaa", p.Name);
        }

        [Test]
        public void Test3()
        {
            var p = Person.FindById(1);
            Assert.AreEqual("Tom", p.Name);

            var ds = (IExcuteableDataSource)new PersonDataSource();
            var keys = new Dictionary<string, object> { { "Id", 1 }, { "Name", "123" } };
            var values = new Dictionary<string, object> { { "Name", null } };
            var oldValues = new Dictionary<string, object> { { "Name", "123" } };
            ds.Update(keys, values, oldValues);

            p = Person.FindById(1);
            Assert.AreEqual("", p.Name);
        }
    }
}
