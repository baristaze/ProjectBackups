using System;

namespace Shebeke.ObjectModel
{
    public class ShebekeException : Exception
    {
        public ShebekeException() : this(null) { }
        public ShebekeException(string message) : this(message, null) { }
        public ShebekeException(string message, Exception inner) : base(message, inner) { }
    }
}