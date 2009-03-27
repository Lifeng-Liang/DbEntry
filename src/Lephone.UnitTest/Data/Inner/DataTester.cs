using Lephone.Data;
using Lephone.Data.Builder;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Data.Dialect;
using NUnit.Framework;

namespace Lephone.UnitTest.Data.Inner
{
	[TestFixture]
	public class DataTester
	{
		private readonly DbDialect dd = new SqlServer2000();

		[Test]
		public void TestSelectSentenceBuilder0()
		{
			var ssb = new SelectStatementBuilder( "UserTable" );
            ssb.SetCountColumn("*");
			string s = "SELECT COUNT(*) AS it__count__ FROM [UserTable];\n";
			Assert.AreEqual(s, ssb.ToSqlStatement(dd).SqlCommandText);
            ssb.SetCountColumn("abc");
            s = "SELECT COUNT([abc]) AS it__count__ FROM [UserTable];\n";
            Assert.AreEqual(s, ssb.ToSqlStatement(dd).SqlCommandText);
            ssb.Keys.Add("zzz");
            s = "SELECT [zzz],COUNT([abc]) AS it__count__ FROM [UserTable];\n";
            Assert.AreEqual(s, ssb.ToSqlStatement(dd).SqlCommandText);
        }

		[Test, ExpectedException(typeof(DataException))]
		public void TestSelectSentenceBuilder1()
		{
			var ssb = new SelectStatementBuilder( "UserTable", null, new Range(1, 10) );
			ssb.ToSqlStatement(dd);
		}

        [Test]
        public void TestSelectSentenceBuilder1a()
        {
            var ssb = new SelectStatementBuilder("UserTable", null, new Range(1, 10));
            ssb.Keys.Add("a");
            const string s = "SELECT TOP 10 [a] FROM [UserTable];\n<Text><60>()";
            Assert.AreEqual(s, ssb.ToSqlStatement(dd).ToString());
        }

        [Test]
		public void TestSelectSentenceBuilder2()
		{
            var ssb = new SelectStatementBuilder("UserTable", null, new Range(1, 10));
            ssb.Keys.Add("a");
            ssb.Where.Conditions = new OrClause("ID", 5, 3, 2);
			const string s = "SELECT TOP 10 [a] FROM [UserTable] WHERE ([ID] = @ID_0) OR ([ID] = @ID_1) OR ([ID] = @ID_2);\n<Text><60>(@ID_0=5:Int32,@ID_1=3:Int32,@ID_2=2:Int32)";
			TesterHelper.AssertSqlSentenceEqual(s, ssb.ToSqlStatement(dd).ToString());
		}

		[Test]
		public void TestSelectSentenceBuilder3()
		{
            var ssb = new SelectStatementBuilder("UserTable", null, new Range(1, 10));
            ssb.Keys.Add("a");
            ssb.Where.Conditions = new OrClause("ID", 5, 3, 2);
			ssb.Where.Conditions = new AndClause(ssb.Where.Conditions, new KeyValueClause("UserName", "l'lf", CompareOpration.Equal, ColumnFunction.None));
			const string s = "SELECT TOP 10 [a] FROM [UserTable] WHERE (([ID] = @ID_0) OR ([ID] = @ID_1) OR ([ID] = @ID_2)) AND ([UserName] = @UserName_3);\n<Text><60>(@ID_0=5:Int32,@ID_1=3:Int32,@ID_2=2:Int32,@UserName_3=l'lf:String)";
			TesterHelper.AssertSqlSentenceEqual(s, ssb.ToSqlStatement(dd).ToString());
		}

		[Test]
		public void TestDeleteSentenceBuilder1()
		{
			var dsb = new DeleteStatementBuilder( "UserTable" );
			const string s = "DELETE FROM [UserTable];";
			TesterHelper.AssertSqlSentenceEqual(dsb.ToSqlStatement(dd).SqlCommandText, s);
		}

		[Test]
		public void TestDeleteSentenceBuilder2()
		{
			var dsb = new DeleteStatementBuilder( "UserTable" );
			dsb.Where.Conditions = new OrClause("ID", 3, 1);
            const string Exp = "DELETE FROM [UserTable] WHERE ([ID] = @ID_0) OR ([ID] = @ID_1);\n<Text><30>(@ID_0=3:Int32,@ID_1=1:Int32)";
			TesterHelper.AssertSqlSentenceEqual(Exp, dsb.ToSqlStatement(dd).ToString());
		}
	}
}
