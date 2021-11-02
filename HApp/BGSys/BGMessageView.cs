using hoshi_lib.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGS {
    public class BGMessageView : HControl {
        public BGSys BGSys { get; protected set; }

        public BGMessageView(BGSys bgs) {
            BGSys = bgs;
            Background = null;
            Size = new hoshi_lib.Size(300, 50);
            BGSys.MainTimer.Tick += (s, e) => this.Update();
        }

        public void Update() {
            if (BGSys == null) return;
            Text = BGSys.CurrentProcess.Message;
        }
    }
}
