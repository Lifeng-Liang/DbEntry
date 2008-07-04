using System;

namespace Lephone.Data.Definition
{
    [Serializable]
    public class DbObjectModel<T, TKey> : DbObjectModelBase<T, TKey> where T : DbObjectModel<T, TKey>
    {
        private TKey m_Id;

        [DbKey]
        public TKey Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
    }

    [Serializable]
    public class DbObjectModel<T> : DbObjectModel<T, long> where T : DbObjectModel<T, long>
    {
    }
}
