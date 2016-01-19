using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Crosspl.ObjectModel
{
    [DataContract()]
    public class UserAuthInfo
    {
        [DataMember()]
        public long UserId { get; set; }

        [DataMember()]
        public UserType UserType { get; set; }

        [DataMember()]
        public int SplitId { get; set; }

        [DataMember()]
        public string FirstName { get; set; }

        [DataMember()]
        public string LastName { get; set; }

        [DataMember()]
        public OAuthProvider OAuthProvider { get; set; }

        [DataMember()]
        public string OAuthUserId { get; set; }

        [DataMember()]
        public string OAuthAccessToken { get; set; }

        [DataMember()]
        public string PhotoUrl { get; set; }

        [DataMember()]
        public DateTime CreateTimeUtc { get; set; }

        public UserAuthInfo()
        {
            this.CreateTimeUtc = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}",  // format
                this.UserId,
                (int)this.UserType,
                this.SplitId,
                (int)this.OAuthProvider,
                this.OAuthUserId,
                this.OAuthAccessToken,
                this.CreateTimeUtc.Ticks,
                this.FirstName,
                this.LastName,
                this.PhotoUrl
                );
        }

        public static UserAuthInfo Parse(string token)
        {
            string[] tokens = token.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                UserAuthInfo info = new UserAuthInfo();
                info.UserId = Int64.Parse(tokens[0]);
                info.UserType = (UserType)Int32.Parse(tokens[1]);
                info.SplitId = Int32.Parse(tokens[2]);
                info.OAuthProvider = (OAuthProvider)Int32.Parse(tokens[3]);
                info.OAuthUserId = tokens[4];
                info.OAuthAccessToken = tokens[5];
                info.CreateTimeUtc = new DateTime(Int64.Parse(tokens[6]));
                info.FirstName = tokens[7];
                info.LastName = tokens[8];
                info.PhotoUrl = tokens[9];
                return info;
            }
            catch (Exception ex)
            {
                throw new CrossplArgumentException("Invalid authentication token to parse", ex);
            }
        }
    }
}
