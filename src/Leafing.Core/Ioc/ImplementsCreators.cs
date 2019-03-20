using System;
using System.Collections.Generic;

namespace Leafing.Core.Ioc {
    internal class ImplementsCreators {
        private ClassCreator _default;
        private int _defaultIndex;
        private readonly List<ClassCreator> _all = new List<ClassCreator>();

        public ImplementsCreators(ClassCreator creator) {
            Add(creator);
        }

        public void Add(ClassCreator creator) {
            if (creator.Index > 0) {
                if (creator.Index > _defaultIndex) {
                    _default = creator;
                    _defaultIndex = creator.Index;
                }
                foreach (var cc in _all) {
                    if (cc.Index == creator.Index) {
                        throw new IocException("[{0}] and [{1}] have the same index {2}", creator.Type, cc.Type, cc.Index);
                    }
                }
            }
            if (!creator.Name.IsNullOrEmpty()) {
                foreach (var cc in _all) {
                    if (cc.Name == creator.Name) {
                        throw new IocException("[{0}] and [{1}] have the same name {2}", creator.Type, cc.Type, cc.Name);
                    }
                }
            }
            _all.Add(creator);
        }

        public object Create() {
            return _default.Create();
        }

        public object Create(int index) {
            if (index == 0 || index == _defaultIndex) {
                return _default.Create();
            }
            foreach (var creator in _all) {
                if (creator.Index == index) {
                    return creator.Create();
                }
            }
            throw new IocException("Can not find implement of [{0}] by index {1}",
                _default.Type, index);
        }

        public object Create(string name) {
            if (name.IsNullOrEmpty()) {
                throw new ArgumentNullException("name");
            }
            foreach (var creator in _all) {
                if (creator.Name != null && creator.Name == name) {
                    return creator.Create();
                }
            }
            throw new IocException("Can not find implement of [{0}] by name {1}",
                _default.Type, name);
        }
    }
}
