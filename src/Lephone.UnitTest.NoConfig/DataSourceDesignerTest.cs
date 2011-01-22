using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Web;
using Lephone.Web.Common;
using NUnit.Framework;

namespace Lephone.UnitTest.NoConfig
{
    public class User : DbObjectModel<User>
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class UserDataSource : DbEntryDataSource<User>
    {
    }

    [TestFixture]
    public class DataSourceDesignerTest
    {
        [Test, Ignore("It should test in design mode to find out if it needed.")]
        public void Test1()
        {
            var ctx = ModelContext.GetInstance(typeof(User));
            Assert.IsNotNull(ctx);

            var o = ctx.NewObject();
            Assert.IsNotNull(o);
        }

        [Test]
        public void Test2()
        {
            var source = new UserDataSource();
            var d = new DbEntryDataSourceDesigner();
            d.Initialize(source);
            var vs = d.GetViewNames();
            Assert.AreEqual(1, vs.Length);
            var v = d.GetView(vs[0]);
            Assert.IsNotNull(v);
            bool b;
            var dd = v.GetDesignTimeData(10, out b);
            Assert.IsTrue(b);
            Assert.IsNotNull(dd);
            int i = 0;
            foreach(User user in dd)
            {
                Assert.AreEqual("Sample Data " + i, user.Name);
                Assert.AreEqual(i + 1, user.Age);
                i++;
            }

            var s = v.Schema;
            Assert.IsNotNull(s);
            Assert.AreEqual("DbEntry_User", s.Name);
            var fs = s.GetFields();
            Assert.IsNotNull(fs);
            Assert.AreEqual(3, fs.Length);
            Assert.AreEqual("Id", fs[0].Name);
            Assert.AreEqual("Name", fs[1].Name);
            Assert.AreEqual("Age", fs[2].Name);
        }
    }
}
