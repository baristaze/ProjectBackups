using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public class UserAuthInfo
    {
        [DataMember()]
        public Guid UserId { get; set; }

        [DataMember()]
        public String Username { get; set; }

        [DataMember()]
        public UserType UserType { get; set; }

        [DataMember()]
        public int SplitId { get; set; }

        [DataMember()]
        public DateTime SessionStartTimeUtc { get; set; }

        public UserAuthInfo()
        {
            this.SessionStartTimeUtc = DateTime.UtcNow;
        }

        public UserAuthInfo(User user)
        {
            this.UserId = user.Id;
            this.Username = user.Username;
            this.UserType = user.UserType;
            this.SplitId = user.SplitId;
            this.SessionStartTimeUtc = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0};{1};{2};{3};{4};padding;{5};{6};{7}",  // format
                this.UserId,
                this.Username,
                (int)this.UserType,
                this.SplitId,
                this.SessionStartTimeUtc.Ticks,
                Guid.NewGuid(), // extra padding
                Guid.NewGuid(), // extra padding
                Guid.NewGuid()  // extra padding
                );
        }

        public bool IsDefaultUser
        {
            get
            {
                return (this.UserId == Guid.Empty);
            }
        }

        public static UserAuthInfo CreateDefault()
        {
            return new UserAuthInfo()
            {
                UserId = Guid.Empty,
                UserType = UserType.Guest,
                SplitId = 0,
            };
        }

        public static UserAuthInfo Parse(string token)
        {   
            try
            {
                string[] tokens = token.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                UserAuthInfo info = new UserAuthInfo();
                info.UserId = Guid.Parse(tokens[0]);
                info.Username = tokens[1];
                info.UserType = (UserType)Int32.Parse(tokens[2]);
                info.SplitId = Int32.Parse(tokens[3]);
                info.SessionStartTimeUtc = new DateTime(Int64.Parse(tokens[4]));

                //string paddingKey = tokens[5];
                //string extraPad1 = tokens[6];
                //string extraPad2 = tokens[7];
                //string extraPad3 = tokens[8]; // 7+1

                /*
                TimeSpan diff = DateTime.UtcNow - info.SessionStartTimeUtc;
                if (diff.TotalSeconds > 24 * 60 * 60)
                {
                    throw new Pic4PicArgumentException("Session timeout out. Please logout and re-login");
                }
                */
                return info;
            }
            catch (Exception ex)
            {
                throw new Pic4PicArgumentException("Your session couldn't be verified", ex);
            }
        }
    }
}
