using System;
using System.Globalization;

namespace Shebeke.ObjectModel
{
    public class NameValuePair<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }

        public NameValuePair() : this(null, default(T)) { }
        public NameValuePair(string name, T value)
        {
            this.Name = name;
            this.Value = value;
        }

        public override string ToString()
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0}={1}",
                this.Name,
                this.Value);
        }
    }
}
