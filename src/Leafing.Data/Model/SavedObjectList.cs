using System.Collections;

namespace Leafing.Data.Model {
    public class SavedObjectList {
        private readonly Hashtable _dict;

        public SavedObjectList() {
            _dict = new Hashtable();
        }

        public SavedObjectList(int initialSize) {
            _dict = new Hashtable(initialSize);
        }

        public bool Contains(object obj) {
            return _dict.Contains(obj);
        }

        public void Add(object obj) {
            _dict.Add(obj, 0);
        }
    }
}

