using Leafing.Data.SqlEntry.Dynamic;

namespace Leafing.Web.Mvc
{
    public static class TestHelper
    {
        public static DynamicRow GetValues(this ControllerBase ctrl)
        {
            var result = new DynamicRow();
            foreach(var o in ctrl.Bag)
            {
                result.SetMember(o.Key, o.Value);
            }
            return result;
        }
    }
}
