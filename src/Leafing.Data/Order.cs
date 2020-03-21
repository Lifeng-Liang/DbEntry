using Leafing.Data.Common;

namespace Leafing.Data {
    public static class Order {
        public static ASC Desc(this string key) {
            return (DESC)key;
        }

        public static OrderBy By(params ASC[] keys) {
            return new OrderBy(keys);
        }
    }
}
