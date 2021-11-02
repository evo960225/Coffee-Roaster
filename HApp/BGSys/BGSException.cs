using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BGS {
    public class BGSException : HException {

        public BGSException(string message)
            : base(message) {

        }
        public BGSException(string message, Exception innerException)
            : base(message, innerException) {

        }
    }

    public class BGSProcessException : BGSException {
        public BGSProcessException(string message)
            : base(message) {

        }
        public BGSProcessException(string message, Exception innerException)
            : base(message, innerException) {

        }

    }

    public class BGSSystemError : BGSException {
        public BGSSystemError(string message)
            : base(message) {

        }
        public BGSSystemError(string message, Exception innerException)
            : base(message, innerException) {

        }

    }
}
