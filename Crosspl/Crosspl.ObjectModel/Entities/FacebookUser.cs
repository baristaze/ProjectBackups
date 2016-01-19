using System;
using System.Globalization;

namespace Crosspl.ObjectModel
{
    public partial class FacebookUser : UserInfo, IDBEntity
    {
        public long FacebookId { get; set; }
        public string FullName { get; set; }
        public string FacebookLink { get; set; }
        public string FacebookUserName { get; set; }
        public long HometownId { get; set; }
        public string CurrentLocation { get; set; }
        public long CurrentLocationId { get; set; }
        public string ISOLocale { get; set; }
        public bool IsVerified { get; set; }
        public string PhotoUrl { get; set; }

        public DateTime CreateTimeUTC{get;set;}
        public DateTime LastUpdateTimeUTC { get; set; }

        public string GetPhotoUrlOrDefault(string rootUrl)
        {
            if (String.IsNullOrWhiteSpace(this.PhotoUrl))
            {
                if (this.FacebookId > 0)
                {
                    return "https://graph.facebook.com/" + this.FacebookId.ToString() + "/picture?type=small";
                }
                else
                {
                    return rootUrl + "/Images/user2.png";
                }
            }
            else
            {
                return this.PhotoUrl;
            }
        }

        public FacebookUser()
        {
            DateTime utc = DateTime.UtcNow;
            this.CreateTimeUTC = utc;
            this.LastUpdateTimeUTC = utc;
        }

        public override string ToString()
        {
            return this.FacebookId.ToString() + " - " + this.FullName;
        }

        public UserAuthInfo GetAuthInfo(string accessToken)
        {
            UserAuthInfo authInfo = new UserAuthInfo();
            authInfo.UserId = this.Id;
            authInfo.OAuthUserId = this.FacebookId.ToString();
            authInfo.OAuthProvider = OAuthProvider.Facebook;
            authInfo.FirstName = this.FirstName;
            authInfo.LastName = this.LastName;
            authInfo.OAuthAccessToken = accessToken;
            if (String.IsNullOrWhiteSpace(this.PhotoUrl))
            {
                authInfo.PhotoUrl = String.Format(CultureInfo.InvariantCulture, "https://graph.facebook.com/{0}/picture?type=small", this.FacebookId);
            }
            else
            {
                authInfo.PhotoUrl = this.PhotoUrl;
            }
            
            authInfo.UserType = this.UserType;
            authInfo.SplitId = this.SplitId;

            return authInfo;
        }

        public bool Merge(FacebookUser old)
        {
            bool needUpdate = false;
            this.CreateTimeUTC = old.CreateTimeUTC;
            
            this.FirstName = MergeProperty(this.FirstName, old.FirstName, ref needUpdate);
            this.LastName = MergeProperty(this.LastName, old.LastName, ref needUpdate);
            this.FullName = MergeProperty(this.FullName, old.FullName, ref needUpdate);
            this.EmailAddress = MergeProperty(this.EmailAddress, old.EmailAddress, ref needUpdate);
            this.BirthDay = MergeProperty(this.BirthDay, old.BirthDay, ref needUpdate);
            this.FacebookLink = MergeProperty(this.FacebookLink, old.FacebookLink, ref needUpdate);
            this.FacebookUserName = MergeProperty(this.FacebookUserName, old.FacebookUserName, ref needUpdate);
            this.Hometown = MergeProperty(this.Hometown, old.Hometown, ref needUpdate);
            this.HometownId = MergeProperty(this.HometownId, old.HometownId, ref needUpdate);
            this.TimeZoneOffset = MergeProperty(this.TimeZoneOffset, old.TimeZoneOffset, ref needUpdate);
            this.CurrentLocation = MergeProperty(this.CurrentLocation, old.CurrentLocation, ref needUpdate);
            this.CurrentLocationId = MergeProperty(this.CurrentLocationId, old.CurrentLocationId, ref needUpdate);
            this.ISOLocale = MergeProperty(this.ISOLocale, old.ISOLocale, ref needUpdate);
            this.PhotoUrl = MergeProperty(this.PhotoUrl, old.PhotoUrl, ref needUpdate);
            
            this.IsVerified = this.IsVerified | old.IsVerified;
            if (this.IsVerified != old.IsVerified)
            {
                needUpdate = true;
            }

            if (this.Gender != old.Gender)
            {
                needUpdate = true;
            }

            if (!needUpdate)
            {
                TimeSpan diff = DateTime.UtcNow - old.LastUpdateTimeUTC;
                if (diff.TotalDays >= 7)
                {
                    needUpdate = true;
                }
            }

            return needUpdate;
        }

