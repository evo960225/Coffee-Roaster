using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGS {

    public class Group : IGroup, IChainProcess {
       
        public string Name { get; set; }
        public string Message { get; set; }
        public BGSys System { get; internal set; }
        public IProcess NextProcess {
            get { return EndProcess.NextProcess; }
            set { EndProcess.NextProcess = value; }
        }
        public IProcess StartProcess { get; set; }
        public IChainProcess EndProcess { get; set; }

        public void Do() {
            if (StartProcess != null) {
                StartProcess.Do();
            }
        }

        public void Break() {
            throw new NotImplementedException();
        }

    }

}
