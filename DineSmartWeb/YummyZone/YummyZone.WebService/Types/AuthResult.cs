using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.WebService
{
    [DataContract()]
    public class AuthResult : BaseResponse
    {
        [DataMember()]
        public string Cookie { get; set; }

        [DataMember()]
        public UserProfile Settings { get; set; }
    }
}