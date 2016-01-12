using System;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    [DataContract]
    public class NameAndValue
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} : {1}", Name, Value);
        }
    }

    public class NameAndValueList : List<NameAndValue> { }
}
