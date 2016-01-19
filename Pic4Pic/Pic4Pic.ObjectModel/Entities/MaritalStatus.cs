using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public enum MaritalStatus
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        Single,

        [EnumMember]
        Married,
    }

    public class MaritalStatusHelper
    {
        public static MaritalStatus Parse(string maritalStatusAsText)
        {
            if (String.IsNullOrWhiteSpace(maritalStatusAsText))
            {
                return MaritalStatus.Unknown;
            }

            if (String.Compare(maritalStatusAsText, "Unknown", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return MaritalStatus.Unknown;
            }

            if (String.Compare(maritalStatusAsText, "Unspecified", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return MaritalStatus.Unknown;
            }

            if (String.Compare(maritalStatusAsText, "Married", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return MaritalStatus.Married;
            }

            return MaritalStatus.Single;
        }
    }
}
