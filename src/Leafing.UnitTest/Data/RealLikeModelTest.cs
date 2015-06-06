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
			var u = new User {Name = "tom", Type = UserType.Manager, Valid = true}; u.Company.Value = c;
			var d = new Department {Name = "Selling"}; d.Company.Value = c;
			var e = new Employee {Name = "jerry", Age = 24}; e.Company.Value = c; e.Department.Value = d;
			var r = new Resource {Name = "Printer", Count = 2}; r.Company.Value = c; r.Department.Value = d;
			var p = new Product { Name = "Tale Story" }; p.Company.Value = c;
			var t = new Trade { Name = "20110001", TotalPrice = 192m }; t.Company.Value = c;
            var o = new Order { Name = "Tale Story", Count = 10, Price = 19.2m};
			o.Description.Value = "test"; o.Trade.Value = t; o.Product.Value = p;

            o.Save();

            var o1 = Order.FindById(o.Id);
            Assert.AreEqual("Tale Story", o1.Name);
            Assert.AreEqual(10, o1.Count);

			Assert.AreEqual("20110001", o1.Trade.Value.Name);
			Assert.AreEqual(192m, o1.Trade.Value.TotalPrice);

			Assert.AreEqual("Tale Story", o1.Product.Value.Name);

			Assert.IsNotNull(o1.Trade.Value.Company);
			Assert.AreEqual("NotExist", o1.Trade.Value.Company.Value.Name);

			Assert.AreEqual("tom", o1.Trade.Value.Company.Value.Users[0].Name);
			Assert.AreEqual(UserType.Manager, o1.Trade.Value.Company.Value.Users[0].Type);

			Assert.AreEqual("Selling", o1.Trade.Value.Company.Value.Departments[0].Name);

			Assert.AreEqual("Printer", o1.Trade.Value.Company.Value.Resources[0].Name);
			Assert.AreEqual("Printer", o1.Trade.Value.Company.Value.Departments[0].Resources[0].Name);

			Assert.AreEqual("jerry", o1.Trade.Value.Company.Value.Departments[0].Employees[0].Name);
        }
    }
}
