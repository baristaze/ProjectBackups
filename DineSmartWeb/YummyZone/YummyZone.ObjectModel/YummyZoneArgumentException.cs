using System;

namespace YummyZone.ObjectModel
{
    public class YummyZoneArgumentException : ArgumentException
    {
        public YummyZoneArgumentException() : this(null) { }
        public YummyZoneArgumentException(string message) : this(message, (Exception)null) { }
        public YummyZoneArgumentException(string message, Exception inner) : base(message, inner) { }
        public YummyZoneArgumentException(string message, string paramName) : base(message, paramName) { }
    }
}
