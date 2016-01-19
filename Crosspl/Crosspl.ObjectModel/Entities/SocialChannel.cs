using System;
using System.Runtime.Serialization;

namespace Crosspl.ObjectModel
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


