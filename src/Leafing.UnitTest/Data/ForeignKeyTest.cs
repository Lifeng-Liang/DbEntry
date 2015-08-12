using System.Collections.Generic;
using Leafing.Data;
using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [TestFixture]
    public class ForeignKeyTest : DataTestBase
    {
        // ReSharper disable InconsistentNaming
        private const string SQLiteException = "System.Data.SQLite.SQLiteException";
		private const string SQLiteExceptionMessage = "constraint failed\nforeign key constraint failed";
        // ReSharper restore InconsistentNaming

        [DbTable("People")]
        public class Person : DbObjectModel<Person>
        {
            public string Name { get; set; }

			public HasMany<Computer> Computers { get; private set; }

			public Person ()
			{
				Computers = new HasMany<Computer>(this, "Id", "Person_Id");
			}
        }

        [DbTable("PCs")]
        public class Computer : DbObjectModel<Computer>
        {
            public string Name { get; set; }

            [DbColumn("Person_Id")]
			public BelongsTo<Person, long> Owner { get; private set; }

			public Computer ()
			{
				Owner = new BelongsTo<Person, long>(this, "Person_Id");
			}
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
			var c = Computer.Find(o => o.Owner.Value.Id == 3, o => o.Id);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual("DELL", c[0].Name);
            Assert.AreEqual("HP", c[1].Name);

            var p = Person.FindById(3);
            p.Delete();

			var c1 = Computer.Find(o => o.Owner.Value.Id == 3, o => o.Id);
            Assert.AreEqual(0, c1.Count);

            // ReSharper disable ConditionIsAlwaysTrueOrFalse
			var c2 = Computer.Find(o => o.Owner.Value.Id == null, o => o.Id);
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            Assert.AreEqual(2, c2.Count);
            Assert.AreEqual("DELL", c2[0].Name);
            Assert.AreEqual("HP", c2[1].Name);
        }
    }
}
