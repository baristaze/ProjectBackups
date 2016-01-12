using System;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    [DataContract]
    public enum Status
    {
        [EnumMember]
        Active = 0,

        [EnumMember]
        Disabled,

        [EnumMember]
        Removed,
    }
}
