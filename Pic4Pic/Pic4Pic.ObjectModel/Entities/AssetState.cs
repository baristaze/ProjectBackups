using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public enum AssetState
    {
        [EnumMember]
        New = 0,

        [EnumMember]
        Disabled,

        [EnumMember]
        Deleted,
    }
}
