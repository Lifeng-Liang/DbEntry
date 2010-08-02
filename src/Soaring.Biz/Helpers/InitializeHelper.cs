using Lephone.Data;
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
                                Email = "lifeng.liang@gmail.com",
                                Password = "123",
                                Nick = "lifeng",
                                Mobile = "123456",
                                Type = UserType.Adminstrator
                            };
                u.Save();
            }
        }
    }
}
