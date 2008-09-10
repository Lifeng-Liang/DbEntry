using System;

namespace Lephone.Data.Definition
{
    [Serializable]
    public class DbObjectModel<T, TKey> : DbObjectModelBase<T, TKey> where T : DbObjectModel<T, TKey>
    {
        [DbKey]
        public TKey Id { get; set; }
    }

    [Serializable]
    public class DbObjectModel<T> : DbObjectModel<T, long> where T : DbObjectModel<T, long>
    {
    }
}
