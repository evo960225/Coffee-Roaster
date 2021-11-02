using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BGS {
    public interface IBGContainer {
        void Add(BGO bgo);
        bool Remove(BGO bgo);
        bool RemoveTo(BGO bgo,IBGContainer container);
    }
    abstract public class BGO {
        #region private
        bool __isSelectable = false;
        bool __isSelected = false;
        #endregion
        #region Event
        public event SelectEvent SelectedEvent {
            add { selectedEvent += value; }
            remove { selectedEvent -= value; }
        }
        protected SelectEvent selectedEvent;
        public event SelectEvent UnselectEvent {
            add { unselectEvent += value; }
            remove { unselectEvent -= value; }
        }
        protected SelectEvent unselectEvent;
        public event SelectEvent SelectableEvent {
            add { selectableEvent += value; }
            remove { selectableEvent -= value; }
        }
        protected SelectEvent selectableEvent;
        public event SelectEvent UnselectableEvent {
            add { unselectableEvent += value; }
            remove { unselectableEvent -= value; }
        }
        protected SelectEvent unselectableEvent;
        public event UpdateEvent UpdatedEvent {
            add { updatedEvent += value; }
            remove { updatedEvent -= value; }
        }
        protected UpdateEvent updatedEvent;
        #endregion

        public int ID { get; private set; }
        public BGOContainer Parent { get; internal set; }

        Dictionary<string, object> _attributes = new Dictionary<string, object>(7);
        public IEnumerable<KeyValuePair<string, object>> Attributes { 
            get{
                return _attributes;
            }
        }
        public object this[string name] {
            get {
                if (_attributes.ContainsKey(name))
                    return _attributes[name];
                else
                    return null;
            }
            set {
                _attributes[name] = value;
            }
        }

        static int number = 0;
        public BGO() {
            ID = number++;
        }

        public bool IsSelected {
            get { return __isSelected; }
            protected set {
                var tmp = __isSelected;
                __isSelected = value;
                if (tmp != __isSelected) {
                    if (__isSelected) {
                        if (selectedEvent != null) selectedEvent();
                    } else {
                        if (unselectEvent != null) unselectEvent();
                    }
                }
            }
        }
        public bool IsSelectable {
            get {
                return __isSelectable;
            }
        }
        public void Select() {
            if (IsSelectable) {
                IsSelected = true;
                SendMsg();
            }
        }
        public void Unselect() {
            if (IsSelectable) {
                this.SendMsg();
            }
        }
        public void Selectable() {
            __isSelectable = true;
            if (selectableEvent != null) selectableEvent();
        }
        public void Unselectable() {
            __isSelectable = false;
            __isSelected = false;
            if (unselectableEvent != null) unselectableEvent();
        }
        void SendMsg() {
            var sys = BGSys.Instance;
            if (sys.CurrentProcess == null) throw new BGSSystemError("this process has't System");
            sys.ListenProcess(this);
        }
        public void EventClear() {
            selectedEvent = null;
            unselectEvent = null;
            selectableEvent = null;
            unselectableEvent = null;
            updatedEvent = null;
        }
        public void RemovedFromContainer() {
            if (Parent == null) return;
            Parent.BGObject = null;
        }
        public void RemovedFromBGOList() {
            if (Parent == null) return;
            Parent.RemovedFromParent();
        }

        
    }

}
