
using System;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Data.Caching;
using Lephone.Data.Common;
using Lephone.Util;
using Lephone.UnitTest.util.timingTask;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class CacheTest
    {
        [Test]
        public void Test1()
        {
            MockNowTimeProvider now = (MockNowTimeProvider)NowProvider.Instance;
            now.SetNow(new DateTime(2007, 11, 4, 15, 23, 43));
            StaticHashCacheProvider c = ClassHelper.CreateInstance<StaticHashCacheProvider>();

            SinglePerson p = new SinglePerson();
            p.Id = 15;
            p.Name = "tom";

            string key = KeyGenerator.Instance.GetKey(p.GetType(), p.Id);
            c[key] = ObjectInfo.CloneObject(p);

            SinglePerson act = (SinglePerson)c[key];

            Assert.IsNotNull(act);
            Assert.AreEqual(15, act.Id);
            Assert.AreEqual("tom", act.Name);

            p.Name = "jerry";
            c[key] = ObjectInfo.CloneObject(p);

            act = (SinglePerson)c[key];
            p.Name = "mike"; // By using ObjectInfo.CloneObject, it doesn't change cached object.

            Assert.IsNotNull(act);
            Assert.AreEqual(15, act.Id);
            Assert.AreEqual("jerry", act.Name);

            now.SetNow(new DateTime(2007, 11, 4, 15, 34, 43));

            act = (SinglePerson)c[key];

            Assert.IsNull(act);
        }
    }
}
