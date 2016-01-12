using System;

namespace YummyZone.ObjectModel
{
    public class YummyZoneException : Exception
    {
        public YummyZoneException() : this(null) { }
        public YummyZoneException(string message) : this(message, null) { }
        public YummyZoneException(string message, Exception inner) : base(message, inner) { }
    }
}
