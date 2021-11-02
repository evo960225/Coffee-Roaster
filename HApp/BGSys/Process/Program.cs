using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGS {

    sealed public class Program : IChainProcess {

        public IProcess NextProcess { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public BGSys System { get; internal set; }

        public Action DoProgram {
            protected get { return _doProgram; }
            set { _doProgram = value; }
        }
        Action _doProgram;

        internal Program() {

        }

        public void Do() {
            if (_doProgram == null) throw new BGSProcessException("Program's DoProgram is null");
            _doProgram();
            next();
        }

        public void Break() {
            
        }

        void next() {
            if (NextProcess == null) System.Break();
            else {
                System.Do(NextProcess);
            }
        }
    }
}
