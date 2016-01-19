using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shebeke.ObjectModel
{
    public class ShebekeAuthException : ShebekeException
    {
        public ShebekeAuthException() : this(null) { }
        public ShebekeAuthException(string message) : this(message, null) { }
        public ShebekeAuthException(string message, Exception inner) : base(message, inner) { }
    }
}
