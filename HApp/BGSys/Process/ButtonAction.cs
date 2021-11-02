using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGS {

    sealed public class ButtonAction : IPalyerAction, IChainProcess {

        public string Name { get; set; }
        public string Message { get; set; }
        public BGSys System { get; internal set; }
        public bool SingleChoiceSkip { get; set; }

        public IEnumerable<string> ActionItems { get; set; }
        public Func<IEnumerable<string>, IEnumerable<string>> ChoiceableItemsFunc {
            protected get { return _choosableItemFunc; }
            set { _choosableItemFunc = value; }
        }
        public Action<ActionButton> DoProgram {
            protected get { return _doProgram; }
            set { _doProgram = value; }
        }
        public IProcess NextProcess { get; set; }
        public IEnumerable<ActionButton> CurrentChoiceableBGO { get; protected set; }
        IEnumerable<BGO> IPalyerAction.CurrentChoiceableBGO {
            get { return CurrentChoiceableBGO; }
        }
        Func<IEnumerable<string>, IEnumerable<string>> _choosableItemFunc;
        Action<ActionButton> _doProgram;

        public ButtonAction() {

        }

        public void Do() {
            init();
            var list = _choosableItemFunc(ActionItems);
            if (list == null || list.Count()==0) {
                CurrentChoiceableBGO = new ActionButton[0];
                Break();
                return;
            }
            var sys = BGSys.Instance;
            sys.AddActionButtons(list.ToArray());
            CurrentChoiceableBGO = sys.SelectItems.ToArray();
            if (SingleChoiceSkip) {
                if (CurrentChoiceableBGO.Count() == 1 && System.CurrentProcess == this) {
                    Listen(CurrentChoiceableBGO.ElementAt(0));
                    return;
                }
            }
            openBGOState();
        }
        public void Listen(BGO bgo) {
            DoProgram(bgo as ActionButton);
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
            if (CurrentChoiceableBGO == null) return;
            foreach (var it in CurrentChoiceableBGO) {
                if (it != null) {
                    it.Unselectable();
                    System.RemoveActionButton(it.Text);
                }
            }
        }
    }
}
