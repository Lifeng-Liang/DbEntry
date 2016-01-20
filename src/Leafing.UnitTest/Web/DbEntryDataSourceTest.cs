using System.Collections.Generic;
using Leafing.UnitTest.Data.Objects;
using Leafing.Web;
using Leafing.Web.Common;
using NUnit.Framework;
using Leafing.Data.Definition;
using Leafing.Core.Text;

namespace Leafing.UnitTest.Web
{
    [TestFixture]
    public class DbEntryDataSourceTest : DataTestBase
    {
		[ShowString("Hello")]
		public class User : DbObjectModel<User>
		{
			public string Name { get; set; }
		}

		public class UserDataSource : DbEntryDataSource<User>
		{
			public string ShowString
			{
				get { return ModelShowName; }
			}
		}

        public class PersonDataSource : DbEntryDataSource<Person>
        {
			public string ShowString
			{
				get { return ModelShowName; }
			}
        }

		[Test]
		public void TestShowName()
		{
			var ds = new UserDataSource();
			Assert.AreEqual("Hello", ds.ShowString);

			var ds1 = new PersonDataSource();
			Assert.AreEqual("Person", ds1.ShowString);
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
