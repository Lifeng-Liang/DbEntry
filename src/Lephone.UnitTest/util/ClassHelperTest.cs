using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Lephone.Data.Definition;
using Lephone.Util;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
    [TestFixture]
    public class ClassHelperTest
    {
        private enum CallType
        {
            None,
            Public,
            Private,
            Static
        }
        private static CallType ct = CallType.None;
        private static string str = "NULL";
        private int num = 0;
        private byte bn = 0;

        public void TestPublic(byte n)
        {
            ct = CallType.Public;
            bn = n;
        }

        private void TestPrivate(int n)
        {
            ct = CallType.Private;
            num = n;
        }

        public static void TestStatic(string s)
        {
            ct = CallType.Static;
            str = s;
        }

        [Test]
        public void TestIt()
        {
            var o = new ClassHelperTest();

            ClassHelper.CallFunction(o, "TestPublic", (byte)18);
            Assert.AreEqual(CallType.Public, ct);
            Assert.AreEqual(18, o.bn);

            ClassHelper.CallFunction(o, "TestPrivate", 23);
            Assert.AreEqual(CallType.Private, ct);
            Assert.AreEqual(23, o.num);

            ClassHelper.CallFunction(o, "TestStatic", "OK");
            Assert.AreEqual(CallType.Static, ct);
            Assert.AreEqual("OK", str);
        }

        [Test]
        public void TestChangeType()
        {
            const string v = "7:30:30";
            object iv = ClassHelper.ChangeType(v, typeof(Time));
            Assert.AreEqual("07:30:30", iv.ToString());
        }

        [Test]
        public void TestIsChildrenOf()
        {
            var ti = typeof(IDbConnection);
            var tb = typeof(DbConnection);
            var to = typeof(SqlConnection);

            Assert.IsTrue(ClassHelper.IsChildrenOf(tb, to));
            Assert.IsTrue(ClassHelper.IsChildrenOf(ti, to));
            Assert.IsTrue(ClassHelper.IsChildrenOf(ti, tb));

            Assert.IsFalse(ClassHelper.IsChildrenOf(to, tb));
            Assert.IsFalse(ClassHelper.IsChildrenOf(tb, ti));
            Assert.IsFalse(ClassHelper.IsChildrenOf(to, ti));
        }

        [Test]
        public void TestIsChildrenOfForInterfaces()
        {
            var to = typeof (IBelongsTo);
            var ti = typeof (ILazyLoading);
            Assert.IsTrue(to.IsChildrenOf(ti));
            Assert.IsFalse(ti.IsChildrenOf(to));
        }

        [Test]
        public void TestIsChildrenOf2()
        {
            var ti = typeof(IDbConnection);
            var tb = typeof(DbConnection);
            var to = typeof(SqlConnection);

            Assert.IsTrue(to.IsChildrenOf(tb));
            Assert.IsTrue(to.IsChildrenOf(ti));
            Assert.IsTrue(tb.IsChildrenOf(ti));

            Assert.IsFalse(tb.IsChildrenOf(to));
            Assert.IsFalse(ti.IsChildrenOf(tb));
            Assert.IsFalse(ti.IsChildrenOf(to));
        }
    }
}
