using System.Security.Cryptography;
using System.Text;
using Lephone.Core.Text;

namespace Soaring.Biz.Helpers
{
    public class CommonHelper
    {
        public static string GetHashedPassword(string password)
        {
            var hash = SHA512.Create();
            var bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            var ret = Base32StringCoding.Decode(bytes);
            return ret;
        }
    }
}