        private string MergeProperty(string newVal, string oldVal, ref bool needUpdate)
        {
            // [James] (new)   [] (database value)
            // [James] (new)   [John] (database value)
            needUpdate |= !String.IsNullOrWhiteSpace(newVal) && newVal != oldVal;

            // [] new   [John] (old)
            if (String.IsNullOrWhiteSpace(newVal) && !String.IsNullOrWhiteSpace(oldVal))
            {
                newVal = oldVal;
            }

            return newVal;
        }

        private DateTime MergeProperty(DateTime newVal, DateTime oldVal, ref bool needUpdate)
        {
            // [James] (new)   [] (database value)
            // [James] (new)   [John] (database value)
            needUpdate |= (newVal != default(DateTime) && newVal != oldVal);

            // [] new   [John] (old)
            if (newVal == default(DateTime) && oldVal != default(DateTime))
            {
                newVal = oldVal;
            }

            return newVal;
        }

        private long MergeProperty(long newVal, long oldVal, ref bool needUpdate)
        {
            // [James] (new)   [] (database value)
            // [James] (new)   [John] (database value)
            needUpdate |= (newVal > 0 && newVal != oldVal);

            // [] new   [John] (old)
            if (newVal <=0 && oldVal > 0)
            {
                newVal = oldVal;
            }

            return newVal;
        }

        private int MergeProperty(int newVal, int oldVal, ref bool needUpdate)
        {
            // [James] (new)   [] (database value)
            // [James] (new)   [John] (database value)
            needUpdate |= (newVal > 0 && newVal != oldVal);

            // [] new   [John] (old)
            if (newVal <= 0 && oldVal > 0)
            {
                newVal = oldVal;
            }

            return newVal;
        }

        public static FacebookUser CreateFacebookUser(dynamic user)
        {
            FacebookUser fbUser = new FacebookUser();

            if (user.id!= null && !String.IsNullOrWhiteSpace(user.id))
            {
                fbUser.FacebookId = Int64.Parse(user.id.ToString());
            }

            if (user.name != null)
            {
                fbUser.FullName = user.name;
            }

            if (user.first_name != null)
            {
                fbUser.FirstName = user.first_name;
            }

            if (user.last_name != null)
            {
                fbUser.LastName = user.last_name;
            }

            if (user.link != null)
            {
                fbUser.FacebookLink = user.link;
            }

            if (user.username != null)
            {
                fbUser.FacebookUserName = user.username;
            }

            if (user.birthday != null && !String.IsNullOrWhiteSpace(user.birthday))
            {
                fbUser.BirthDay = DateTime.Parse(user.birthday.ToString());
            }

            if (user.hometown != null && user.hometown.name != null)
            {
                fbUser.Hometown = user.hometown.name;
            }

            if (user.hometown != null && !String.IsNullOrWhiteSpace(user.hometown.id))
            {
                fbUser.HometownId = Int64.Parse(user.hometown.id.ToString());
            }

            if (user.location != null && user.location.name != null)
            {
                fbUser.CurrentLocation = user.location.name;
            }
            
            if (user.location != null && user.location.id != null && !String.IsNullOrWhiteSpace(user.location.id))
            {
                fbUser.CurrentLocationId = Int64.Parse(user.location.id.ToString());
            }

            if (user.gender != null)
            {
                fbUser.Gender = GenderHelper.GetGenderFromString(user.gender);
            }

            if (user.email != null)
            {
                fbUser.EmailAddress = user.email;
            }

            if (user.timezone != null)
            {
                fbUser.TimeZoneOffset = Int32.Parse(user.timezone.ToString());
            }

            if (user.locale != null)
            {
                fbUser.ISOLocale = user.locale;
            }

            if (user.verified != null)
            {
                fbUser.IsVerified = user.verified;
            }

            if (user.picture != null && user.picture.data != null && user.picture.data.url != null)
            {
                fbUser.PhotoUrl = null; // not a good idea as FB doesn't like using CDN links. Also user might change profile picture.
                // fbUser.PhotoUrl = user.picture.data.url.ToString();
            }

            return fbUser;
        }
    }
}
