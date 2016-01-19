using System;
using System.Globalization;

namespace Pic4Pic.ObjectModel
{
    public partial class FacebookUser : UserProfile, IDBEntity
    {
        // internal class
        public class UserFieldPermissionMap
        {
            public string Field { get; set; }
            public string Permisson { get; set; }
            public UserFieldPermissionMap(string field, string permission)
            {
                this.Field = field;
                this.Permisson = permission;
            }
        }

        // fields that require extra permissions
        public const string FB_FIELD_EMAIL = "email";
        public const string FB_FIELD_HOMETOWN = "hometown";
        public const string FB_FIELD_BIRTHDAY = "birthday";
        public const string FB_FIELD_WORK = "work";
        public const string FB_FIELD_EDUCATION = "education";
        public const string FB_FIELD_RELATIONSHIP = "relationship_status";
        public const string FB_FIELD_RELIGION = "religion";
        public const string FB_FIELD_POLITICAL = "political";

        // permission names
        public const string FB_PERM_EMAIL = "email";
        public const string FB_PERM_HOMETOWN = "user_hometown";
        public const string FB_PERM_BIRTHDAY = "user_birthday";
        public const string FB_PERM_WORK = "user_work_history";
        public const string FB_PERM_EDUCATION = "user_education_history";
        public const string FB_PERM_RELATIONSHIP = "user_relationships";
        public const string FB_PERM_RELIGION_POLITICS = "user_religion_politics";

        // permission maps
        public static readonly UserFieldPermissionMap[] userFieldPermissionMaps = new UserFieldPermissionMap[] 
        { 
            new UserFieldPermissionMap(FB_FIELD_EMAIL, FB_PERM_EMAIL),
            new UserFieldPermissionMap(FB_FIELD_HOMETOWN, FB_PERM_HOMETOWN),
            new UserFieldPermissionMap(FB_FIELD_BIRTHDAY, FB_PERM_BIRTHDAY),
            new UserFieldPermissionMap(FB_FIELD_WORK, FB_PERM_WORK),
            new UserFieldPermissionMap(FB_FIELD_EDUCATION, FB_PERM_EDUCATION),
            new UserFieldPermissionMap(FB_FIELD_RELATIONSHIP, FB_PERM_RELATIONSHIP),
            new UserFieldPermissionMap(FB_FIELD_RELIGION, FB_PERM_RELIGION_POLITICS),
            new UserFieldPermissionMap(FB_FIELD_POLITICAL, FB_PERM_RELIGION_POLITICS),
        };

        // facebook specific fields
        public long FacebookId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string FacebookLink { get; set; }
        public string FacebookUserName { get; set; }
        public string MaritalStatusAsText { get; set; }
        public long HometownId { get; set; }
        public string CurrentLocationCity { get; set; }
        public string CurrentLocationState { get; set; }
        public long CurrentLocationId { get; set; }
        public string ISOLocale { get; set; }
        public bool IsVerified { get; set; }
        public string PhotoUrl { get; set; }

