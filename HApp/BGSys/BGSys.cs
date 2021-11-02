using hoshi_lib.Thread;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BGS {
    public class BGSys {

        //@Edit:HTimer純取在小一點
        public HTimer MainTimer {
            get { return mainTimer; }
        }
        HTimer mainTimer = new HTimer(15);
        static public BGSys Instance {
            get {
                if (_instance == null) _instance = new BGSys();
                return _instance;
            }
        }
        static protected BGSys _instance;
        internal IPalyerAction listeningProcess { get; set; }
        //Player
        public int PlayerAmount {
            get { return _playerAmount; }
        }
        public IEnumerable<Player> Players {
            get {
                return _players;
            }
        }
        public Player CurrentPlayer {
            get { return _players[cur_player_index]; }
        }
        //Process
        public IProcess CurrentProcess { get; internal set; }
        public IEnumerable<ActionButton> SelectItems {
            get { return _selectItems; }
        }
        public bool IsEnabled {
            get { return MainTimer.IsEnabled; }
        }
        public int ListenMsgTimes { get; protected set; }
        //Event
        public event Action EndingEvent {
            add { _endingEvent += value; }
            remove { _endingEvent -= value; }
        }
        Action _endingEvent;

        #region private
        BGOList<ActionButton> _selectItems = new BGOList<ActionButton>();
        int _playerAmount = 0;
        List<Player> _players = new List<Player>(8);
        int cur_player_index = 0;
        #endregion

        protected BGSys() {

        }

        //process 
        bool nextTickEnd = false;
        EventHandler timerStopEvent;
        public void Start(IProcess begin) {
            foreach (var it in Players) {
                it.Init();
            }

            timerStopEvent = (s, e) => {
                if (nextTickEnd) MainTimer.Stop();
                nextTickEnd = false;
                MainTimer.Tick -= timerStopEvent;
            };
            MainTimer.Tick += timerStopEvent;
            MainTimer.Start();

            CurrentProcess = begin;
            init();
            Do(begin);
        }
        public void Break() {
            if (priorityProcess.Count > 0) {
                var priP = priorityProcess.Pop();
                priP.Do();
            } else if (waitingProcess.Count > 0) {
                var p = waitingProcess.Pop();
                Do(p);
            } else {
                End();
            }
        }
        public void End() {
            nextTickEnd = true;
        }
        public void NextPlayer() {
            ++cur_player_index;
            if (cur_player_index == _players.Count) {
                cur_player_index = 0;
            }
        }

        void init() {
            replaceProcess.Clear();
            priorityProcess.Clear();
            waitingProcess.Clear();

        }

        List<KeyValuePair<string, ConditionProcess>> replaceProcess = new List<KeyValuePair<string, ConditionProcess>>();
        Stack<IProcess> priorityProcess = new Stack<IProcess>();
        Stack<IProcess> waitingProcess = new Stack<IProcess>();

        public void Do(IProcess process) { //priority
            if (priorityProcess.Count > 0) {
                waitingProcess.Push(process);
                var priP = priorityProcess.Pop();
                priP.Do();//
                return;
            }
            var pro = replaceProcess.ToLookup((x) => x.Key)[process.Name];
            if (pro.Count() != 0) { //replace
                var rep = pro.ElementAt(0).Value;
                if (rep.Condition()) {
                    //replaceProcess.Remove(pro.ElementAt(0));
                    rep.Process.Do();//
                    return;
                }
            }

            if (process != null) { //nornal
                process.Do();
            }
        }
        bool islock = false;
        internal void ListenProcess(BGO bgo) {
            if (!islock) {
                islock = true;
                if (bgo == null) throw new NullReferenceException();
                listeningProcess.Listen(bgo);
                ++ListenMsgTimes;
                islock = false;
            }
        }

        public void CallPriorityNextProcess(IProcess process) {
            priorityProcess.Push(process);
        }
        public void RegisterReplaceProcess(string replacedName, IProcess process) {
            var cp = new ConditionProcess(()=>true, process);
            replaceProcess.Add(new KeyValuePair<string, ConditionProcess>(replacedName, cp));
        }
        public void RegisterReplaceProcess(string replacedName, Func<bool> condition, IProcess process) {
            var cp = new ConditionProcess(condition, process);
            replaceProcess.Add(new KeyValuePair<string, ConditionProcess>(replacedName,cp));
        }
        class ConditionProcess {
            public Func<bool> Condition { get; set; }
            public IProcess Process { get; set; }
            public ConditionProcess(Func<bool> Condition, IProcess Process) {
                this.Condition = Condition;
                this.Process = Process;
            }
        }

        //action button
        public ActionButton AddActionButton(string text) {
            var but = new ActionButton() { Text = text };
            _selectItems.Add(but);
            return but;
        }
        public IEnumerable<ActionButton> AddActionButtons(IEnumerable<string> text) {
            List<ActionButton> list =new List<ActionButton>();
            foreach (var it in text) {
                var but = new ActionButton() { Text = it };
                list.Add(but);
                _selectItems.Add(but);
            }
            return list;
        }
        public void RemoveActionButton(string text) {
            var items = _selectItems.Where((x) => x.Text == text).ToList();
            foreach (var item in items) {
                _selectItems.Remove(item);
            }
        }
        public void ClearActionButton() {
            _selectItems.Clear();
        }
        
        //player
        public void AddPlayer(Player player) {
            if (IsEnabled) throw new BGSSystemError("Add player failed");
            _players.Add(player);
        }
        public void AddPlayer(params Player[] player) {
            AddPlayer(player as IEnumerable<Player>);
        }
        public void AddPlayer(IEnumerable<Player> player) {
            if (IsEnabled) throw new BGSSystemError("Add player failed");
            _players.AddRange(player);
        }
        public void RemovePlayer(Player player) {
            if (IsEnabled) throw new BGSSystemError("Remove player failed");
            _players.Remove(player);
        }
        public void PlayerClear() {
            _players.Clear();
        }
     

        //make
        public PlayerAction NewPlayerAction(string name) {
            var p = new PlayerAction();
            p.Name = name;
            p.System = this;
            return p;
        }
        public PlayerAction<T> NewPlayerAction<T>(string name) where T : BGO {
            var p = new PlayerAction<T>();
            p.Name = name;
            p.System = this;
            return p;
        }
        public ActionBranch NewActionBranch(string name) {
            var p = new ActionBranch();
            p.Name = name;
            p.System = this;
            return p;
        }
        public MultiChoiceAction NewMultiChoiceProcess(string name) {
            var p = new MultiChoiceAction();
            p.Name = name;
            p.System = this;
            return p;
        }
        public MultiChoiceAction<T> NewMultiChoiceProcess<T>(string name) where T : BGO {
            var p = new MultiChoiceAction<T>();
            p.Name = name;
            p.System = this;
            return p;
        }
        public FlowBranch NewFlowBranch(string name) {
            var p = new FlowBranch();
            p.Name = name;
            p.System = this;
            return p;
        }
        public ButtonAction NewButtonAction(string name) {
            var p = new ButtonAction();
            p.Name = name;
            p.System = this;
            return p;
        }
        public Program NewProgram(string name) {
            var p = new Program();
            p.Name = name;
            p.System = this;
            return p;
        }
        public Group NewGroup(string name) {
            var p = new Group();
            p.Name = name;
            p.System = this;
            return p;
        }

      
    }

    public class ActionButton : BGO {
        public string Text { get; set; }
    }
}
