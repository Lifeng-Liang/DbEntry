using Leafing.Core.Setting;
using Leafing.Core.Text;
using NUnit.Framework;

namespace Leafing.UnitTest.util
{
    [TestFixture]
	public class ConfigTest1
	{
        private static readonly ConfigTest1 ct = new ConfigTest1();

        public static readonly int TestInt = 0;

        [ShowString("TheString")]
        public static readonly string TestString = "";

        private readonly bool TestBool = false;

        [SetUp]
        public void SetUp()
        {
            ConfigHelper ch = new ConfigHelper("Leafing.Settings");
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

        [Test]
        public void TestCreateClass()
        {
            Assert.IsNotNull(ConfigTest3.TheClass);
            Assert.AreEqual("aaa", ConfigTest3.TheClass.Name);
        }

        [Test]
        public void TestCreateClass2()
        {
            Assert.IsNotNull(ConfigTest3.MyClass);
            Assert.AreEqual("aaa", ConfigTest3.MyClass.Name);
        }
	}

    public enum ConfigEnum
    {
        Debug,
        Release
    }

    public class ConfigTestClass
    {
        public string Name = "aaa";
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

    public static class ConfigTest3
    {
        public static readonly ConfigTestClass TheClass = null;
        public static readonly ConfigTestClass MyClass = null;

        static ConfigTest3()
        {
            new ConfigHelper("MySetting").InitClass(typeof(ConfigTest3));
        }
    }
}
