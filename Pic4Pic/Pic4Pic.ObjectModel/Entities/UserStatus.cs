using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public enum UserStatus
    {
        [EnumMember]
        Partial = 0,

        [EnumMember]
        Active,

        [EnumMember]
        Suspended,

        [EnumMember]
        Disabled,

        [EnumMember]
        Deleted,
    }
}
