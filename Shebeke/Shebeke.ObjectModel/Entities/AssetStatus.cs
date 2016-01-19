using System;
using System.Runtime.Serialization;

namespace Shebeke.ObjectModel
{
    [DataContract]
    public enum AssetStatus
    {
        [EnumMember]
        New = 0,

        [EnumMember]
        UnderReview,

        [EnumMember]
        Suspended,

        [EnumMember]
        Disabled,

        [EnumMember]
        Deleted
    }
}
