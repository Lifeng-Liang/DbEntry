using System.Collections.Specialized;
using System.Web.Security;
using Leafing.Web;
using NUnit.Framework;

namespace Leafing.UnitTest.Web
{
    [TestFixture]
    public class MembershipTest
    {
        [Test]
        public void Test1()
        {
            var dmp = new DbEntryMembershipProvider();
            MembershipCreateStatus status;
            dmp.CreateUser("tom", "123456", "tom@123.com", "hello", "world", true, null, out status);

            //dmp.EnablePasswordReset = true;
            string pwd = dmp.ResetPassword("tom", "");
            Assert.IsNull(pwd);

            dmp.Initialize("", new NameValueCollection { { "requiresQuestionAndAnswer", "false" } });
            pwd = dmp.ResetPassword("tom", "");
            Assert.IsNotNull(pwd);
        }
    }
}
