namespace Lephone.Data.Definition
{
    public abstract class LazyLoadOneBase<T> : ILazyLoading
    {
        protected object Owner;
        protected string RelationName;
        protected T m_Value;

        protected LazyLoadOneBase(object owner)
        {
            this.Owner = owner;
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

        public T Value
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

        void ILazyLoading.Init(string relationName)
        {
            this.RelationName = relationName;
        }

        void ILazyLoading.Load()
        {
            DoLoad();
        }

        protected abstract void DoLoad();
    }
}
