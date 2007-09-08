
#region usings

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using NUnit.Framework;

using Lephone.Util.Text;
using Lephone.Util.Setting;

#endregion

namespace Lephone.UnitTest.util
{
    [TestFixture]
	public class ConfigTest1
	{
        private static ConfigTest1 ct = new ConfigTest1();

        public static readonly int TestInt = 0;

        [ShowString("TheString")]
        public static readonly string TestString = "";

        private readonly bool TestBool = false;


        [SetUp]
        public void SetUp()
        {
            ConfigHelper ch = new ConfigHelper("Lephone.Settings");
            ch.InitClass(ct);
        }

        [Test]
        public void Test1()
        {
            Assert.AreEqual(10, TestInt);
            Assert.AreEqual("OK", TestString);
            Assert.AreEqual(true, ct.TestBool);
        }

        [Test]
        public void Test2()
        {
            Assert.AreEqual(23, ConfigTest2.TestInt);
            Assert.AreEqual(ConfigEnum.Release, ConfigTest2.TestEnum);
        }
	}

    public enum ConfigEnum
    {
        Debug,
        Release
    }

    public static class ConfigTest2
    {
        public static readonly int TestInt = 1;

        [ShowString("CompMode")]
        public static readonly ConfigEnum TestEnum = ConfigEnum.Debug;

        static ConfigTest2()
        {
            ConfigHelper ch = new ConfigHelper("MySetting");
            ch.InitClass(typeof(ConfigTest2));
        }
    }
}
