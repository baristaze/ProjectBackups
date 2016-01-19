using System;
using System.Runtime.Serialization;

namespace Crosspl.ObjectModel
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