        public string GetPhotoUrl()
        {
            if (String.IsNullOrWhiteSpace(this.PhotoUrl))
            {
                if (this.FacebookId > 0)
                {
                    return "https://graph.facebook.com/" + this.FacebookId.ToString() + "/picture?type=small";
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return this.PhotoUrl;
            }
        }

        public override string ToString()
        {
            string s = this.FacebookId.ToString() + " - " + this.FullName;
            if (!String.IsNullOrWhiteSpace(this.Username)) 
            {
                s += " - " + this.Username;
            }

            return  s;
        }
        
        // 11 base fields, 15 fields => 2 identity fields => 24 fields to merge
        public bool Merge(FacebookUser old)
        {
            bool needUpdate = false;

            // Keep the CreateTimeUTC of old record. 
            this.CreateTimeUTC = old.CreateTimeUTC;

            this.FirstName = MergeProperty(this.FirstName, old.FirstName, ref needUpdate);
            this.LastName = MergeProperty(this.LastName, old.LastName, ref needUpdate);
            this.FullName = MergeProperty(this.FullName, old.FullName, ref needUpdate);
            this.EmailAddress = MergeProperty(this.EmailAddress, old.EmailAddress, ref needUpdate);
            this.Gender = (Gender)MergeProperty((byte)this.Gender, (byte)old.Gender, ref needUpdate);
            this.MaritalStatus = (MaritalStatus)MergeProperty((byte)this.MaritalStatus, (byte)old.MaritalStatus, ref needUpdate);
            this.MaritalStatusAsText = MergeProperty(this.MaritalStatusAsText, old.MaritalStatusAsText, ref needUpdate);
            this.BirthDay = MergeProperty(this.BirthDay, old.BirthDay, ref needUpdate);
            this.Profession = MergeProperty(this.Profession, old.Profession, ref needUpdate);
            this.EducationLevel = (EducationLevel)MergeProperty((byte)this.EducationLevel, (byte)old.EducationLevel, ref needUpdate);
            this.FacebookLink = MergeProperty(this.FacebookLink, old.FacebookLink, ref needUpdate);
            this.FacebookUserName = MergeProperty(this.FacebookUserName, old.FacebookUserName, ref needUpdate);
            this.HometownCity = MergeProperty(this.HometownCity, old.HometownCity, ref needUpdate);
            this.HometownState = MergeProperty(this.HometownState, old.HometownState, ref needUpdate);
            this.HometownId = MergeProperty(this.HometownId, old.HometownId, ref needUpdate);
            this.PhotoUrl = MergeProperty(this.PhotoUrl, old.PhotoUrl, ref needUpdate);
            this.TimeZoneOffset = MergeProperty(this.TimeZoneOffset, old.TimeZoneOffset, ref needUpdate);
            this.CurrentLocationCity = MergeProperty(this.CurrentLocationCity, old.CurrentLocationCity, ref needUpdate);
            this.CurrentLocationState = MergeProperty(this.CurrentLocationState, old.CurrentLocationState, ref needUpdate);
            this.CurrentLocationId = MergeProperty(this.CurrentLocationId, old.CurrentLocationId, ref needUpdate);
            this.ISOLocale = MergeProperty(this.ISOLocale, old.ISOLocale, ref needUpdate);
            
            // merge IsVerified
            this.IsVerified = this.IsVerified | old.IsVerified;
            if (this.IsVerified != old.IsVerified)
            {
                needUpdate = true;
            }

            // merge update time
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
            needUpdate |= (newVal > 0 && newVal != oldVal);
            if (newVal <= 0 && oldVal > 0)
            {
                newVal = oldVal;
            }

            return newVal;
        }

        private byte MergeProperty(byte newVal, byte oldVal, ref bool needUpdate)
        {
            needUpdate |= (newVal > 0 && newVal != oldVal);
            if (newVal <= 0 && oldVal > 0)
            {
                newVal = oldVal;
            }

            return newVal;
        }

        // 11 base fields, 15 fields => 1 external field, 2 auto-time => 23 fields to fill up
        public static FacebookUser CreateFacebookUser(dynamic user, out WorkHistory workHistory, out EducationHistory eduHistory)
        {
            FacebookUser fbUser = new FacebookUser();

            if (user.id != null && !String.IsNullOrWhiteSpace(user.id))
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

            if (user.email != null)
            {
                fbUser.EmailAddress = user.email;
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

            if (user.hometown != null)
            {
                if (!String.IsNullOrWhiteSpace(user.hometown.id))
                {
                    fbUser.HometownId = Int64.Parse(user.hometown.id.ToString());
                }

                if (user.hometown.name != null)
                {
                    string state = null;
                    string city = user.hometown.name;
                    int commaIndex = city.LastIndexOf(',');
                    if (commaIndex > 0) // not >=
                    {
                        // make sure that state is handled first!!!
                        state = city.Substring(commaIndex + 1).Trim();
                        city = city.Substring(0, commaIndex).Trim();
                    }

                    fbUser.HometownCity = city;
                    fbUser.HometownState = state;
                }
            }


            if (user.location != null)
            {
                if (user.location.id != null && !String.IsNullOrWhiteSpace(user.location.id))
                {
                    fbUser.CurrentLocationId = Int64.Parse(user.location.id.ToString());
                }

                if (user.location.name != null)
                {
                    string state = null;
                    string city = user.location.name;
                    int commaIndex = city.LastIndexOf(',');
                    if (commaIndex > 0) // not >=
                    {
                        // make sure that state is handled first!!!
                        state = city.Substring(commaIndex + 1).Trim();
                        city = city.Substring(0, commaIndex).Trim();
                    }

                    fbUser.CurrentLocationCity = city;
                    fbUser.CurrentLocationState = state;
                }
            }
            
            if (user.gender != null)
            {
                fbUser.Gender = GenderHelper.GetGenderFromString(user.gender);
            }

            if (user.relationship_status != null && !String.IsNullOrWhiteSpace(user.relationship_status))
            {
                fbUser.MaritalStatusAsText = user.relationship_status;
                fbUser.MaritalStatus = MaritalStatusHelper.Parse(fbUser.MaritalStatusAsText);
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
                fbUser.PhotoUrl = user.picture.data.url.ToString();
            }

            workHistory = null;
            if (user.work != null && user.work.Count > 0)
            {
                workHistory = WorkHistory.CreateFromFacebookArray(user.work);
                if (workHistory != null && workHistory.Count > 0)
                {
                    fbUser.Profession = workHistory.SortAndGetLastPositionName(false, 10);
                }
            }

            eduHistory = null;
            if (user.education != null && user.education.Count > 0)
            {
                eduHistory = EducationHistory.CreateFromFacebookArray(user.education);
                if (eduHistory != null && eduHistory.Count > 0)
                {
                    fbUser.EducationLevel = eduHistory.SortAndGetHighestEducationLevel();
                }
            }

            return fbUser;
        }
    }
}
