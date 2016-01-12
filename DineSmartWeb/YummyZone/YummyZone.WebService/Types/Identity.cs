using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.WebService
{
    public enum UsernameType
    {
        FreeText,
        Email,
        Guid,
        EmailOrGuid
    }

    [DataContract()]
    public class Identity
    {
        internal const int MinPasswordLength = 6;
        internal const int MinUserNameLength = 6;
        internal const UsernameType DefaultUserNameFormat = UsernameType.FreeText;

        [DataMember()]
        public string UserName { get; set; }

        [DataMember()]
        public string Password { get; set; }

        public Guid TryParseUserNameAsGuidOrDefault(Guid defaultVal)
        {
            Guid userId = defaultVal;
            Guid.TryParse(this.UserName, out userId);
            return userId;
        }
    }
}