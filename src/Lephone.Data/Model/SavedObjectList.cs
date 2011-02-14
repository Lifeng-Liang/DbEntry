using System.Collections.Specialized;

namespace Lephone.Data.Model
{
    public class SavedObjectList
    {
        private readonly HybridDictionary _dict;

        public SavedObjectList()
        {
            _dict = new HybridDictionary();
        }

        public SavedObjectList(int initialSize)
        {
            _dict = new HybridDictionary(initialSize);
        }

        public SavedObjectList(int initialSize, bool caseInsensitive)
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

