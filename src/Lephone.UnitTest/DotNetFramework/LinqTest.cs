using System;
using System.Linq.Expressions;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.DotNetFramework
{
    [TestFixture]
    public class LinqTest
    {
        [DbTable("Books")]
        public class Book : DbObjectModel<Book>
        {
            public string Name { get; set; }
            public int Category_Id { get; set; }
        }

        [Test]
        public void Test1()
        {
            Select<Book>(p => new {p.Name, Age = p.Category_Id});
        }

        private static void Select<T>(Expression<Func<T, object>> expr)
        {
            Assert.AreEqual(ExpressionType.Lambda, expr.NodeType);

            var newExpr = (NewExpression)expr.Body;
            dynamic o = newExpr.Constructor.Invoke(new object[] {"tom", 123});
            Assert.AreEqual("tom", o.Name);
            Assert.AreEqual(123, o.Age);
            Console.WriteLine("{0} - {1}", o.Name, o.Age);
            var p1 = (MemberExpression)newExpr.Arguments[0];
            Assert.AreEqual("Name", p1.Member.Name);
            var p2 = (MemberExpression)newExpr.Arguments[1];
            Assert.AreEqual("Category_Id", p2.Member.Name);
            Console.WriteLine("{0} - {1}", p1.Member.Name, p2.Member.Name);
        }
    }
}
