using System;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public class AuthTokenResponse : BaseResponse
    {
        [DataMember()]
        public string Token { get; set; }

        [DataMember()]
        public int ExpiresInSeconds { get; set; }

        [DataMember()]
        public OAuthProvider OAuthProvider { get; set; }

        [DataMember()]
        public string OAuthUserId { get; set; }
    }
}
