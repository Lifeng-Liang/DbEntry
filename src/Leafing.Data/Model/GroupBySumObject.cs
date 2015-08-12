using Leafing.Data.Definition;

namespace Leafing.Data.Model
{
    public class GroupBySumObject<T1, T2> : IDbObject
    {
        // set it to key to make it looks like the first column
        [DbKey(IsDbGenerate = false)]
        public T1 Column;

        public T2 Sum;

        public GroupBySumObject() { }

        public GroupBySumObject(T1 column, T2 sum)
        {
            Column = column;
            Sum = sum;
        }
    }
}
