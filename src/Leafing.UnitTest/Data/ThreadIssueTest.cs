using System;
using System.Threading;
using Leafing.Data;
using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Data {
    [TestFixture]
    public class ThreadIssueTest {
        public class User1 : DbObjectModel<User1> {
            public string Name { get; set; }
        }

        public static Exception Exception;

        [Test]
        public void Test1() {
            var t1 = new Thread(DoWork1);
            var t2 = new Thread(DoWork1);

            t1.Start();
            t2.Start();

            Thread.Sleep(2000);

            if (Exception != null) {
                throw Exception;
            }
        }

        public static void DoWork1() {
            try {
                ModelContext.GetInstance(typeof(User1));
            } catch (Exception ex) {
                Exception = ex;
            }
        }
    }
}