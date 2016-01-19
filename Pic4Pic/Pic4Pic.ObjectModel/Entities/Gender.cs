using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public enum Gender
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        Male,

        [EnumMember]
        Female
    }

    public class GenderHelper
    {
        public static Gender GetGenderFromString(string gender)
        {
            if (!String.IsNullOrWhiteSpace(gender))
            {
                gender = gender.Trim();

                if (0 == String.Compare(gender, "Male", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Gender.Male;
                }
                else if (0 == String.Compare(gender, "Female", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Gender.Female;
                }
            }

            return Gender.Unknown;
        }
    }
}
