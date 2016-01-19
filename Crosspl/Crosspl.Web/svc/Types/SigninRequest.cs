using System;
using System.Runtime.Serialization;

using Crosspl.ObjectModel;

namespace Crosspl.Web.Services
{
    [DataContract()]
    public class SigninRequest
    {
        [DataMember()]
        public OAuthProvider OAuthProvider { get; set; }

        [DataMember()]
        public string OAuthUserId { get; set; }

        [DataMember()]
        public string OAuthAccessToken { get; set; }

        [DataMember()]
        public int OAuthExpiresInSeconds { get; set; }

        [DataMember()]
        public int SplitId { get; set; }

        public long OAuthUserIdAsInt64 
        {
            get
            {
                return Int64.Parse(this.OAuthUserId);
            }
        }

        public DateTime OAuthExpireTimeUTC
        {
            get 
            {
                return DateTime.UtcNow.AddSeconds(this.OAuthExpiresInSeconds);
            }
        }
    }
}
