using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crosspl.ObjectModel
{
    public class CrossplArgumentException : ArgumentException
    {
        public CrossplArgumentException() : this(null) { }
        public CrossplArgumentException(string message) : this(message, (Exception)null) { }
        public CrossplArgumentException(string message, Exception inner) : base(message, inner) { }
        public CrossplArgumentException(string message, string paramName) : base(message, paramName) { }
    }

}