//#define SqlServerActive

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using Lephone.Data;
using Lephone.Data.SqlEntry;
using Lephone.MockSql;
using Lephone.MockSql.Recorder;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class BulkCopyTest
    {
        #region Init

        [SetUp]
        public void SetUp()
        {
            InitHelper.Init();
            StaticRecorder.ClearMessages();
        }

        [TearDown]
        public void TearDown()
        {
            InitHelper.Clear();
        }

        #endregion

#if SqlServerActive
        [Test]
        public void TestGetTheRightCopier3()
        {
            DbContext dc = new DbContext("SqlServer");
            dc.UsingTransaction(delegate()
            {
                IDbBulkCopy c = dc.GetDbBulkCopy();
                Assert.IsNotNull(c);
                Assert.IsTrue(c is SqlServerBulkCopy);
            });
        }

        [Test]
        public void TestSqlServeerBulkCopy()
        {
            DbContext dc = new DbContext("SqlServer");
            SqlStatement sql = new SqlStatement("select [Id],[Name] from [Books] order by [Id]");
            List<long> rcs = new List<long>();
            DbEntry.Context.ExecuteDataReader(sql, delegate(IDataReader dr)
            {
                dc.UsingConnection(delegate()
                {
                    IDbBulkCopy c = dc.GetDbBulkCopy();
                    c.BatchSize = 2;
                    c.DestinationTableName = "test";
                    c.NotifyAfter = 3;
                    c.SqlRowsCopied += new SqlRowsCopiedEventHandler(delegate(object sender, SqlRowsCopiedEventArgs e)
                    {
                        rcs.Add(e.RowsCopied);
                    });
                    c.WriteToServer(dr);
                });
            });
            Assert.AreEqual(1, rcs.Count);
            Assert.AreEqual(3, rcs[0]);
        }
#endif

        [Test]
        public void TestGetTheRightCopier()
        {
            IDbBulkCopy c = DbEntry.Context.GetDbBulkCopy();
            Assert.IsNotNull(c);
            Assert.IsTrue(c is CommonBulkCopy);
        }

        [Test, ExpectedException(typeof(InvalidCastException))]
        public void TestGetTheRightCopier2()
        {
            DbContext dc = new DbContext("SqlServerMock");
            dc.UsingTransaction(delegate()
            {
                IDbBulkCopy c = dc.GetDbBulkCopy(); // exception
                Assert.IsNotNull(c);
                Assert.IsTrue(c is SqlServerBulkCopy);
            });
        }

        [Test]
        public void TestCommonBulkCopy()
        {
            DbContext dc = new DbContext("SQLite");
            SqlStatement sql = new SqlStatement("select [Id],[Name] from [Books] order by [Id]");
            List<long> rcs = new List<long>();
            DbEntry.Context.ExecuteDataReader(sql, delegate(IDataReader dr)
            {
                dc.UsingConnection(delegate()
                {
                    IDbBulkCopy c = dc.GetDbBulkCopy();
                    c.BatchSize = 2;
                    c.DestinationTableName = "test";
                    c.NotifyAfter = 3;
                    c.SqlRowsCopied += new SqlRowsCopiedEventHandler(delegate(object sender, SqlRowsCopiedEventArgs e)
                    {
                        rcs.Add(e.RowsCopied);
                    });
                    c.WriteToServer(dr);
                });
            });
            Assert.AreEqual(1, rcs.Count);
            Assert.AreEqual(3, rcs[0]);

            Assert.AreEqual(5, StaticRecorder.Messages.Count);
            StaticRecorder.Messages.ForEach(delegate(string s)
            {
                Assert.AreEqual("Insert Into [test] ([Id],[Name]) Values (@Id_0,@Name_1);\n", s);
            });
        }

        [Test]
        public void TestCommonBulkCopyWithTable()
        {
            DbContext dc = new DbContext("SQLite");
            SqlStatement sql = new SqlStatement("select [Id],[Name] from [Books] order by [Id]");
            List<long> rcs = new List<long>();
            DataSet ds = DbEntry.Context.ExecuteDataset(sql);
            dc.UsingConnection(delegate()
            {
                IDbBulkCopy c = dc.GetDbBulkCopy();
                c.BatchSize = 2;
                c.DestinationTableName = "test";
                c.NotifyAfter = 3;
                c.SqlRowsCopied += new SqlRowsCopiedEventHandler(delegate(object sender, SqlRowsCopiedEventArgs e)
                {
                    rcs.Add(e.RowsCopied);
                });
                c.WriteToServer(ds.Tables[0]);
            });
            Assert.AreEqual(1, rcs.Count);
            Assert.AreEqual(3, rcs[0]);

            Assert.AreEqual(5, StaticRecorder.Messages.Count);
            StaticRecorder.Messages.ForEach(delegate(string s)
            {
                Assert.AreEqual("Insert Into [test] ([Id],[Name]) Values (@Id_0,@Name_1);\n", s);
            });
        }

        [Test]
        public void TestCommonBulkCopyWithRowArray()
        {
            DbContext dc = new DbContext("SQLite");
            SqlStatement sql = new SqlStatement("select [Id],[Name] from [Books] order by [Id]");
            List<long> rcs = new List<long>();
            DataSet ds = DbEntry.Context.ExecuteDataset(sql);
            dc.UsingConnection(delegate()
            {
                IDbBulkCopy c = dc.GetDbBulkCopy();
                c.BatchSize = 2;
                c.DestinationTableName = "test";
                c.NotifyAfter = 2;
                c.SqlRowsCopied += new SqlRowsCopiedEventHandler(delegate(object sender, SqlRowsCopiedEventArgs e)
                {
                    rcs.Add(e.RowsCopied);
                });
                c.WriteToServer((DataRow[])new ArrayList(ds.Tables[0].Rows).ToArray(typeof(DataRow)));
            });
            Assert.AreEqual(2, rcs.Count);
            Assert.AreEqual(2, rcs[0]);
            Assert.AreEqual(4, rcs[1]);

            Assert.AreEqual(5, StaticRecorder.Messages.Count);
            StaticRecorder.Messages.ForEach(delegate(string s)
            {
                Assert.AreEqual("Insert Into [test] ([Id],[Name]) Values (@Id_0,@Name_1);\n", s);
            });
        }

        [Test]
        public void TestCommonBulkCopyAbort()
        {
            DbContext dc = new DbContext("SQLite");
            SqlStatement sql = new SqlStatement("select [Id],[Name] from [Books] order by [Id]");
            DbEntry.Context.ExecuteDataReader(sql, delegate(IDataReader dr)
            {
                dc.UsingConnection(delegate()
                {
                    IDbBulkCopy c = dc.GetDbBulkCopy();
                    c.BatchSize = 2;
                    c.DestinationTableName = "test";
                    c.NotifyAfter = 3;
                    c.SqlRowsCopied += new SqlRowsCopiedEventHandler(delegate(object sender, SqlRowsCopiedEventArgs e)
                    {
                        e.Abort = true;
                    });
                    c.WriteToServer(dr);
                });
            });
            Assert.AreEqual(3, StaticRecorder.Messages.Count);
            StaticRecorder.Messages.ForEach(delegate(string s)
            {
                Assert.AreEqual("Insert Into [test] ([Id],[Name]) Values (@Id_0,@Name_1);\n", s);
            });
        }

        [Test]
        public void TestBulkCopyWithNullValue()
        {
            DbContext dc = new DbContext("SQLite");
            SqlStatement sql = new SqlStatement("select [Id],[Name],[MyInt],[MyBool] from [NullTest] order by [Id]");
            DbEntry.Context.ExecuteDataReader(sql, delegate(IDataReader dr)
            {
                dc.UsingConnection(delegate()
                {
                    IDbBulkCopy c = dc.GetDbBulkCopy();
                    c.BatchSize = 2;
                    c.DestinationTableName = "test";
                    c.NotifyAfter = 3;
                    c.WriteToServer(dr);
                });
            });
            Assert.AreEqual(4, StaticRecorder.Messages.Count);
            StaticRecorder.Messages.ForEach(delegate(string s)
            {
                Assert.AreEqual("Insert Into [test] ([Id],[Name],[MyInt],[MyBool]) Values (@Id_0,@Name_1,@MyInt_2,@MyBool_3);\n", s);
            });
        }
    }
}
