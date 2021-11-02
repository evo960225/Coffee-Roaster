using System;
using System.Collections.Generic;

namespace BGS {

    sealed public class FlowBranch : IBranch {

        public string Name { get; set; }
        public string Message { get; set; }
        public BGSys System { get; internal set; }

        public Func<IProcess> NextProcessFunc {
            get { return _nextProcessFunc; }
            set { _nextProcessFunc = value; }
        }
        Func<IProcess> _nextProcessFunc;
        internal FlowBranch() {

        }

        public void Do() {
            if (_nextProcessFunc == null) throw new BGSProcessException("FlowBranch has't NextProcessFunc");
            var process = _nextProcessFunc();
            if (process == null) System.Break();
            else {
                System.Do(process);
            }
        }
        public void Break() {

        }
    }
}
