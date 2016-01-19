using System;

namespace Crosspl.ObjectModel
{
    public class CrossplException : Exception
    {
        public CrossplException() : this(null) { }
        public CrossplException(string message) : this(message, null) { }
        public CrossplException(string message, Exception inner) : base(message, inner) { }
    }
}