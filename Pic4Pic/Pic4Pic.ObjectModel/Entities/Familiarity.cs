using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public enum Familiarity
    {
        [EnumMember]
        Stranger = 0,

        [EnumMember]
	    Familiar,
    }
}
