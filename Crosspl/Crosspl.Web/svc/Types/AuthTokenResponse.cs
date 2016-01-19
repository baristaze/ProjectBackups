using System;
using System.Runtime.Serialization;

using Crosspl.ObjectModel;

namespace Crosspl.Web.Services
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

        [DataMember()]
        public string UserFriendlyName { get; set; }

        [DataMember()]
        public string PhotoUrl { get; set; }
    }
}
