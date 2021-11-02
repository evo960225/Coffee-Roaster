using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BGS {
    public class BGOList<T> : BGO, IBGContainer, IEnumerable<T> where T : BGO {

        List<BGOContainer> _bgContainers;
        public IEnumerable<BGOContainer> BGContainers {
            get { return _bgContainers; }
        }
        public IEnumerable<T> BGOs { get { return BGContainers.Select((x) => x.BGObject).OfType<T>(); } }
        int? _maxSize = null;
        public int? MaxSize {
            get { return _maxSize; }
            set {
                if (value != null && value < 0) throw new ArgumentOutOfRangeException("MaxSize", "BGOContainer");
                _maxSize = value;
            }
        }

        public BGOList(int capcity = 0) {
            _bgContainers = new List<BGOContainer>(capcity);
            for (int i = 0; i < capcity; ++i) {
                _bgContainers.Add(new BGOContainer());
            }
        }
        public BGOList(IEnumerable<T> collection) {
            _bgContainers = new List<BGOContainer>(collection.Count());
            AddRange(collection);
        }

        public T this[int index] {
            get {
                if (index >= _bgContainers.Count) throw new IndexOutOfRangeException("set BGO.this[]");
                return BGOs.ElementAt(index);
            }
            set {
                _bgContainers[index] = addBGOContainer(value);
            }
        }
        protected BGOContainer addBGOContainer(T bgo) {
            var bg = new BGOContainer(bgo);
            bg.Parent = this;
            _bgContainers.Add(bg);
            return bg;
        }
        protected bool removeBGOContainer(BGOContainer bgoc) {
            bgoc.Parent = null;
            var b = _bgContainers.Remove(bgoc);
            if (b) bgoc.BGObject = null;
            return b;
        }

        public int IndexOf(T item) {
            return _bgContainers.IndexOf(item.Parent);
        }
        public void RemoveAt(int index) {
            removeBGOContainer(_bgContainers[index]);
        }
        public void RemoveBGOAt(int index) {
            _bgContainers.ElementAt(index).BGObject = null;
        }
        public void Add(BGO item) {
            Add(item as T);
        }
        public void Add(T item) {
            if (MaxSize != null) {
                if (_bgContainers.Count == MaxSize) return;
            }
            addBGOContainer(item);
        }
        public void Insert(int index, T item) {
            if (_bgContainers.Count == MaxSize && MaxSize != -1) return;
            _bgContainers.Insert(index, new BGOContainer(item));
        }
        /// <summary>
        /// if contain is full, method has nothing to do.
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<T> collection) {
            if (MaxSize != null) {
                var len = (MaxSize - _bgContainers.Count);
                if (len <= 0 && MaxSize != -1) return;
            }
            foreach (var it in collection.ToList()) {
                addBGOContainer(it);
            }
        }
        public void AddAndFillEmptySpace(IEnumerable<T> item) {
            List<int> nullIndex = new List<int>(_bgContainers.Count / 2);
            for (int i = 0; i < _bgContainers.Count; ++i) {
                if (_bgContainers[i].BGObject == null) nullIndex.Add(i);
            }
            if (_bgContainers.Count >= MaxSize - nullIndex.Count && MaxSize != -1) return;

            int itemIndex = 0;
            foreach (var it in nullIndex) {
                if (itemIndex >= item.Count()) { break; }
                _bgContainers[it].BGObject = item.ElementAt(itemIndex++);
            }
            var itemCount = item.Count();
            if (itemIndex < itemCount) {
                while (itemIndex < itemCount) {
                    addBGOContainer(item.ElementAt(itemIndex++));
                }
            }
        }
        public void AddAndFillEmptySpace(params T[] item) {
            AddAndFillEmptySpace(item as IEnumerable<T>);
        }

        public void Clear() {
            foreach (var it in _bgContainers) {
                it.Parent = null;
            }
            _bgContainers.Clear();
        }
        public bool Contains(T item) {
            return _bgContainers.Contains(item.Parent);
        }

        public int Count {
            get { return _bgContainers.Count; }
        }
        public bool IsReadOnly {
            get { return (_bgContainers as IList<T>).IsReadOnly; }
        }
        public bool Remove(T item) {
            var bgoc = item.Parent;
            return removeBGOContainer(bgoc);
        }
        public bool Remove(BGO item) {
            return Remove(item as T);
        }

        public bool RemoveBGO(T item) {
            var ok = _bgContainers.Contains(item.Parent);
            if (!ok) return false;
            var bgoc = item.Parent;
            bgoc.BGObject = null;
            return true;
        }
        public bool RemoveBGO(BGO item) {
            var ok = _bgContainers.Contains(item.Parent);
            if (!ok) return false;
            var bgoc = item.Parent;
            bgoc.BGObject = null;
            return true;
        }
        public bool RemoveTo(BGO bgo, IBGContainer container) {
            if (Remove(bgo)) {
                container.Add(bgo);
                return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator() {
            return BGOs.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return BGOs.GetEnumerator();
        }

     
    }
}
