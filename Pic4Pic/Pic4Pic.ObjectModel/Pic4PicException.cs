using System;

namespace Pic4Pic.ObjectModel
{
    public class Pic4PicException : Exception
    {
        public Pic4PicException() : this(null) { }
        public Pic4PicException(string message) : this(message, null) { }
        public Pic4PicException(string message, Exception inner) : base(message, inner) { }
    }
}