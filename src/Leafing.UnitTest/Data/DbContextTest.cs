using Leafing.Data;
using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [DbTable("People")]
    public class CtxUser : DbObjectModel<CtxUser>
    {
        public string Name { get; set; }
    }

    [DbContext("SqlServerMock")]
    public class CtxSvcUser : DbObjectModel<CtxSvcUser>
    {
        public string Name { get; set; }
    }

    [TestFixture]
    public class DbContextTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            DbEntry.UsingConnection(
                ()=>
                    {
                        new CtxUser { Name = "test" }.Save();
                        DbEntry.NewTransaction(() => new CtxSvcUser {Name = "aaa"}.Save());
                    });
        }

        [Test]
        public void Test2()
        {
            DbEntry.UsingTransaction(
                () =>
                {
                    new CtxUser { Name = "test" }.Save();
                    DbEntry.NewTransaction(() => new CtxSvcUser { Name = "aaa" }.Save());
                });
        }
    }
}
