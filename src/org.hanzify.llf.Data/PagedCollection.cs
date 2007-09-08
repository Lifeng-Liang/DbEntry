
#region usings

using System;
using System.Collections;
using Lephone.Data.Common;

#endregion

namespace Lephone.Data
{
    public class PagedCollection : ICollection
    {
        private ICollection list;
        private long PageIndex;
        private int PageSize;
        private long MaxSize;

        public PagedCollection(ICollection list, long PageIndex, int PageSize, long MaxSize)
        {
            this.list = list;
            this.PageIndex = PageIndex;
            this.PageSize = PageSize;
            this.MaxSize = MaxSize;
        }

        public PagedCollection(IPagedSelector ps, int PageIndex)
        {
            this.list = ps.GetCurrentPage(PageIndex);
            this.PageIndex = PageIndex;
            this.PageSize = ps.PageSize;
            this.MaxSize = ps.GetResultCount();
        }

        public void CopyTo(Array array, int index)
        {
            int l = array.Length - index;
            if (l >= PageSize)
            {
                list.CopyTo(array, index);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        int ICollection.Count
        {
            get { return (int)MaxSize; }
        }

        public long Count
        {
            get { return MaxSize; }
        }

        public bool IsSynchronized
        {
            get { return list.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return list.SyncRoot; }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return new PagedCollectionEnumerator(list.GetEnumerator(), PageIndex, PageSize, MaxSize);
        }

        public class PagedCollectionEnumerator : IEnumerator
        {
            private IEnumerator ListEnum;
            private long MaxSize;
            private long StartIndex;
            private long EndIndex;
            private long CurrentIndex;

            public PagedCollectionEnumerator(IEnumerator ListEnum, long PageIndex, int PageSize, long MaxSize)
            {
                this.ListEnum = ListEnum;
                this.MaxSize = MaxSize;
                this.StartIndex = PageIndex * PageSize;
                this.EndIndex = StartIndex + PageSize;
                Reset();
            }

            public object Current
            {
                get
                {
                    if (CurrentIndex >= StartIndex && CurrentIndex < EndIndex)
                    {
                        return ListEnum.Current;
                    }
                    return null;
                }
            }

            public bool MoveNext()
            {
                CurrentIndex++;
                if (CurrentIndex >= StartIndex && CurrentIndex <= EndIndex)
                {
                    ListEnum.MoveNext();
                }
                return CurrentIndex < MaxSize;
            }

            public void Reset()
            {
                ListEnum.Reset();
                CurrentIndex = -1;
            }
        }
    }
}
