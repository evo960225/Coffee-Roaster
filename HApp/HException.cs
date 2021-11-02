using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace BGS {
    public class HException : Exception {
        public HException(string msg)
            : base(msg) {
                MessageBox.Show(msg);
        }
        public HException(string msg, Exception inner)
            : base(msg, inner) {
                MessageBox.Show(msg);
        }
    }
    public class EventDelegateIsNullException : HException {
        public EventDelegateIsNullException(string msg)
            : base(msg) {

        }
        public EventDelegateIsNullException(string msg, Exception inner)
            : base(msg, inner) {

        }
        
    }
    public class ContainerOutofRangeException : HException {
         public ContainerOutofRangeException(string msg)
            : base(msg) {

        }
         public ContainerOutofRangeException(string msg, Exception inner)
            : base(msg, inner) {

        }
    }

    public class TreeLoopException : HException {
        public TreeLoopException(string msg)
            : base(msg) {

        }
        public TreeLoopException(string msg, Exception inner)
            : base(msg, inner) {

        }
    }

    public class TypeOfTypeException : HException {
        public TypeOfTypeException(string msg)
            : base(msg) {

        }
        public TypeOfTypeException(string msg, Exception inner)
            : base(msg, inner) {

        }
    }
}
