using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public enum EducationLevel
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        Elementary,

        [EnumMember]
        HighSchool,

        [EnumMember]
        College,

        [EnumMember]
        Master,

        [EnumMember]
        PhdOrAbove
    }

    public class EducationLevelHelper
    {
        public static EducationLevel ParseFromFacebook(string edu)
        {
            if (String.Compare(edu, "Elementary School", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return EducationLevel.Elementary;
            }

            if (String.Compare(edu, "High School", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return EducationLevel.HighSchool;
            }

            if (String.Compare(edu, "College", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return EducationLevel.College;
            }

            if (String.Compare(edu, "Graduate School", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return EducationLevel.Master;
            }

            return EducationLevel.Unknown;
        }

        public static string ToString(EducationLevel edu)
        {
            if (edu == EducationLevel.Elementary)
            {
                return "Elementary School";
            }

            if (edu == EducationLevel.HighSchool)
            {
                return "High School";
            }

            if (edu == EducationLevel.College)
            {
                return "College";
            }

            if (edu == EducationLevel.Master)
            {
                return "Master";
            }

            if (edu == EducationLevel.PhdOrAbove)
            {
                return "PhD";
            }

            return "Unknown";
        }

        public static string ToUserFriendlyString(EducationLevel edu)
        {
            if (edu == EducationLevel.Elementary)
            {
                return "Elementary school grad";
            }

            if (edu == EducationLevel.HighSchool)
            {
                return "High school grad";
            }

            if (edu == EducationLevel.College)
            {
                return "College grad";
            }

            if (edu == EducationLevel.Master)
            {
                return "Master degree";
            }

            if (edu == EducationLevel.PhdOrAbove)
            {
                return "PhD degree";
            }

            return "Unknown";
        }
    }
}
