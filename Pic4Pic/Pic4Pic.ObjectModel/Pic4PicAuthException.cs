using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pic4Pic.ObjectModel
{
    public class Pic4PicAuthException : Pic4PicException
    {
        public Pic4PicAuthException() : this(null) { }
        public Pic4PicAuthException(string message) : this(message, null) { }
        public Pic4PicAuthException(string message, Exception inner) : base(message, inner) { }
    }
}
