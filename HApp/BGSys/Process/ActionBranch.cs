
using System.Collections.Generic;
using System.Linq;

namespace BGS {

    sealed public class ActionBranch : IBranch,IPalyerAction {
        public string Name { get; set; }
        public string Message { get; set; }
        public BGSys System { get; internal set; }
        public bool SingleChoiceSkip { get; set; }
        public IEnumerable<IPalyerAction> PriorityPalyerActions {
            get {
                return _priorityPalyerActions;
            }
        }
        List<IPalyerAction> _priorityPalyerActions;
        public IEnumerable<BGO> CurrentChoiceableBGO { get; protected set; }

        internal ActionBranch() {
            _priorityPalyerActions = new List<IPalyerAction>();
        }

        public void Do() {
            init();
            List<BGO> list = new List<BGO>();
            foreach (var it in PriorityPalyerActions) {
                it.Do();
                list.AddRange(it.CurrentChoiceableBGO);
            }
            CurrentChoiceableBGO = list;
            BGSys.Instance.listeningProcess = this;
            BGSys.Instance.CurrentProcess = this;
            if (SingleChoiceSkip) {
                if (CurrentChoiceableBGO.Count() == 1) {
                    Listen(CurrentChoiceableBGO.ElementAt(0));
                    return;
                }
            }
        }
        public void Listen(BGO bgo) {
            IPalyerAction getaction = null;
            foreach (var it in PriorityPalyerActions) {
                if (getaction == null && it.CurrentChoiceableBGO.Contains(bgo)) {
                    getaction = it as IPalyerAction;
                } else {
                    it.Break();
                }
            }
            if (getaction == null) throw new BGSException("Has't next process");
            getaction.Listen(bgo);
        }
        public void Break() {
            foreach (var it in PriorityPalyerActions) {
                it.Break();
            }
        }
        void init() {

        }


        public void AddPalyerAction(IPalyerAction action) {
            _priorityPalyerActions.Add(action);
        }
        public void AddPalyerActions(params IPalyerAction[] actions) {
            _priorityPalyerActions.AddRange(actions);
        }
        public void AddPalyerActions(IEnumerable<IPalyerAction> actions) {
            _priorityPalyerActions.AddRange(actions);
        }
        public void AddButtonBranch(string text, IProcess next) {
            var p = BGSys.Instance.NewButtonAction(text);
            p.ActionItems = new string[] { text };
            p.ChoiceableItemsFunc = (x) => x;
            p.DoProgram = (x) => { };
            p.NextProcess = next;
        }
    }
    
}
