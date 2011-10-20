﻿using System;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using Lephone.Web;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [DbContext("Access")]
    public class CreatedOnModel : DbObjectModel<CreatedOnModel>
    {
        public string Name { get; set; }

        [SpecialName]
        public DateTime CreatedOn { get; set; }
    }

    [DbTable("SYS_GRU")]
    public class AppGrupoUsr : DbObjectModel<AppGrupoUsr>
    {
        [Length(20)]
        public string Codigo { get; set; }

        [Length(50), AllowNull]
        public string Nombre { get; set; }

        [HasMany(OrderBy = "Id")]
        public IList<AppGrupoUsrMnu> GrpMnu { get; private set; }
    }

    [DbTable("SYS_GUM")]
    public class AppGrupoUsrMnu : DbObjectModel<AppGrupoUsrMnu>
    {
        [BelongsTo, DbColumn("gru_id")]
        public AppGrupoUsr AppGrupoUsr { get; set; }

        [Length(20), AllowNull]
        public string CodigoMenu { get; set; }

        [Length(50), AllowNull]
        public string Atts { get; set; }
    }

    [Serializable]
    public class BaoXiuRS : DbObjectModel<BaoXiuRS>
    {
        [AllowNull, Length(50)]
        public string UserId { get; set; }// ID 
        [AllowNull, Length(50)]
        public string UserName { get; set; }//  
        [AllowNull, Length(50)]
        public string ADDR { get; set; }//班级或办公室名称
        [AllowNull, Length(50)]
        public string Fenlei { get; set; }// 分类 
        [AllowNull, Length(500)]
        public string Content { get; set; }//故障内容
        public Date? DT1 { get; set; }// 维修日期
        public Date? DT { get; set; }// 报修日期
        [AllowNull, Length(50)]
        public string People { get; set; }//PEOPLE 使用人
        [AllowNull, Length(50)]
        public string Tel { get; set; }//

        [AllowNull, Length(50)]
        public string Weixiu { get; set; }//维修记录
    }

    public class SelfGuid : IDbObject
    {
        [DbKey(IsDbGenerate = false)]
        public Guid Id;

        public string Name;
    }

    [DbTable("Article")]
    public class ErrorArticle : DbObjectModel<ErrorArticle>
    {
        public int Name { get; set; }
    }

    [TestFixture]
    public class UserIssueTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            var u = new AppGrupoUsr {Codigo = "codigo", Nombre = "test"};
            var g = new AppGrupoUsrMnu {CodigoMenu = "menu", Atts = "atts"};
            u.GrpMnu.Add(g);
            u.Save();

            var u1 = AppGrupoUsr.FindById(u.Id);
            Assert.IsNotNull(u1);
            foreach (var a in u1.GrpMnu)
            {
                Assert.IsNotNull(a);
            }
            Assert.AreEqual(1, u1.GrpMnu.Count);
            Assert.AreEqual("menu", u1.GrpMnu[0].CodigoMenu);
        }

        [Test]
        public void Test2()
        {
            var o = new BaoXiuRS {DT1 = new Date(2009, 5, 20), DT = new Date(2009, 5, 22)};
            o.Save();

            var o1 = BaoXiuRS.FindById(o.Id);
            Assert.AreEqual(new Date(2009, 5, 20), o1.DT1);
            Assert.AreEqual(new Date(2009, 5, 22), o1.DT);

            o1.DT = new Date(1998, 12, 22);
            o1.Save();

            var o2 = BaoXiuRS.FindById(o1.Id);
            Assert.AreEqual(new Date(2009, 5, 20), o2.DT1);
            Assert.AreEqual(new Date(1998, 12, 22), o2.DT);
        }

        [Test]
        public void Test3()
        {
            var gid = Guid.NewGuid();
            var o1 = new SelfGuid {Id = gid, Name = "tom"};
            DbEntry.Insert(o1);

            var o2 = DbEntry.GetObject<SelfGuid>(gid);
            Assert.IsNotNull(o2);
            Assert.AreEqual("tom", o2.Name);
        }

        [Test, ExpectedException(typeof(DataException), ExpectedMessage = "The type of member [Name] is [System.Int32] but sql value of it is [System.String]")]
        public void TestErrorArticle()
        {
            var ctx = ModelContext.GetInstance(typeof(ErrorArticle));
            ctx.Operator.ExecuteList<ErrorArticle>("SELECT * FROM [Article]");
        }

        [Test, ExpectedException(typeof(DataException), ExpectedMessage = "The type of member [Name] is [System.Int32] but sql value of it is [System.String]")]
        public void TestErrorArticle2()
        {
            ErrorArticle.Find(Condition.Empty);
        }

        [Test]
        public void TestDbEntryMembershipUser()
        {
            var u = new CreatedOnModel {Name = "tom"};
            u.Save();
            var n = StaticRecorder.Messages.Count;
            Assert.AreEqual(@"INSERT INTO [Created_On_Model] ([Name],[CreatedOn]) VALUES (@Name_0,Now());<Text><30>(@Name_0=tom:String)",
                StaticRecorder.Messages[n - 2]);
            AssertSql(@"SELECT @@IDENTITY;<Text><30>(@Name_0=tom:String)");
        }
    }
}
