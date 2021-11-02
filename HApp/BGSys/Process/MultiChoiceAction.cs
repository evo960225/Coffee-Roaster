using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGS {
    sealed public class MultiChoiceAction : IPalyerAction, IChainProcess {

        public string Name { get; set; }
        public string Message { get; set; }
        public BGSys System { get; internal set; }
        public IProcess NextProcess { get; set; }
        public bool SingleChoiceSkip { get; set; }

        public IEnumerable<BGO> CurrentChoiceableBGO { get; protected set; }
        public Func<IEnumerable<BGO>> ChoiceableFunc {
            protected get { return _choosableFunc; }
            set { _choosableFunc = value; }
        }
        Func<IEnumerable<BGO>> _choosableFunc;
        public Action<IEnumerable<BGO>> DoProgram {
            protected get { return _doProgram; }
            set { _doProgram = value; }
        }
        Action<IEnumerable<BGO>> _doProgram;
        public Func<IEnumerable<BGO>, bool> LoopCondition {
            protected get { return _loopCondition; }
            set { _loopCondition = value; }
        }
        Func<IEnumerable<BGO>, bool> _loopCondition;
        public IEnumerable<BGO> ChoicedBGOs {
            get { return _choiceBGOs; }
        }
        HashSet<BGO> _choiceBGOs = new HashSet<BGO>();
       

        internal MultiChoiceAction() {

        }

        public void Do() {
            init(); 
            var list = ChoiceableFunc();
            if (list == null || list.Count() == 0) {
                System.Break();
                return;
            }
            CurrentChoiceableBGO = list.ToArray();
            if (SingleChoiceSkip) {
                if (CurrentChoiceableBGO.Count() == 1 && System.CurrentProcess == this) {
                    System.ListenProcess(CurrentChoiceableBGO.ElementAt(0));
                    return;
                }
            }
            openBGOState();
        }

        public void Listen(BGO bgo) {
            if (CurrentChoiceableBGO.Contains(bgo)) {
                var tmpbogs = CurrentChoiceableBGO.ToList();
                _choiceBGOs.Add(bgo);
                foreach (var it in CurrentChoiceableBGO) {
                    if (!ChoicedBGOs.Contains(it)) {
                        it.Unselectable();
                    } else {
                        it.Select();
                    }
                }
            }
            if (_loopCondition == null || !_loopCondition(_choiceBGOs)) {
                DoProgram(_choiceBGOs);
                clearBGOState();
                _choiceBGOs.Clear();
                next();
            } else {
                var tmpbogs = ChoiceableFunc().ToList();
                foreach (var it in ChoicedBGOs) {
                    tmpbogs.Remove(it);
                }
                CurrentChoiceableBGO = tmpbogs;
                openBGOState();
            }
        }

        public void Break() {
            clearBGOState();
        }

        void next() {
            if (NextProcess == null) System.Break();
            else {
                System.Do(NextProcess);
            }
        }
        void init() {
            BGSys.Instance.listeningProcess = this;
            BGSys.Instance.CurrentProcess = this;
        }
        void openBGOState() {
            foreach (var it in CurrentChoiceableBGO) {
                it.Selectable();
            }
        }
        void clearBGOState() {
            foreach (var it in CurrentChoiceableBGO) {
                it.Unselectable();
            }
            foreach (var it in ChoicedBGOs) {
                if (it != null) {
                    it.Unselectable();
                }
            }
        }
    }

    sealed public class MultiChoiceAction<T> : IPalyerAction, IChainProcess where T : BGO {

        public string Name { get; set; }
        public string Message { get; set; }
        public BGSys System { get; internal set; }
        public IEnumerable<T> CurrentChoiceableBGO { get; protected set; }
        IEnumerable<BGO> IPalyerAction.CurrentChoiceableBGO {
            get { return CurrentChoiceableBGO; }
        }

        public Func<IEnumerable<T>> ChoiceableFunc {
            protected get { return _choosableFunc; }
            set { _choosableFunc = value; }
        }
        public Action<IEnumerable<T>> DoProgram {
            protected get { return _doProgram; }
            set { _doProgram = value; }
        }
        public Func<IEnumerable<T>, bool> LoopCondition {
            protected get { return _loopCondition; }
            set { _loopCondition = value; }
        }
        public IProcess NextProcess { get; set; }
        public IEnumerable<T> ChoicedBGOs {
            get { return _choiceBGOs; }
        }
        HashSet<T> _choiceBGOs = new HashSet<T>();
        
        Func<IEnumerable<T>> _choosableFunc;
        Action<IEnumerable<T>> _doProgram;
        Func<IEnumerable<T>, bool> _loopCondition;

        internal MultiChoiceAction() {

        }

        public void Do() {
            init();
            var list = ChoiceableFunc();
            CurrentChoiceableBGO = list.ToArray();
            openBGOState();
        }

        public void Listen(BGO bgo) {
            T t = bgo as T;
            if (CurrentChoiceableBGO.Contains(bgo)) {
                var tmpbogs = CurrentChoiceableBGO.ToList();
                _choiceBGOs.Add(t);
                foreach (var it in CurrentChoiceableBGO) {
                    if (!ChoicedBGOs.Contains(it)) {
                        it.Unselectable();
                    } else {
                        it.Select();
                    }
                }
            }
            if (_loopCondition == null || !_loopCondition(_choiceBGOs)) {
                DoProgram(_choiceBGOs);
                clearBGOState();
                _choiceBGOs.Clear();
                next();
            } else {
                var tmpbogs = ChoiceableFunc().ToList();
                foreach (var it in ChoicedBGOs) {
                    tmpbogs.Remove(it);
                }
                CurrentChoiceableBGO = tmpbogs;
                openBGOState();
            }
        }

        public void Break() {
            clearBGOState();
        }

        void next() {
            if (NextProcess == null) System.Break();
            else {
                System.Do(NextProcess);
            }
        }
        void init() {
            BGSys.Instance.listeningProcess = this;
            BGSys.Instance.CurrentProcess = this;
        }
        void openBGOState() {
            foreach (var it in CurrentChoiceableBGO) {
                if (it != null) {
                    it.Selectable();
                }
            }
        }
        void clearBGOState() {
            foreach (var it in CurrentChoiceableBGO) {
                if (it != null) {
                    it.Unselectable();
                }
            }
            foreach (var it in ChoicedBGOs) {
                if (it != null) {
                    it.Unselectable();
                }
            }
        }
       
    }
    
}
