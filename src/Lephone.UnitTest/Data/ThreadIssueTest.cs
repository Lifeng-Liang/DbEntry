using System;
using System.Threading;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class ThreadIssueTest
    {
        public class User1 : DbObjectModel<User1>
        {
            public string Name { get; set; }
        }

        public static Exception Exception;

        [Test]
        public void Test1()
        {
            var t1 = new Thread(DoWork1);
            var t2 = new Thread(DoWork1);

            t1.Start();
            t2.Start();

            Thread.Sleep(2000);

            if(Exception != null)
            {
                throw Exception;
            }
        }

        public static void DoWork1()
        {
            try
            {
                ObjectInfo.GetInstance(typeof(User1));
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
        }

        //=============================================================

        public class FakeDynamicAssemblyHandler : AssemblyHandler.DynamicAssemblyHandler
        {
            public static new FakeDynamicAssemblyHandler Instance = new FakeDynamicAssemblyHandler();

            public static Exception Exception;

            public override Type GetImplementedType(Type sourceType)
            {
                Type t = base.GetImplementedType(sourceType);
                Thread.Sleep(1000);
                return t;
            }
        }

        public class User2 : DbObjectModel<User2>
        {
            public string Name { get; set; }
        }

        [Test]
        public void Test2()
        {
            var t1 = new Thread(DoWork2);
            var t2 = new Thread(DoWork2);

            t1.Start();
            t2.Start();

            Thread.Sleep(2000);

            if (FakeDynamicAssemblyHandler.Exception != null)
            {
                throw FakeDynamicAssemblyHandler.Exception;
            }
        }

        public static void DoWork2()
        {
            try
            {
                FakeDynamicAssemblyHandler.Instance.GetImplType(typeof(User2));
            }
            catch (Exception ex)
            {
                FakeDynamicAssemblyHandler.Exception = ex;
            }
        }
    }
}
