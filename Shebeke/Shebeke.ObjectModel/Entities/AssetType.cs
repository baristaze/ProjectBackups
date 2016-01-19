using System;
using System.Runtime.Serialization;

namespace Shebeke.ObjectModel
{
    [DataContract]
    public enum AssetType
    {
        // do not change values; they have corresponding DBase values
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        User = 1,

        [EnumMember]
        Topic = 2,

        [EnumMember]
        Entry = 3
    }
}
