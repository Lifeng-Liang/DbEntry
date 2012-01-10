using Leafing.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [TestFixture]
    public class RealLikeModelTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            var c = new Company {Name = "NotExist"};
            new User {Name = "tom", Type = UserType.Manager, Company = c, Valid = true};
            var d = new Department {Name = "Selling", Company = c};
            new Employee {Name = "jerry", Age = 24, Company = c, Department = d};
            new Resource {Name = "Printer", Count = 2, Company = c, Department = d};
            var p = new Product { Name = "Tale Story", Company = c };
            var t = new Trade { Name = "20110001", TotalPrice = 192m, Company = c };
            var o = new Order { Name = "Tale Story", Count = 10, Price = 19.2m, Description = "test", Trade = t, Product = p};

            o.Save();

            var o1 = Order.FindById(o.Id);
            Assert.AreEqual("Tale Story", o1.Name);
            Assert.AreEqual(10, o1.Count);

            Assert.AreEqual("20110001", o1.Trade.Name);
            Assert.AreEqual(192m, o1.Trade.TotalPrice);

            Assert.AreEqual("Tale Story", o1.Product.Name);

            Assert.IsNotNull(o1.Trade.Company);
            Assert.AreEqual("NotExist", o1.Trade.Company.Name);

            Assert.AreEqual("tom", o1.Trade.Company.Users[0].Name);
            Assert.AreEqual(UserType.Manager, o1.Trade.Company.Users[0].Type);

            Assert.AreEqual("Selling", o1.Trade.Company.Departments[0].Name);

            Assert.AreEqual("Printer", o1.Trade.Company.Resources[0].Name);
            Assert.AreEqual("Printer", o1.Trade.Company.Departments[0].Resources[0].Name);

            Assert.AreEqual("jerry", o1.Trade.Company.Departments[0].Employees[0].Name);
        }
    }
}
