using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    public class RequiredModel : DbObjectModel<RequiredModel>
    {
        [QueryRequired]
        public string Name { get; set; }

        public int Age { get; set; }
    }

    [DbTable("People")]
    public class RequiredPerson : DbObjectModel<RequiredPerson>
    {
        public string Name { get; set; }

        [HasMany]
        public IList<RequiredPc> Pc { get; private set; }
    }

    [DbTable("PCs")]
    public class RequiredPc : DbObjectModel<RequiredPc>
    {
        public string Name { get; set; }

        [BelongsTo, DbColumn("Person_Id"), QueryRequired]
        public RequiredPerson Owner { get; set; }
    }

    public class RequiredTwo : DbObjectModel<RequiredTwo>
    {
        public string Name { get; set; }

        [QueryRequired]
        public int Age { get; set; }

        [DbColumn("Person_Id"), QueryRequired]
        public long Owner { get; set; }
    }

    [TestFixture]
    public class QueryRequiredTest : DataTestBase
    {
        [Test, ExpectedException(typeof(DataException), ExpectedMessage = "The QueryRequired fields not found in query.")]
        public void Test1()
        {
            RequiredModel.Find(p => p.Age > 18);
        }

        [Test]
        public void Test2()
        {
            RequiredModel.Find(p => p.Name == "tom");
        }

        [Test]
        public void Test3()
        {
            RequiredModel.FindById(1);
        }

        [Test, ExpectedException(typeof(DataException), ExpectedMessage = "The QueryRequired fields not found in query.")]
        public void Test4()
        {
            RequiredModel.Find(CK.K["Age"] > 18);
        }

        [Test, ExpectedException(typeof(DataException), ExpectedMessage = "The QueryRequired fields not found in query.")]
        public void Test5()
        {
            RequiredModel.DeleteBy(p => p.Age > 18);
        }

        [Test, ExpectedException(typeof(DataException), ExpectedMessage = "The QueryRequired fields not found in query.")]
        public void Test6()
        {
            RequiredPc.Find(p => p.Name == "ibm");
        }

        [Test]
        public void Test7()
        {
            RequiredPc.Find(p => p.Owner.Id == 1);
        }

        [Test]
        public void Test8()
        {
            var ctx = ModelContext.GetInstance(typeof(RequiredPc));
            Assert.AreEqual("Person_Id", ctx.Info.QueryRequiredFields[0]);
        }

        [Test, ExpectedException(typeof(DataException), ExpectedMessage = "The QueryRequired fields not found in query.")]
        public void Test9()
        {
            RequiredModel.Find(p => p.Age.In(18, 19));
        }

        [Test, ExpectedException(typeof(DataException), ExpectedMessage = "The QueryRequired fields not found in query.")]
        public void Test10()
        {
            RequiredTwo.Find(p => p.Name == "tom");
        }

        [Test]
        public void Test11()
        {
            RequiredTwo.Find(p => p.Age > 18);
        }

        [Test]
        public void Test12()
        {
            RequiredTwo.Find(p => p.Owner == 1);
        }
    }
}
