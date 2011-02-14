using Lephone.Data.Definition;

namespace Lephone.Data.Model
{
    public class GroupByObject<T> : IDbObject
    {
        // set it to key to make it looks like the first column
        [DbKey(IsDbGenerate = false)]
        public T Column;

        public long Count;

        public GroupByObject() { }

        public GroupByObject(T column, long count)
        {
            Column = column;
            Count = count;
        }
    }
}

