using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shebeke.ObjectModel
{
    public class ShebekeArgumentException : ArgumentException
    {
        public ShebekeArgumentException() : this(null) { }
        public ShebekeArgumentException(string message) : this(message, (Exception)null) { }
        public ShebekeArgumentException(string message, Exception inner) : base(message, inner) { }
        public ShebekeArgumentException(string message, string paramName) : base(message, paramName) { }
    }

}