using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public enum UserType
    {
        [EnumMember]
        Guest = 0,

        [EnumMember]
        Regular,

        [EnumMember]
        Editor,

        [EnumMember]
        Moderator,

        [EnumMember]
        PowerUser,

        [EnumMember]
        SystemAdmin,
    }
}
