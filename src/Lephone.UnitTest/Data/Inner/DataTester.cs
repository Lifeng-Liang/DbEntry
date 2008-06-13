using Lephone.Data;
using Lephone.Data.Builder;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Common;
using Lephone.Data.Dialect;
using NUnit.Framework;

namespace Lephone.UnitTest.Data.Inner
{
	[TestFixture]
	public class DataTester
	{
		private DbDialect dd = new SqlServer2000();

		[Test]
		public void TestSelectSentenceBuilder0()
		{
			SelectStatementBuilder ssb = new SelectStatementBuilder( "UserTable" );
            ssb.SetCountColumn("*");
			string s = "Select Count(*) As it__count__ From [UserTable];\n";
			Assert.AreEqual(s, ssb.ToSqlStatement(dd).SqlCommandText);
            ssb.SetCountColumn("abc");
            s = "Select Count([abc]) As it__count__ From [UserTable];\n";
            Assert.AreEqual(s, ssb.ToSqlStatement(dd).SqlCommandText);
            ssb.Keys.Add("zzz");
            s = "Select [zzz],Count([abc]) As it__count__ From [UserTable];\n";
            Assert.AreEqual(s, ssb.ToSqlStatement(dd).SqlCommandText);
        }

		[Test, ExpectedException(typeof(DataException))]
		public void TestSelectSentenceBuilder1()
		{
			SelectStatementBuilder ssb = new SelectStatementBuilder( "UserTable", null, new Range(1, 10) );
			ssb.ToSqlStatement(dd);
		}

        [Test]
        public void TestSelectSentenceBuilder1a()
        {
            SelectStatementBuilder ssb = new SelectStatementBuilder("UserTable", null, new Range(1, 10));
            ssb.Keys.Add("a");
            string s = "Select Top 10 [a] From [UserTable];\n<Text><60>()";
            Assert.AreEqual(s, ssb.ToSqlStatement(dd).ToString());
        }

        [Test]
		public void TestSelectSentenceBuilder2()
		{
            SelectStatementBuilder ssb = new SelectStatementBuilder("UserTable", null, new Range(1, 10));
            ssb.Keys.Add("a");
            ssb.Where.Conditions = new OrClause("ID", 5, 3, 2);
			string s = "Select Top 10 [a] From [UserTable] Where ([ID] = @ID_0) Or ([ID] = @ID_1) Or ([ID] = @ID_2);\n<Text><60>(@ID_0=5:Int32,@ID_1=3:Int32,@ID_2=2:Int32)";
			TesterHelper.AssertSqlSentenceEqual(s, ssb.ToSqlStatement(dd).ToString());
		}

		[Test]
		public void TestSelectSentenceBuilder3()
		{
            SelectStatementBuilder ssb = new SelectStatementBuilder("UserTable", null, new Range(1, 10));
            ssb.Keys.Add("a");
            ssb.Where.Conditions = new OrClause("ID", 5, 3, 2);
			ssb.Where.Conditions = new AndClause(ssb.Where.Conditions, new KeyValueClause("UserName", "l'lf"));
			string s = "Select Top 10 [a] From [UserTable] Where (([ID] = @ID_0) Or ([ID] = @ID_1) Or ([ID] = @ID_2)) And ([UserName] = @UserName_3);\n<Text><60>(@ID_0=5:Int32,@ID_1=3:Int32,@ID_2=2:Int32,@UserName_3=l'lf:String)";
			TesterHelper.AssertSqlSentenceEqual(s, ssb.ToSqlStatement(dd).ToString());
		}

		[Test]
		public void TestDeleteSentenceBuilder1()
		{
			DeleteStatementBuilder dsb = new DeleteStatementBuilder( "UserTable" );
			string s = "Delete From [UserTable];";
			TesterHelper.AssertSqlSentenceEqual(dsb.ToSqlStatement(dd).SqlCommandText, s);
		}

		[Test]
		public void TestDeleteSentenceBuilder2()
		{
			DeleteStatementBuilder dsb = new DeleteStatementBuilder( "UserTable" );
			dsb.Where.Conditions = new OrClause("ID", 3, 1);
            string Exp = "Delete From [UserTable] Where ([ID] = @ID_0) Or ([ID] = @ID_1);\n<Text><30>(@ID_0=3:Int32,@ID_1=1:Int32)";
			TesterHelper.AssertSqlSentenceEqual(Exp, dsb.ToSqlStatement(dd).ToString());
		}
	}
}
