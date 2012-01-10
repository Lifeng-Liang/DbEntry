using Leafing.Data;
using Leafing.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [TestFixture]
    public class PropertyClassTest : DataTestBase
    {
        [Test]
        public void TestPropertyClassImpl()
        {
            var o = DbEntry.GetObject<PropertyClassImpl>(1);
            Assert.IsNotNull(o);
            Assert.AreEqual("Tom", o.Name);
        }

        [Test]
        public void TestPropertyClassBase()
        {
            var o = DbEntry.GetObject<PropertyClassBase>(1);
            Assert.IsNotNull(o);
            Assert.AreEqual("Tom", o.Name);
        }

        [Test]
        public void TestPropertyClassBaseCrud()
        {
            var o = new PropertyClassBase {Name = "OK"};
            // create
            DbEntry.Save(o);
            // read
            var o1 = DbEntry.GetObject<PropertyClassBase>(o.Id);
            Assert.AreEqual("OK", o1.Name);
            // update
            o1.Name = "CANCEL";
            DbEntry.Save(o1);
            Assert.AreEqual("CANCEL", DbEntry.GetObject<PropertyClassBase>(o.Id).Name);
            // delete
            DbEntry.Delete(o);
            Assert.IsNull(DbEntry.GetObject<PropertyClassBase>(o1.Id));
        }

        [Test]
        public void TestPropertyClassWithDbColumn()
        {
            var o = new PropertyClassWithDbColumn { TheName = "OK" };
            // create
            DbEntry.Save(o);
            // read
            var o1 = DbEntry.GetObject<PropertyClassWithDbColumn>(o.Id);
            Assert.AreEqual("OK", o1.TheName);
            // update
            o1.TheName = "CANCEL";
            DbEntry.Save(o1);
            Assert.AreEqual("CANCEL", DbEntry.GetObject<PropertyClassWithDbColumn>(o.Id).TheName);
            // delete
            DbEntry.Delete(o);
            Assert.IsNull(DbEntry.GetObject<PropertyClassWithDbColumn>(o1.Id));
        }
    }
}
