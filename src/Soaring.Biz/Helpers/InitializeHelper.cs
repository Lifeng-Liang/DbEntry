using Leafing.Data;
using Soaring.Biz.Models;
using Soaring.Biz.Models.Enums;

namespace Soaring.Biz.Helpers
{
    public static class InitializeHelper
    {
        public static void Initialize()
        {
            if(User.GetCount(Condition.Empty) == 0)
            {
                var u = new User
                            {
                                Email = "test@163.com",
                                Password = "123",
                                Nick = "test",
                                Mobile = "123456",
                                Type = UserType.Adminstrator,
                            };
                u.ResetSessionId();
                u.Save();
            }
        }
    }
}
