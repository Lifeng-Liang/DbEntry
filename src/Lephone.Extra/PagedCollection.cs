using System;
using System.Collections;
using Lephone.Data;
using Lephone.Data.Definition;

namespace Lephone.Extra
{
    public class PagedCollection<T> : ICollection where T : class, IDbObject
    {
        private readonly ICollection _list;
        private readonly long _pageIndex;
        private readonly int _pageSize;
        private readonly long _maxSize;

        public PagedCollection(ICollection list, long pageIndex, int pageSize, long maxSize)
        {
            this._list = list;
            this._pageIndex = pageIndex;
            this._pageSize = pageSize;
            this._maxSize = maxSize;
        }

        public PagedCollection(IPagedSelector<T> ps, int pageIndex)
        {
            this._list = ps.GetCurrentPage(pageIndex);
            this._pageIndex = pageIndex;
            this._pageSize = ps.PageSize;
            this._maxSize = ps.GetResultCount();
        }

        public void CopyTo(Array array, int index)
        {
            int l = array.Length - index;
            if (l >= _pageSize)
            {
                _list.CopyTo(array, index);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        int ICollection.Count
        {
            get { return (int)_maxSize; }
        }

        public long Count
        {
            get { return _maxSize; }
        }

        public bool IsSynchronized
        {
            get { return _list.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return _list.SyncRoot; }
        }

        public IEnumerator GetEnumerator()
        {
            return new PagedCollectionEnumerator(_list.GetEnumerator(), _pageIndex, _pageSize, _maxSize);
        }

        public class PagedCollectionEnumerator : IEnumerator
        {
            private readonly IEnumerator _listEnum;
            private readonly long _maxSize;
            private readonly long _startIndex;
            private readonly long _endIndex;
            private long _currentIndex;

            public PagedCollectionEnumerator(IEnumerator listEnum, long pageIndex, int pageSize, long maxSize)
            {
                this._listEnum = listEnum;
                this._maxSize = maxSize;
                this._startIndex = pageIndex * pageSize;
                this._endIndex = _startIndex + pageSize;
                Reset();
            }

            public object Current
            {
                get
                {
                    if (_currentIndex >= _startIndex && _currentIndex < _endIndex)
                    {
                        return _listEnum.Current;
                    }
                    return null;
                }
            }

            public bool MoveNext()
            {
                _currentIndex++;
                if (_currentIndex >= _startIndex && _currentIndex <= _endIndex)
                {
                    _listEnum.MoveNext();
                }
                return _currentIndex < _maxSize;
            }

            public void Reset()
            {
                _listEnum.Reset();
                _currentIndex = -1;
            }
        }
    }
}
