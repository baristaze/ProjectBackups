using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract]
    public enum AssetType
    {
        // do not change values; they have corresponding DBase values
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        User = 1,
    }
}
