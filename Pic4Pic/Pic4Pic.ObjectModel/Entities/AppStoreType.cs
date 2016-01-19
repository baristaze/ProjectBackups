using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public enum AppStoreType
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        GooglePlay = 1,
    }
}
