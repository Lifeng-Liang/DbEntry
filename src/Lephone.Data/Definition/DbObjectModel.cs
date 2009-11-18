using System;

namespace Lephone.Data.Definition
{
    [Serializable]
    public class DbObjectModel<T, TKey> : DbObjectModelBase<T, TKey> where T : DbObjectModel<T, TKey>
    {
        [Exclude]
        protected TKey _id;

        [DbKey]
        public TKey Id
        {
            get { return _id; }
            set { _id = value; }
        }
    }

    [Serializable]
    public class DbObjectModel<T> : DbObjectModel<T, long> where T : DbObjectModel<T, long>
    {
    }
}
