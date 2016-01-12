using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YummyZone.WebService
{
    public class YummyZoneException : Exception
    {
        public YummyZoneException() : this(null) { }
        public YummyZoneException(string message) : this(message, null) { }
        public YummyZoneException(string message, Exception inner) : base(message, inner) { }
    }
}