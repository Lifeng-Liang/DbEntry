using System.Collections.Specialized;

namespace Lephone.Core
{
    public class AvoidObjectList
    {
        private readonly HybridDictionary _dict;

        public AvoidObjectList()
        {
            _dict = new HybridDictionary();
        }

        public AvoidObjectList(int initialSize)
        {
            _dict = new HybridDictionary(initialSize);
        }

        public AvoidObjectList(int initialSize, bool caseInsensitive)
        {
            _dict = new HybridDictionary(initialSize, caseInsensitive);
        }

        public bool Contains(object obj)
        {
            return _dict.Contains(obj);
        }

        public void Add(object obj)
        {
            _dict.Add(obj, null);
        }
    }
}
