
#region usings

using System;
using NUnit.Framework;
using Lephone.Util;

#endregion

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

        public void TestIt()
        {
            ClassHelperTest o = new ClassHelperTest();

            ClassHelper.CallFunction(o, "TestPublic", (byte)18);
            Assert.AreEqual(CallType.Public, ct);
            Assert.AreEqual((byte)18, o.bn);

            ClassHelper.CallFunction(o, "TestPrivate", 23);
            Assert.AreEqual(CallType.Private, ct);
            Assert.AreEqual(23, o.num);

            ClassHelper.CallFunction(o, "TestStatic", "OK");
            Assert.AreEqual(CallType.Static, ct);
            Assert.AreEqual("OK", str);
        }
    }
}
