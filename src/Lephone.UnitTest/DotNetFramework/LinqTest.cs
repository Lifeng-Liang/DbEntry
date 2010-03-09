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
        public abstract class Book : DbObjectModel<Book>
        {
            public abstract string Name { get; set; }
            public abstract int Category_Id { get; set; }
        }

        [Test]
        public void Test1()
        {
            Select<Book>(p => new { Name = p.Name, Age = p.Category_Id });
        }

        private void Select<T>(Expression<Func<T, object>> expr)
        {
            Assert.AreEqual(ExpressionType.Lambda, expr.NodeType);
            Console.WriteLine(expr);
            Console.WriteLine(expr.Type);
            Console.WriteLine();
            foreach (var parameterExpression in expr.Parameters)
            {
                OutputParamter(parameterExpression);
            }
            Console.WriteLine();
            Console.WriteLine(expr.Body);
            var ex = expr.Compile();
            Console.WriteLine();
            Console.WriteLine(ex.Method);
            Console.WriteLine(ex.Target);
        }

        private void OutputParamter(ParameterExpression expr)
        {
            Console.WriteLine(expr);
        }
    }
}
