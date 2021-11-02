using hoshi_lib.Thread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGS {
    public interface IPalyerAction<T> : IProcess where T : BGO {

    }
    public interface IPalyerAction : IProcess {
        IEnumerable<BGO> CurrentChoiceableBGO { get; }
        void Listen(BGO bgo);
    }

    public interface IChainProcess : IProcess {
        IProcess NextProcess { get; set; }
    }
    public interface IBranch : IProcess {

    }
    public interface IGroup : IProcess {
        IProcess StartProcess { get; set; }
        IChainProcess EndProcess { get; set; }
    }

    public interface IProcess {
        string Name { get; set; }
        string Message { get; set; }

        void Do();
        void Break();
    }


    sealed public class PlayerAction : IPalyerAction, IChainProcess {

        public string Name { get; set; }
        public string Message { get; set; }
        public BGSys System { get; internal set; }
        public bool SingleChoiceSkip { get; set; }

        public Func<IEnumerable<BGO>> ChoiceableFunc {
            protected get { return _choosableFunc; }
            set { _choosableFunc = value; }
        }
        Func<IEnumerable<BGO>> _choosableFunc;
        public Action<BGO> DoProgram {
            protected get { return _doProgram; }
            set { _doProgram = value; }
        }
        Action<BGO> _doProgram;
        public IProcess NextProcess { get; set; }
        public IEnumerable<BGO> CurrentChoiceableBGO { get; protected set; }
       

        public PlayerAction() {

        }

        public void Do() {
            init();
            var list = ChoiceableFunc();
            if (list == null || list.Count()==0) {
                CurrentChoiceableBGO = new BGO[0];
                Break();
                return;
            }
            CurrentChoiceableBGO = list.ToArray();
            if (SingleChoiceSkip) {
                if (CurrentChoiceableBGO.Count() == 1 && System.CurrentProcess == this) {
                    Listen(CurrentChoiceableBGO.ElementAt(0));
                    return;
                }
            }
            openBGOState();
        }
        public void Listen(BGO bgo) {
            DoProgram(bgo);
            clearBGOState();
            next();
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
        }
    }

    sealed public class PlayerAction<T> : IPalyerAction, IChainProcess where T : BGO {

        public string Name { get; set; }
        public string Message { get; set; }
        public BGSys System { get; internal set; }
        public bool SingleChoiceSkip { get; set; }

        public Func<IEnumerable<T>> ChoiceableFunc {
            protected get { return _choosableFunc; }
            set { _choosableFunc = value; }
        }
        public Action<T> DoProgram {
            protected get { return _doProgram; }
            set { _doProgram = value; }
        }
        public IProcess NextProcess { get; set; }
        public IEnumerable<T> CurrentChoiceableBGO { get; protected set; }
        IEnumerable<BGO> IPalyerAction.CurrentChoiceableBGO {
            get { return CurrentChoiceableBGO; }
        }

        Func<IEnumerable<T>> _choosableFunc;
        Action<T> _doProgram;


        public void Listen(BGO bgo) {
            Listen(bgo as T);
        }


        internal PlayerAction() {

        }

        public void Do() {
            init();
            var list = ChoiceableFunc();
            if (list == null || list.Count() == 0) {
                CurrentChoiceableBGO = new T[0];
                Break();
                return;
            }
            CurrentChoiceableBGO = list.ToArray();
            if (SingleChoiceSkip) {
                if (CurrentChoiceableBGO.Count() == 1 && System.CurrentProcess == this) {
                    Listen(CurrentChoiceableBGO.ElementAt(0));
                    return;
                }
            }
            openBGOState();
            
        }
        public void Listen(T bgo) {
            DoProgram(bgo);
            clearBGOState();
            next();
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
        }
    }

}
