using System;

namespace Crosspl.ObjectModel
{
    public partial class UserToken : IEditable
    {
        public long UserId { get; set; }
        public OAuthProvider OAuthProvider { get; set; }
        public string OAuthUserId { get; set; }
        public string OAuthAccessToken { get; set; }
        public DateTime CreateTimeUTC { get; set; }
        public DateTime ExpireTimeUTC { get; set; }

        public UserToken()
        {
            this.CreateTimeUTC = DateTime.UtcNow;
        }

        public UserToken Clone()
        {
            UserToken token = new UserToken();

            token.UserId = this.UserId;
            token.OAuthProvider = this.OAuthProvider;
            token.OAuthUserId = this.OAuthUserId;
            token.OAuthAccessToken = this.OAuthAccessToken;
            token.CreateTimeUTC = this.CreateTimeUTC;
            token.ExpireTimeUTC = this.ExpireTimeUTC;

            return token;
        }
    }
}
