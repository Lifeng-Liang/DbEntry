using Leafing.Data;
using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Data {
    [DbTable("PCs")]
    public class UnsignedPC : DbObjectModel<UnsignedPC> {
        public string Name { get; set; }
        public ulong Person_Id { get; set; }
    }

    [DbTable("PCs")]
    public class UnsignedPCReal : IDbObject {
        [DbKey] public long Id;
        public string Name;
        public uint Person_Id;
    }

    [TestFixture]
    public class UnsignedNumberTest : DataTestBase {
        [Test]
        public void Test1() {
            var o = UnsignedPC.FindById(1);
            Assert.AreEqual("IBM", o.Name);
            Assert.AreEqual(2, o.Person_Id);
        }

        [Test]
        public void Test2() {
            var o = DbEntry.GetObject<UnsignedPCReal>(1);
            Assert.AreEqual("IBM", o.Name);
            Assert.AreEqual(2, o.Person_Id);
        }

        [Test]
        public void Test3() {
            var ls = DbEntry.ExecuteList<UnsignedPC>("select * from PCs where [Id] = 1");
            Assert.AreEqual(1, ls.Count);
            Assert.AreEqual("IBM", ls[0].Name);
            Assert.AreEqual(2, ls[0].Person_Id);
        }
    }
}