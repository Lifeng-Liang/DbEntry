using Lephone.Data;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    class vtest : IDbObject
    {
        [Length(1, 5), StringColumn(IsUnicode=false)]
        public string Name;

        [AllowNull, Length(1, 8)]
        public string Address;

        [StringColumn(Regular=CommonRegular.EmailRegular)]
        public string Email;

        public vtest(string Name, string Address, string Email)
        {
            this.Name = Name;
            this.Address = Address;
            this.Email = Email;
        }
    }

    class vtest0 : IDbObject
    {
        [Length(5, ErrorMessage = "Length limited by 5"), StringColumn(IsUnicode = false)]
        public string Name;

        [AllowNull, Length(8)]
        public string Address;

        [StringColumn(Regular = CommonRegular.EmailRegular, ErrorMessage = "Please input valid email address")]
        public string Email;

        public vtest0(string Name, string Address, string Email)
        {
            this.Name = Name;
            this.Address = Address;
            this.Email = Email;
        }
    }

    [DbTable("People")]
    public abstract class vPeople : DbObjectModel<vPeople>
    {
        [Index(UNIQUE = true, UniqueErrorMessage = "Already in use, please input another")]
        public abstract string Name { get; set; }

        public vPeople Init(string name)
        {
            this.Name = name;
            return this;
        }
    }

    [DbTable("People")]
    public abstract class vPeople0 : DbObjectModel<vPeople0>
    {
        [Index(UNIQUE = true)]
        public abstract string Name { get; set; }

        public vPeople0 Init(string name)
        {
            this.Name = name;
            return this;
        }
    }

    [TestFixture]
    public class ValidateHandlerTest
    {
        #region Init

        [SetUp]
        public void SetUp()
        {
            InitHelper.Init();
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
            ValidateHandler vh = new ValidateHandler(true);
            Assert.IsTrue(vh.ValidateObject(new vtest("12345", null, "a@b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest("123456", null, "a@b.c")));
            Assert.IsTrue(vh.ValidateObject(new vtest("测试1", null, "a@b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest("测试一", null, "a@b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest(null, null, "a@b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest("", null, "a@b.c")));
            Assert.IsTrue(vh.ValidateObject(new vtest("1", null, "a@b.c")));

            Assert.IsTrue(vh.ValidateObject(new vtest("1", "", "a@b.c")));
            Assert.IsTrue(vh.ValidateObject(new vtest("1", "12345678", "a@b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest("1", "123456789", "a@b.c")));
            Assert.IsTrue(vh.ValidateObject(new vtest("1", "测试测试测试测试", "a@b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest("1", "测试测试测试测试1", "a@b.c")));

            Assert.IsFalse(vh.ValidateObject(new vtest("1", null, "a.b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest("1", null, null)));
            Assert.IsFalse(vh.ValidateObject(new vtest("1", null, "")));
            Assert.IsFalse(vh.ValidateObject(new vtest("1", null, "1")));
        }

        [Test]
        public void Test2()
        {
            ValidateHandler vh = new ValidateHandler(false);
            Assert.IsTrue(vh.ValidateObject(new vtest0("12345", null, "a@b.c")));
            Assert.IsTrue(vh.ValidateObject(new vtest0("12345", "", "a@b.c")));
        }

        [Test]
        public void Test3()
        {
            ValidateHandler vh = new ValidateHandler(false);
            Assert.IsTrue(vh.ValidateObject(new vtest("12345", null, "a@b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest("123456", "", "a@b.c")));
        }

        [Test]
        public void Test4()
        {
            ValidateHandler vh = new ValidateHandler(false, false,"{1}", "", "", "", "", ", ");
            Assert.IsFalse(vh.ValidateObject(new vtest0("llf", "beijing", "aaa")));
            Assert.AreEqual(1, vh.ErrorMessages.Count);
            Assert.AreEqual("Please input valid email address", vh.ErrorMessages["Email"]);
        }

        [Test]
        public void Test5()
        {
            ValidateHandler vh = new ValidateHandler(false);
            Assert.IsTrue(vh.ValidateObject(new vtest0("llf", "beijing", "aaa@bbb.ccc")));
        }

        [Test]
        public void Test6()
        {
            ValidateHandler vh = new ValidateHandler(false, false, "{1}", "null", "match", "length", "unique", ", ");
            Assert.IsFalse(vh.ValidateObject(new vtest0("lifeng", "beijing", "aaa@bbb.ccc")));
            Assert.AreEqual(1, vh.ErrorMessages.Count);
            Assert.AreEqual("Length limited by 5", vh.ErrorMessages["Name"]);
        }

        [Test]
        public void Test7()
        {
            ValidateHandler vh = new ValidateHandler(false, false, "{1}", "null", "match", "length", "unique", ", ");
            Assert.IsFalse(vh.ValidateObject(new vtest("lifeng", "beijing", "aaa@bbb.ccc")));
            Assert.AreEqual(1, vh.ErrorMessages.Count);
            Assert.AreEqual("length", vh.ErrorMessages["Name"]);
        }

        [Test]
        public void Test8()
        {
            ValidateHandler vh = new ValidateHandler(false, false, "{1}", "null", "match", "length", "unique", ",");
            Assert.IsFalse(vh.ValidateObject(vPeople.New().Init("Tom")));
            Assert.AreEqual(1, vh.ErrorMessages.Count);
            Assert.AreEqual("Already in use, please input another", vh.ErrorMessages["Name"]);
        }

        [Test]
        public void Test9()
        {
            ValidateHandler vh = new ValidateHandler(false, false, "{1}", "null", "match", "length", "unique", ", ");
            Assert.IsFalse(vh.ValidateObject(vPeople0.New().Init("Tom")));
            Assert.AreEqual(1, vh.ErrorMessages.Count);
            Assert.AreEqual("unique", vh.ErrorMessages["Name"]);
        }
    }
}
