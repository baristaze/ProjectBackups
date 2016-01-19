using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract]
    public class NameValuePair<T>
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public T Value { get; set; }

        public NameValuePair() : this(null, default(T)) { }
        public NameValuePair(string name, T value)
        {
            this.Name = name;
            this.Value = value;
        }

        public override string ToString()
        {
            return this.ToString("=");
        }

        public string ToString(String delim)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", this.Name, delim, this.Value);
        }
    }

    [DataContract]
    public class NameValue : NameValuePair<string>
    {
        public NameValue() : this(null, null) { }

        public NameValue(string name, string value) : base(name, value) { }

        public static string ToStringAll<T>(List<T> all, string pairDelim, string lineDelim) where T : NameValue
        {
            StringBuilder builder = new StringBuilder();

            for (int x = 0; x < all.Count; x++)
            {
                builder.Append(all[x].ToString(pairDelim));
                if (x != all.Count - 1)
                {
                    builder.Append(lineDelim);
                }
            }

            return builder.ToString();
        }
    }
}
