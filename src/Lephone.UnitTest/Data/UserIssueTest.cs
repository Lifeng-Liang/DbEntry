using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
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

        protected AppGrupoUsrMnu() {}

        protected AppGrupoUsrMnu(string CodigoMenu, string Atts)
        {
            this.CodigoMenu = CodigoMenu;
            this.Atts = Atts;
        }
    }

    [TestFixture]
    public class UserIssueTest
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

        [Test]
        public void Test1()
        {
            var u = AppGrupoUsr.New();
            u.Codigo = "codigo";
            u.Nombre = "test";
            var g = AppGrupoUsrMnu.New("menu", "atts");
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
    }
}
