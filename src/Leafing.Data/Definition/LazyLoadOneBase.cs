namespace Leafing.Data.Definition
{
    public abstract class LazyLoadOneBase<T> : ILazyLoading, IThat
    {
        protected DbObjectSmartUpdate Owner;
        protected string RelationName;
        protected T m_Value;

        protected LazyLoadOneBase(DbObjectSmartUpdate owner, string relationName)
        {
            this.Owner = owner;
            this.RelationName = relationName;
        }

        public bool IsLoaded { get; set; }

        object ILazyLoading.Read()
        {
            if (!IsLoaded)
            {
                ((ILazyLoading)this).Load();
                IsLoaded = true;
            }
            return m_Value;
        }

        void ILazyLoading.Write(object item, bool isLoad)
        {
            object oldValue = m_Value;
            m_Value = (T)item;
            DoWrite(oldValue, isLoad);
            IsLoaded = true;
        }

        protected virtual void DoWrite(object oldValue, bool isLoad) { }

        public virtual T Value
        {
            get
            {
                return (T)((ILazyLoading)this).Read();
            }
            set
            {
                ((ILazyLoading)this).Write(value, false);
            }
        }

        void ILazyLoading.Load()
        {
            DoLoad();
        }

        protected abstract void DoLoad();

        public void Add(object obj)
        {
            Value = (T)obj;
        }
    }
}
