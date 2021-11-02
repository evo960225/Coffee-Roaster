
using System;
namespace BGS {
    public class BGOContainer : BGO, IBGContainer {
        new public IBGContainer Parent { get; internal set; }
        BGO _bgObject;
        public BGO BGObject {
            get { return _bgObject; }
            set {
                if (value == null) {
                    if (_bgObject != null) {
                        _bgObject.Parent = null;
                        _bgObject = null;
                    }
                } else {
                    _bgObject = value;
                    _bgObject.Parent = this;
                }
            }
        }
        public BGOContainer()
            : base() {

        }

        public BGOContainer(BGO bgo) {
            BGObject = bgo;
        }

        public void RemovedFromParent() {
            if (Parent != null) {
                Parent.Remove(this.BGObject);
            } else {
                Console.WriteLine("Parent.Remove() Parent is null");
            }
        }
        public void Add(BGO item) {
            if (_bgObject != null) {
                throw new BGSException("BGContainer can't own plural BGObject");
            } else {
                BGObject = item;
            }
        }
        public bool Remove(BGO item) {
            if (BGObject == null) return false;
            if (BGObject != item) return false;
            BGObject = null;
            return true;
        }
        public bool RemoveTo(BGO bgo,IBGContainer container) {
            if (Remove(bgo)) {
                container.Add(bgo);
                return true;
            }
            return false;
        }
    }
}
