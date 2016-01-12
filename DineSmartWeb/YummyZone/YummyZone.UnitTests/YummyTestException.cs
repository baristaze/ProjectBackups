using System;

namespace YummyZone.UnitTests
{
    public class YummyTestException : Exception
    {
        public YummyTestException() : this(null) { }
        public YummyTestException(string message) : this(message, null) { }
        public YummyTestException(string message, Exception inner) : base(message, inner) { }
    }
}

