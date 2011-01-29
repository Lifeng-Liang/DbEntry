using Lephone.Data;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class ForeignKeyTest : DataTestBase
    {
        // ReSharper disable InconsistentNaming
        private const string SQLiteException = "System.Data.SQLite.SQLiteException";
        private const string SQLiteExceptionMessage = "Abort due to constraint violation\r\nforeign key constraint failed";
        // ReSharper restore InconsistentNaming

        [DbTable("People")]
        public class Person : DbObjectModel<Person>
        {
            public string Name { get; set; }

            [HasMany]
            public HasMany<Computer> Computers { get; set; }
        }

        [DbTable("PCs")]
        public class Computer : DbObjectModel<Computer>
        {
            public string Name { get; set; }

            [BelongsTo, DbColumn("Person_Id")]
            public Person Owner { get; set; }
        }
        
        [Test, ExpectedException(SQLiteException, ExpectedMessage = SQLiteExceptionMessage)]
        public void Test1()
        {
            DbEntry.Provider.ExecuteNonQuery(
@"PRAGMA foreign_keys = ON;
INSERT INTO [PCs] ([Name],[Person_Id]) VALUES ('should be failed', 6);");
        }

        [Test]
        public void Test2()
        {
            DbEntry.Provider.ExecuteNonQuery(
@"PRAGMA foreign_keys = ON;
INSERT INTO [PCs] ([Name],[Person_Id]) VALUES ('should be failed', NULL);");
        }

        [Test, ExpectedException(SQLiteException, ExpectedMessage = SQLiteExceptionMessage)]
        public void Test3()
        {
            DbEntry.UsingConnection(
                ()=>
                    {
                        DbEntry.Provider.ExecuteNonQuery("PRAGMA foreign_keys = ON;");
                        DbEntry.Provider.ExecuteNonQuery("INSERT INTO [PCs] ([Name],[Person_Id]) VALUES ('should be failed', 6);");
                    });
        }

        [Test, ExpectedException(SQLiteException, ExpectedMessage = SQLiteExceptionMessage)]
        public void Test4()
        {
            DbEntry.UsingTransaction(
                () =>
                {
                    DbEntry.Provider.ExecuteNonQuery("PRAGMA foreign_keys = ON;");
                    DbEntry.Provider.ExecuteNonQuery("INSERT INTO [PCs] ([Name],[Person_Id]) VALUES ('should be failed', 6);");
                });
        }

        [Test, ExpectedException(SQLiteException, ExpectedMessage = SQLiteExceptionMessage)]
        public void Test5()
        {
            DbEntry.UsingTransaction(
                () => DbEntry.Provider.ExecuteNonQuery("PRAGMA foreign_keys = ON;INSERT INTO [PCs] ([Name],[Person_Id]) VALUES ('should be failed', 6);"));
        }

        [Test, ExpectedException(SQLiteException, ExpectedMessage = SQLiteExceptionMessage)]
        public void Test6()
        {
            DbEntry.UsingConnection(
                () =>
                    {
                        DbEntry.Provider.ExecuteNonQuery("PRAGMA foreign_keys = ON;");
                        DbEntry.UsingTransaction(
                            () => DbEntry.Provider.ExecuteNonQuery(
                                "INSERT INTO [PCs] ([Name],[Person_Id]) VALUES ('should be failed', 6);"));
                    });
        }

        [Test]
        public void TestRemoveManypartModel()
        {
            var c = Computer.Find(o => o.Owner.Id == 3, o => o.Id);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual("DELL", c[0].Name);
            Assert.AreEqual("HP", c[1].Name);

            var p = Person.FindById(1);
            p.Delete();

            var c1 = Computer.Find(o => o.Owner.Id == 3, o => o.Id);
            Assert.AreEqual(0, c1.Count);

            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            var c2 = Computer.Find(o => o.Owner.Id == null, o => o.Id);
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            Assert.AreEqual(2, c2.Count);
            Assert.AreEqual("DELL", c2[0].Name);
            Assert.AreEqual("HP", c2[1].Name);
        }
    }
}
