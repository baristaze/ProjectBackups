using System;
using System.Runtime.Serialization;

namespace Shebeke.ObjectModel
{
    [DataContract]
    public enum SocialChannel
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        Facebook,

        [EnumMember]
        Twitter,
    }
}


