
#region usings

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Lephone.Data.Definition;
using Lephone.Data;

#endregion

namespace Lephone.UnitTest.Data
{
    class vtest
    {
        [MaxLength(5), StringColumn(IsUnicode=false)]
        public string Name;

        [AllowNull, MaxLength(8)]
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

    [TestFixture]
    public class ValidateHandlerTest
    {
        [Test]
        public void Test1()
        {
            ValidateHandler vh = new ValidateHandler(true, 1);
            Assert.IsTrue(vh.ValidateObject(new vtest("12345", null, "a@b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest("123456", null, "a@b.c")));
            Assert.IsTrue(vh.ValidateObject(new vtest("≤‚ ‘1", null, "a@b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest("≤‚ ‘“ª", null, "a@b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest(null, null, "a@b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest("", null, "a@b.c")));
            Assert.IsTrue(vh.ValidateObject(new vtest("1", null, "a@b.c")));

            Assert.IsTrue(vh.ValidateObject(new vtest("1", "", "a@b.c")));
            Assert.IsTrue(vh.ValidateObject(new vtest("1", "12345678", "a@b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest("1", "123456789", "a@b.c")));
            Assert.IsTrue(vh.ValidateObject(new vtest("1", "≤‚ ‘≤‚ ‘≤‚ ‘≤‚ ‘", "a@b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest("1", "≤‚ ‘≤‚ ‘≤‚ ‘≤‚ ‘1", "a@b.c")));

            Assert.IsFalse(vh.ValidateObject(new vtest("1", null, "a.b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest("1", null, null)));
            Assert.IsFalse(vh.ValidateObject(new vtest("1", null, "")));
            Assert.IsFalse(vh.ValidateObject(new vtest("1", null, "1")));
        }

        [Test]
        public void Test2()
        {
            ValidateHandler vh = new ValidateHandler(false, 0);
            Assert.IsTrue(vh.ValidateObject(new vtest("12345", null, "a@b.c")));
            Assert.IsTrue(vh.ValidateObject(new vtest("12345", "", "a@b.c")));
        }

        [Test]
        public void Test3()
        {
            ValidateHandler vh = new ValidateHandler(false, 1);
            Assert.IsTrue(vh.ValidateObject(new vtest("12345", null, "a@b.c")));
            Assert.IsFalse(vh.ValidateObject(new vtest("123456", "", "a@b.c")));
        }
    }
}
