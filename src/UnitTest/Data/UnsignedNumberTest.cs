
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Lephone.Data;
using Lephone.Data.Definition;

namespace Lephone.UnitTest.Data
{
    [DbTable("PCs")]
    public abstract class UnsignedPC : DbObjectModel<UnsignedPC>
    {
        public abstract string Name { get; set; }
        public abstract uint Person_Id { get; set; }
    }

    [DbTable("PCs")]
    public class UnsignedPCReal : IDbObject
    {
        [DbKey] public long Id;
        public string Name;
        public uint Person_Id;
    }

    [TestFixture]
    public class UnsignedNumberTest
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
        public void Test1()
        {
            UnsignedPC o = UnsignedPC.FindById(1);
            Assert.AreEqual("IBM", o.Name);
            Assert.AreEqual(2, o.Person_Id);
        }

        [Test]
        public void Test2()
        {
            UnsignedPCReal o = DbEntry.GetObject<UnsignedPCReal>(1);
            Assert.AreEqual("IBM", o.Name);
            Assert.AreEqual(2, o.Person_Id);
        }

        [Test]
        public void Test3()
        {
            List<UnsignedPC> ls = DbEntry.Context.ExecuteList<UnsignedPC>("select * from PCs where [Id] = 1");
            Assert.AreEqual(1, ls.Count);
            Assert.AreEqual("IBM", ls[0].Name);
            Assert.AreEqual(2, ls[0].Person_Id);
        }
    }
}
