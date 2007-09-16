
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
    }
}
