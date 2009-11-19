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
        public abstract class User1 : DbObjectModel<User1>
        {
            public abstract string Name { get; set; }
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

        public class FakeDynamicObjectBuilder : DynamicObjectBuilder
        {
            public static new FakeDynamicObjectBuilder Instance = new FakeDynamicObjectBuilder();

            public static Exception Exception;

            protected override Type GenerateType(Type sourceType)
            {
                Type t = base.GenerateType(sourceType);
                Thread.Sleep(1000);
                return t;
            }
        }

        public abstract class User2 : DbObjectModel<User2>
        {
            public abstract string Name { get; set; }
        }

        [Test]
        public void Test2()
        {
            var t1 = new Thread(DoWork2);
            var t2 = new Thread(DoWork2);

            t1.Start();
            t2.Start();

            Thread.Sleep(2000);

            if(FakeDynamicObjectBuilder.Exception != null)
            {
                throw FakeDynamicObjectBuilder.Exception;
            }
        }

        public static void DoWork2()
        {
            try
            {
                FakeDynamicObjectBuilder.Instance.GetImplType(typeof(User2));
            }
            catch (Exception ex)
            {
                FakeDynamicObjectBuilder.Exception = ex;
            }
        }
    }
}
