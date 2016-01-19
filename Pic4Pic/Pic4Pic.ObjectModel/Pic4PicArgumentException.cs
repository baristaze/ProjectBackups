using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pic4Pic.ObjectModel
{
    public class Pic4PicArgumentException : ArgumentException
    {
        public Pic4PicArgumentException() : this(null) { }
        public Pic4PicArgumentException(string message) : this(message, (Exception)null) { }
        public Pic4PicArgumentException(string message, Exception inner) : base(message, inner) { }
        public Pic4PicArgumentException(string message, string paramName) : base(message, paramName) { }
    }
}