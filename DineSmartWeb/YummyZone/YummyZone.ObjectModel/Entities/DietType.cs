using System;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    [Flags]
    [DataContract]
    public enum DietType
    {
        [EnumMember]
        Unspecified = 0,

        [EnumMember]
        Kosher = 1 << 0,

        [EnumMember]
        Halal = 1 << 1,

        [EnumMember]
        Vegan = 1 << 2,

        [EnumMember]
        Vegetarian = 1 << 3,

        [EnumMember]
        GlutenFree = 1 << 4,

        [EnumMember]
        LowFat = 1 << 5,

        [EnumMember]
        LowSalt = 1 << 6,

        [EnumMember]
        Organic = 1 << 7,
    }
}
