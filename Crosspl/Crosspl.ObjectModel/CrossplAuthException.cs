using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crosspl.ObjectModel
{
    public class CrossplAuthException : CrossplException
    {
        public CrossplAuthException() : this(null) { }
        public CrossplAuthException(string message) : this(message, null) { }
        public CrossplAuthException(string message, Exception inner) : base(message, inner) { }
    }
}
