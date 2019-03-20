using System.Collections.Generic;
using Leafing.Data;
using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Linq {
    [DbContext("SQLite")]
    public class User : DbObjectModel<User> {
        public string Name { get; set; }

        public HasMany<Article> Articles { get; private set; }

        public User() {
            Articles = new HasMany<Article>(this, "Id", "User_Id");
        }
    }

    [DbContext("SQLite")]
    public class Article : DbObjectModel<Article> {
        public string Title { get; set; }

        public BelongsTo<User, long> User { get; set; }

        public Article() {
            User = new BelongsTo<User, long>(this, "User_Id");
        }
    }

    [TestFixture]
    public class FunctionTest : DataTestBase {
        [Test]
        public void Test1() {
            DbEntry.From<Article>().Where(p => p.User.Value.Id.In(1)).Select();
            AssertSql("SELECT [Id],[Title],[User_Id] AS [User] FROM [Article] WHERE [User_Id] IN (@in_0);\n<Text><60>(@in_0=1:Int64)");
        }

        [Test]
        public void Test2() {
            DbEntry.From<Article>().Where(p => p.Title.StartsWith("tom")).Select();
            AssertSql("SELECT [Id],[Title],[User_Id] AS [User] FROM [Article] WHERE [Title] LIKE @Title_0;\n<Text><60>(@Title_0=tom%:String)");
        }

        [Test]
        public void Test3() {
            DbEntry.From<Article>().Where(p => p.Title.ToLower() == "tom").Select();
            AssertSql("SELECT [Id],[Title],[User_Id] AS [User] FROM [Article] WHERE LOWER([Title]) = @Title_0;\n<Text><60>(@Title_0=tom:String)");
        }
    }
}