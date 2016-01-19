using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Shebeke.ObjectModel
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

        [DataMember()]
        public DateTime SessionStartTimeUtc { get; set; }

        public UserAuthInfo()
        {
            this.CreateTimeUtc = DateTime.UtcNow;
            this.SessionStartTimeUtc = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}",  // format
                this.UserId,
                (int)this.UserType,
                this.SplitId,
                (int)this.OAuthProvider,
                this.OAuthUserId,
                this.OAuthAccessToken,
                this.CreateTimeUtc.Ticks,
                this.SessionStartTimeUtc.Ticks,
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
                info.SessionStartTimeUtc = new DateTime(Int64.Parse(tokens[7]));
                info.FirstName = tokens[8];
                info.LastName = tokens[9];
                info.PhotoUrl = tokens[10];

                TimeSpan diff = DateTime.UtcNow - info.SessionStartTimeUtc;
                if (diff.TotalSeconds > 24 * 60 * 60)
                {
                    throw new ShebekeArgumentException("Oturum zamanaşımına uğradı. Çıkış yapıp tekrar deneyin.");
                }

                return info;
            }
            catch (Exception ex)
            {
                throw new ShebekeArgumentException("Oturum bilgisi doğrulanamadı. Çıkış yapıp tekrar deneyin.", ex);
            }
        }
    }
}
