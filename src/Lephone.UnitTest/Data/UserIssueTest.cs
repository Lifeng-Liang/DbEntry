using System;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [DbTable("SYS_GRU")]
    public abstract class AppGrupoUsr : DbObjectModel<AppGrupoUsr>
    {
        [Length(20)]
        public abstract string Codigo { get; set; }

        [Length(50), AllowNull]
        public abstract string Nombre { get; set; }

        [HasMany(OrderBy = "Id")]
        public abstract HasMany<AppGrupoUsrMnu> GrpMnu { get; set; }
    }

    [DbTable("SYS_GUM")]
    public abstract class AppGrupoUsrMnu : DbObjectModel<AppGrupoUsrMnu>
    {
        [BelongsTo, DbColumn("gru_id")]
        public abstract AppGrupoUsr AppGrupoUsr { get; set; }

        [Length(20), AllowNull]
        public abstract string CodigoMenu { get; set; }

        [Length(50), AllowNull]
        public abstract string Atts { get; set; }

        public abstract AppGrupoUsrMnu Init(string CodigoMenu, string Atts);
    }

    [Serializable]
    public abstract class BaoXiuRS : DbObjectModel<BaoXiuRS>
    {
        [AllowNull, Length(50)]
        public abstract string UserId { get; set; }// ID 
        [AllowNull, Length(50)]
        public abstract string UserName { get; set; }//  
        [AllowNull, Length(50)]
        public abstract string ADDR { get; set; }//班级或办公室名称
        [AllowNull, Length(50)]
        public abstract string Fenlei { get; set; }// 分类 
        [AllowNull, Length(500)]
        public abstract string Content { get; set; }//故障内容
        public abstract Date? DT1 { get; set; }// 维修日期
        public abstract Date? DT { get; set; }// 报修日期
        [AllowNull, Length(50)]
        public abstract string People { get; set; }//PEOPLE 使用人
        [AllowNull, Length(50)]
        public abstract string Tel { get; set; }//

        [AllowNull, Length(50)]
        public abstract string Weixiu { get; set; }//维修记录
    }

    [TestFixture]
    public class UserIssueTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            var u = AppGrupoUsr.New;
            u.Codigo = "codigo";
            u.Nombre = "test";
            var g = AppGrupoUsrMnu.New.Init("menu", "atts");
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
            var o = BaoXiuRS.New;
            o.DT1 = new Date(2009, 5, 20);
            o.DT = new Date(2009, 5, 22);
            o.Save();

            var o1 = BaoXiuRS.FindById(o.Id);
            Assert.AreEqual(new Date(2009, 5, 20), o1.DT1);
            Assert.AreEqual(new Date(2009, 5, 22), o1.DT);

            o1.DT = new Date(1998, 12, 22);
            o1.Save();

            var o2 = BaoXiuRS.FindById(o1.Id);
            Assert.AreEqual(new Date(2009, 5, 20), o1.DT1);
            Assert.AreEqual(new Date(1998, 12, 22), o1.DT);
        }
    }

}
