using Lephone.Data;
using Lephone.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class PropertyClassTest
    {
        #region Init

        [SetUp]
        public void SetUp()
        {
            InitHelper.Init();
        }

        [TearDown]
        public void TearDown()
        {
            InitHelper.Clear();
        }

        #endregion

        [Test]
        public void TestPropertyClassImpl()
        {
            PropertyClassImpl o = DbEntry.GetObject<PropertyClassImpl>(1);
            Assert.IsNotNull(o);
            Assert.AreEqual("Tom", o.Name);
        }

        [Test]
        public void TestPropertyClassBase()
        {
            PropertyClassBase o = DbEntry.GetObject<PropertyClassBase>(1);
            Assert.IsNotNull(o);
            Assert.AreEqual("Tom", o.Name);
        }

        [Test]
        public void TestPropertyClassBaseCRUD()
        {
            PropertyClassBase o = DynamicObject.NewObject<PropertyClassBase>("OK");
            // create
            DbEntry.Save(o);
            // read
            PropertyClassBase o1 = DbEntry.GetObject<PropertyClassBase>(o.Id);
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
            PropertyClassWithDbColumn o = DynamicObject.NewObject<PropertyClassWithDbColumn>("OK");
            // create
            DbEntry.Save(o);
            // read
            PropertyClassWithDbColumn o1 = DbEntry.GetObject<PropertyClassWithDbColumn>(o.Id);
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
