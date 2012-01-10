namespace Soaring.Biz.Helpers
{
    public static class CommonExtends
    {
        public static bool LikeNull(this string s)
        {
            if (s == null)
            {
                return true;
            }
            if (s.Trim() == "")
            {
                return true;
            }
            return false;
        }
    }
}
