using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [DbTable(typeof(BaoXiuRS))]
    public class PoModel : DbObjectModel<PoModel>
    {
        [AllowNull, Length(50)]
        public string UserName { get; set; }
    }

    [TestFixture]
    public class PartOfClassTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            var o = new BaoXiuRS {UserName = "tom"};
            o.Save();
            var p = PoModel.FindById(o.Id);
            Assert.AreEqual("tom", p.UserName);
            p.UserName = "jerry";
            p.Save();
            p = PoModel.FindById(o.Id);
            Assert.AreEqual("jerry", p.UserName);
        }

        [Test]
        public void Test2()
        {
            var o = new PoModel {UserName = "abc"};
            o.Save();
            o = PoModel.FindById(o.Id);
            Assert.AreEqual("abc", o.UserName);
        }
    }
}
